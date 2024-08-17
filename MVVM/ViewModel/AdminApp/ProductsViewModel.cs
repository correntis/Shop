using Microsoft.EntityFrameworkCore;
using Shop.AdminApp;
using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;

namespace Shop.MVVM.ViewModel.AdminApp
{
    public class ProductsViewModel : ObservableObject, IUpdatable
    {

        private PagingCollectionView pagingProducts;
        public PagingCollectionView PagingProducts
        {
            get { return pagingProducts; }
            set { pagingProducts = value; OnPropertyChanged(nameof(PagingProducts));}
        }


        public RelayCommand NextPageCommand { get; set; }
        public RelayCommand PrevPageCommand { get; set; }

        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand RemoveProductCommand { get; set; }
        public RelayCommand EditCharactersCommand { get; set; }

        public RelayCommand FilterCommand { get; set; }
        public RelayCommand ClearFilterCommand { get; set; }

        private int itemsOnPage = 15;
        public int ItemsOnPage
        {
            get { return itemsOnPage; }
            set { itemsOnPage = value;  _=ResetPaginProductsAsync(); OnPropertyChanged(nameof(ItemsOnPage)); }
        }
        public List<int> ItemsPerPageOptions { get; } = [1,15, 30, 50, 100];

        private string producerFilter = string.Empty;
        public string ProducerFilter
        {
            get { return producerFilter; }
            set { producerFilter = value; OnPropertyChanged(nameof(ProducerFilter)); }
        }

        private string nameFilter = string.Empty;
        public string NameFilter
        {
            get { return nameFilter; }
            set { nameFilter = value; OnPropertyChanged(nameof(NameFilter)); }
        }

        private string codeFilter = string.Empty;
        public string CodeFilter
        {
            get { return codeFilter; }
            set { codeFilter = value; OnPropertyChanged(nameof(CodeFilter)); }
        }


        public ProductsViewModel()
        {
            _ = ResetPaginProductsAsync();

            NextPageCommand = new(o =>
            {
                PagingProducts.MoveToNextPage();
            });

            PrevPageCommand = new(o =>
            { 
                PagingProducts.MoveToPreviousPage();
            });

            AddProductCommand = new(ExecuteAddProduct);
            RemoveProductCommand = new(ExecuteRemoveProduct);
            EditCharactersCommand = new(ExecuteEditCharacters);
            FilterCommand = new(ExecuteFilter);
            ClearFilterCommand = new(ExecuteClearFilter);
        }

        public async Task ResetPaginProductsAsync()
        {
            await Task.Run(() =>
            {
                using ShopContext context = new();

                var products = context.Products.ToList()
                    .Where(p =>
                    {
                        return p.Name.ToLower().Contains(NameFilter.ToLower()) &&
                        p.Producer.ToLower().Contains(ProducerFilter.ToLower()) &&
                        p.Code.ToLower().Contains(CodeFilter.ToLower()) &&
                        p.Status == (int)ProductStatus.Upload;
                    })
                    .OrderByDescending(p => p.Id).ToList();

                PagingProducts = new(products, ItemsOnPage);
            });
        }

        private void ExecuteAddProduct(object parameter)
        {
            var window = new AddProductWindow(AddEventHandler, (Product)parameter);
            window.Show();
        }

        private async void ExecuteRemoveProduct(object objItem)
        {
            if (objItem is Product product)
            {
                await Task.Run(async() =>
                {
                    using ShopContext context = new();

                    context.RemoveProduct(product);

                    await ResetPaginProductsAsync();
                });
            }
        }

        private void ExecuteClearFilter(object objItem)
        {
            NameFilter = string.Empty;
            ProducerFilter = string.Empty;
            CodeFilter = string.Empty;

            ExecuteFilter(null);
        }

        private void ExecuteEditCharacters(object parameter)
        {
            var window = new EditCharactersWindow(AddEventHandler, (Product)parameter);
            window.Show();
        }

        private async void ExecuteFilter(object parameter)
        {
            await ResetPaginProductsAsync();
        }

        public async void AddEventHandler(object sender, EventArgs args)
        {
            await ResetPaginProductsAsync();
        }

        public async Task Update()
        {
            await ResetPaginProductsAsync();
        }
    }
}
