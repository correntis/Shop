using Microsoft.EntityFrameworkCore;
using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using Shop.MVVM.ViewModel.AdminApp;
using Shop.UserApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.ViewModel.UserApp
{
    public class ProductsCatalogViewModel : ObservableObject
    {
        public string Title { get; set; }

        public RelayCommand FilterCommand { get; set; }
        public RelayCommand UpdateCommand { get; set; }
        public RelayCommand OpenProductCommand { get; set; }
        public RelayCommand CreateRequestCommand { get; set; }

        public Category Current { get; set; }

        public ObservableCollection<CharacterValue> SelectedValues = [];

        public List<Product> AllProducts { get; set; } = [];
        private ObservableCollection<Product> products;
        public ObservableCollection<Product> Products
        {
            get { return products; }
            set { products = value; OnPropertyChanged(nameof(Products)); }
        }

        private string nameFilter = string.Empty;
        public string NameFilter
        {
            get { return nameFilter; }
            set { nameFilter = value; OnPropertyChanged(nameof(NameFilter)); }
        }

        private decimal lowPriceFilter = 0;
        public decimal LowPriceFilter
        {
            get { return lowPriceFilter; }
            set { lowPriceFilter = value; OnPropertyChanged(nameof(LowPriceFilter)); }
        }

        private decimal highPriceFilter = 0;
        public decimal HighPriceFilter
        {
            get { return highPriceFilter; }
            set { highPriceFilter = value; OnPropertyChanged(nameof(HighPriceFilter)); }
        }
        public decimal HighPrice { get; set; }

        public ObservableCollection<object> TabControls { get; set; }


        public ProductsCatalogViewModel(ObservableCollection<object> tabControls, Category category)
        {
            TabControls = tabControls;
            Current = category;
            Title = Current?.Name;

            LoadProducts();

            SelectedValues.CollectionChanged += (sender, e) =>
            {
                ResetProducts();
            };

            FilterCommand = new(ExecuteFilter);
            UpdateCommand = new(ExecuteUpdate);
            OpenProductCommand = new(ExecuteOpenProduct);
            CreateRequestCommand = new(ExecuteCreateRequest);
        }

        private void ExecuteCreateRequest(object parameter)
        {
            if (parameter is Product product && product.Quantity <= 0)
            {
                var window = new RequestWindow(product);
                window.Show();
            }
        }

        private void ExecuteOpenProduct(object parameter)
        {
            if (parameter is Product product)
            {
                TabControls.Add(new ProductCardViewModel(TabControls, product));
            }
        }

        private void ExecuteFilter(object parameter)
        {
            ResetProducts();
        }

        private void ExecuteUpdate(object parameter)
        {
            NameFilter = string.Empty;
            ResetProducts();
        }

        private async void LoadProducts()
        {
            AllProducts = [.. 
                Current.Products
                .OrderByDescending(p => p.Quantity)
                .ThenByDescending(p => p.Id)
            ];

            HighPrice = AllProducts.OrderByDescending(p => p.Price).FirstOrDefault()?.Price ?? 100m;
            HighPriceFilter = HighPrice; 

            Products = new([.. AllProducts]);

            await RemovePhantomCharaterValues();
        }

        private void ResetProducts()
        {

            var products = AllProducts
                .Where(p =>
                {
                    return p.Name.ToLower().Contains(NameFilter.ToLower()) &&
                    p.Price <= HighPriceFilter && p.Price >= LowPriceFilter &&
                    CheckFilters(p);
                })
                .ToList();

            Products = new(products);
        }

        private bool CheckFilters(Product p)
        {
            if (SelectedValues.Count == 0)
            {
                return true;
            }

            foreach (var value in SelectedValues)
            {
                if (p.CharacterValues.Any(cv => cv.Id == value.Id))
                {
                    return true;   
                }
            }

            return false;
        }

        private async Task RemovePhantomCharaterValues()
        {
            using var context = new ShopContext();

            Current.Characters = new(await context.GetCategoryCharactersAsync(Current.Id));

            var allCategoryCharacterValues = Current.Characters.SelectMany(c => c.CharacterValues).ToList();
            var productCharacterValues = AllProducts
                .SelectMany(p => p.CharacterValues)
                .Where(cv => Current.Characters.Any(c => c.Id == cv.CharactersId))
                .DistinctBy(cv => cv.Id)
                .ToList();


            var valuesToRemove = allCategoryCharacterValues.Except(productCharacterValues, new CharacterValueComparer());

            foreach (var characterValue in valuesToRemove)
            {
                foreach (var character in Current.Characters)
                {
                    character.CharacterValues.Remove(characterValue);
                }
            }
        }

    }
}
