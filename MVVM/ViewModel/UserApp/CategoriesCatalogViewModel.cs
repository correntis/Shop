using Microsoft.EntityFrameworkCore;
using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using Shop.MVVM.ViewModel.AdminApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.ViewModel.UserApp
{
    public class CategoriesCatalogViewModel : ObservableObject, ICloneable
    {
        public string Title { get; } = "Категории";

        public RelayCommand FilterCommand { get; set; }
        public RelayCommand UpdateCommand { get; set; }

        public RelayCommand OpenCatalogCommand { get; set; }

        public List<Category> AllCategories { get; set; } = [];

        private ObservableCollection<Category> categories = [];
        public ObservableCollection<Category> Categories
        {
            get { return categories; }
            set { categories = value; OnPropertyChanged(nameof(Categories)); }
        }

        private string nameFilter = string.Empty;
        public string NameFilter
        {
            get { return nameFilter; }
            set { nameFilter = value; OnPropertyChanged(nameof(NameFilter)); }
        }

        public ObservableCollection<object> TabControls { get; set; } = [];

        private bool isLoaded = false;
        public bool IsLoaded
        {
            get { return isLoaded; }
            set { isLoaded = value; OnPropertyChanged(nameof(IsLoaded)); }
        }

        public CategoriesCatalogViewModel(ObservableCollection<object> tabControls)
        {
            TabControls = tabControls;

            _ = LoadCategoriesAsync();

            FilterCommand = new(ExecuteFilter);
            UpdateCommand = new(ExecuteUpdate);
            OpenCatalogCommand = new(ExecuteOpenCatalog);
        }

        private void ExecuteOpenCatalog(object parameter)
        {
            if (parameter is Category category)
            {
                TabControls.Add(new ProductsCatalogViewModel(TabControls, category));
            }
        }

        private void ExecuteFilter(object parameter)
        {
            ResetCategories();
        }

        private void ExecuteUpdate(object parameter)
        {
            NameFilter = string.Empty;
            ResetCategories();
        }

        private async Task LoadCategoriesAsync()
        {
            using ShopContext context = new();
           
            var categories = await context.GetCategoryCatalogAsync();
            IsLoaded = true;

            AllCategories = categories;
            Categories = new(categories);

            foreach (var category in categories)
            {
                await context.LoadCategoryCharactersAsync(category);
            }

            ResetCategories();
        }

        private void ResetCategories()
        {
            var categories = AllCategories.Where(c => c.Name.ToLower().Contains(NameFilter.ToLower()));

            Categories = new(categories);
        }

        public object Clone()
        {
            return new CategoriesCatalogViewModel(TabControls);
        }

        public void ResetProducts(object newProducts)
        {
            if (newProducts is ObservableCollection<UserProduct> userProducts)
            {
                var items = AllCategories.SelectMany(c => c.Products).Distinct();
                var allNewProducts = userProducts.Select(c => c.Product);

                foreach (var product in items)
                {
                    var newProduct = allNewProducts.FirstOrDefault(p => p.Id == product.Id);
                    if (newProduct is not null)
                    {
                        product.Quantity = newProduct.Quantity;
                    }
                }
            }

            ResetCategories();
        }
    }
}
