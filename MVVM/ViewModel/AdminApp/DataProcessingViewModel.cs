using Shop.AdminApp;
using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using Shop.MVVM.View.UserApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.ViewModel.AdminApp
{
    public class DataProcessingViewModel : ObservableObject, IUpdatable
    {
        public event EventHandler Upload;

        private ObservableCollection<object> selectedProducts = [];
        public ObservableCollection<object> SelectedProducts
        {
            get { return selectedProducts; }
            set {  selectedProducts = value; OnPropertyChanged(nameof(SelectedProducts)); }
        }

        private PagingCollectionView pagingProducts;
        public PagingCollectionView PagingProducts
        {
            get { return pagingProducts; }
            set { pagingProducts = value; OnPropertyChanged(nameof(PagingProducts)); }
        }


        public RelayCommand NextPageCommand { get; set; }
        public RelayCommand PrevPageCommand { get; set; }

        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand RemoveProductCommand { get; set; }
        public RelayCommand EditCharactersCommand { get; set; }
        public RelayCommand OpenParseEditCommand { get; set; }
        public RelayCommand CloseParseEditCommand { get; set; }
        public RelayCommand ParseCommand { get; set; }
        public RelayCommand UploadProductCommand{ get; set; }
        public RelayCommand FindImagesCommand{ get; set; }

        public RelayCommand FilterCommand { get; set; }
        public RelayCommand ClearFilterCommand { get; set; }
        

        private int itemsOnPage = 15;
        public int ItemsOnPage
        {
            get { return itemsOnPage; }
            set { itemsOnPage = value; ResetPaginProductsAsync(); OnPropertyChanged(nameof(ItemsOnPage)); }
        }
        public List<int> ItemsPerPageOptions { get; } = [1, 15, 30, 50, 100];

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

        private bool showParseEdit = false;
        public bool ShowParseEdit
        {
            get { return showParseEdit; }
            set { showParseEdit = value; OnPropertyChanged(nameof(ShowParseEdit)); }
        }


        public ObservableCollection<ExcelMapping> Mapping { get; set; }


        public DataProcessingViewModel()
        {
            Mapping = [..ExcelMapping.GetDefaultMapping()];

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
            OpenParseEditCommand = new(ExecuteOpenParseEdit);
            CloseParseEditCommand = new(ExecuteCloseParseEdit);
            ParseCommand = new(ExecuteParse);
            UploadProductCommand = new(ExecuteUpload);
            FindImagesCommand = new(ExecuteFindImages);
        }

        private async void ExecuteFindImages(object parameter)
        {
            if (parameter is Product product)
            {
                var window = new EditPhotosWindow(product, OnPhotoEdit);
                window.Show();
            }
        }

        private async void ExecuteUpload(object parameter)
        {
            if (parameter is Product product)
            {
                using ShopContext context = new();

                var productDb = await context.FindProductAsync(product.Id);

                productDb.Status = (int)ProductStatus.Upload;
                
                await context.SaveChangesAsync();

                await ResetPaginProductsAsync();

                OnUpload();
            }
        }

        private async void ExecuteParse(object parameter)
        {
            ShowParseEdit = false;

            for (int i = 0; i < Mapping.Count; i++)
            {
                if (string.IsNullOrEmpty(Mapping[i].ExcelLabel))
                {
                    Mapping.Remove(Mapping[i]);
                    i--;
                }
            }

            var loadingWindow = new LoadingWindow();
            loadingWindow.Show();

            var path = ChooseFile();
            var products = await ExcelParser.ParseAsync(path, [.. Mapping]);

            using ShopContext context = new();
            context.Products.AddRange(products);

            await context.SaveChangesAsync();
            await ResetPaginProductsAsync();

            loadingWindow.Close();
        }

        private void ExecuteOpenParseEdit(object parameter)
        {
            ShowParseEdit = true;
        }

        private void ExecuteCloseParseEdit(object parameter)
        {
            ShowParseEdit = false;
        }


        private async Task ResetPaginProductsAsync()
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
                        p.Status == (int)ProductStatus.Processing;
                    })
                    .OrderBy(p => p.Id).ToList();

                PagingProducts = new(products, ItemsOnPage);
            });
        }

        private static string ChooseFile()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                Filter = "Документы Excel (*.xlsx)|*.xlsx",
                InitialDirectory = "C:\\Users\\Максим\\Desktop\\КП\\Shop\\EXCEL"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;

        }

        private void ExecuteAddProduct(object parameter)
        {
            var window = new AddProductWindow(ChangeEventHandler, (Product)parameter);
            window.Show();
        }

        private async void ExecuteRemoveProduct(object objItem)
        {
            if (objItem is Product product)
            {
                using ShopContext context = new();

                context.RemoveProduct(product);

               await ResetPaginProductsAsync();
            }
        }

        private async void ExecuteClearFilter(object objItem)
        {
            NameFilter = string.Empty;
            ProducerFilter = string.Empty;
            CodeFilter = string.Empty;

            await ResetPaginProductsAsync();
        }

        private void ExecuteEditCharacters(object parameter)
        {
            var window = new EditCharactersWindow(ChangeEventHandler, (Product)parameter);
            window.Show();
        }

        private async void ExecuteFilter(object parameter)
        {
            await ResetPaginProductsAsync();
        }

        public async void ChangeEventHandler(object sender, EventArgs args)
        {
            await ResetPaginProductsAsync();
        }

        public void OnPhotoEdit(object sender, EventArgs args)
        {
            ChangeEventHandler(this, null);
            OnUpload();
        }

        public void OnUpload()
        {
            Upload?.Invoke(this, EventArgs.Empty);
        }

        public async Task Update()
        {
            await ResetPaginProductsAsync();
        }
    }
}
