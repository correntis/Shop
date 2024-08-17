using Microsoft.EntityFrameworkCore;
using Shop.DB;
using Shop.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace Shop.MVVM.ViewModel.AdminApp
{
    public class AddProductViewModel : ObservableObject
    {
        public event EventHandler ProductAddedEvent;

        private ObservableCollection<Category> categories = [];
        public ObservableCollection<Category> Categories
        {
            get { return categories; }
            set { categories = value; OnPropertyChanged(nameof(Categories)); }
        }

        private ObservableCollection<Category> selectedCategories = [];
        public ObservableCollection<Category> SelectedCategories
        {
            get { return selectedCategories; }
            set { selectedCategories = value; OnPropertyChanged(nameof(SelectedCategories)); }
        }


        private Product current;
        public Product Current
        {
            get { return current; }
            set { current = value; OnPropertyChanged(nameof(Current)); }
        }

        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand ChooseFileCommand { get; set; }

        public ShopContext Context { get; set; }

        private string headerText { get; set; }
        private string buttonText { get; set; }
        public string HeaderText
        {
            get { return headerText; }
            set { headerText = value; OnPropertyChanged(nameof(HeaderText)); }
        }
        public string ButtonText
        {
            get { return buttonText; }
            set { buttonText = value; OnPropertyChanged(nameof(ButtonText)); }
        }
        public bool IsNew { get; set; }

        public AddProductViewModel(Product product)
        {
            IsNew = product is null;
            Context = new ShopContext();

            _ = InitializePropertiesAsync(product);

            AddProductCommand = new(ExecuteAddProduct);
            ChooseFileCommand = new(ExecuteChooseFile);
        }

        private async Task InitializePropertiesAsync(Product product)
        {
            Categories = new([.. await Context.GetCategoriesAsync()]);

            if (IsNew)
            {
                SelectedCategories = [];
                HeaderText = "Добавление товара";
                ButtonText = "Добавить товар";
                Current = new Product
                {
                    Name = "test",
                    Brand = "test",
                    Code = "test",
                    Country = "test",
                    Producer = "test",
                    Description = "test",
                    Price = 15.5134m,
                    Quantity = 1,
                    Image = File.ReadAllBytes("C:\\Users\\Максим\\Desktop\\КП\\Shop\\Images\\default_product.jpg"),
                    Status = (int)ProductStatus.Upload,
                };
            } 
            else
            {
                Current = await Context.FindProductAsync(product.Id);
                HeaderText = "Редактирование товара";
                ButtonText = "Изменить товар";
                SelectedCategories = Current.Categories;
            }
        }

        private void ExecuteAddProduct(object parameter)
        {
            if (!Current.ValidateProduct())
            {
                return;
            }

            if (IsNew)
            {
                Add();
            }

            Context.SaveChanges();
            Context.Dispose();
            OnProductAdded();
        }

        private void Add()
        {
            Context.Add(Current);

            foreach (var category in SelectedCategories)
            {
                var dbCategory = Context.Categories.FirstOrDefault(c => c.Id == category.Id);
                if (dbCategory is not null)
                {
                    Current.Categories.Add(dbCategory);
                }
            }
        }

        private void ExecuteChooseFile(object parameter)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|Все файлы (*.*)|*.*",
                InitialDirectory = "D:\\уник\\четвертый сем\\ООП\\lab4-5\\Shop\\images\\items"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;

                if (IsImageFile(selectedImagePath))
                {
                    Current.Image = File.ReadAllBytes(selectedImagePath);
                } 
                else
                {
                    MessageBox.Show("Неверный формат");
                }
            }
        }

        private bool IsImageFile(string filePath)
        {
            try
            {
                using var image = Image.FromFile(filePath);
                return image.RawFormat.Equals(ImageFormat.Jpeg) ||
                       image.RawFormat.Equals(ImageFormat.Png) ||
                       image.RawFormat.Equals(ImageFormat.Bmp);
            }
            catch (OutOfMemoryException) 
            {
                return false;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected virtual void OnProductAdded()
        {
            ProductAddedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
