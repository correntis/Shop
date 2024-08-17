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
    public class AddCategoryViewModel : ObservableObject
    {
        public event EventHandler ProductAddedEvent;

        public ObservableCollection<Character> Characters { get; set; }
        public ObservableCollection<Character> SelectedCharacters{ get; set; }
        public Category Current { get; set; }

        public RelayCommand AddCategoryCommand { get; set; }
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

        public AddCategoryViewModel(Category category)
        {
            IsNew = category is null;
            Context = new ShopContext();

            InitializeProperties(category);

            AddCategoryCommand = new(ExecuteAddCategory);
            ChooseFileCommand = new(ExecuteChooseFile);
        }

        private void InitializeProperties(Category category)
        {
            Characters = new([.. Context.Characters.Include(x => x.Categories)]);

            if (IsNew)
            {
                SelectedCharacters = [];
                HeaderText = "Добавление категории";
                ButtonText = "Добавить категорию";
                Current = new Category
                {
                    Name = "test category",
                    ShowOnMain = false,
                    Image = File.ReadAllBytes("C:\\Users\\Максим\\Desktop\\КП\\Shop\\Images\\default_product.jpg")
                };
            }
            else
            {
                HeaderText = "Редактирование категории";
                ButtonText = "Изменить категорию";
                Current = Context.Categories.FirstOrDefault(c => c.Id == category.Id);
                SelectedCharacters = Current.Characters;
            }
        }

        private void ExecuteAddCategory(object parameter)
        {
            if (!Current.ValidateCategory())
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

            foreach (var character in SelectedCharacters)
            {
                var dbCharacter= Context.Characters.FirstOrDefault(c => c.Id == character.Id);
                if (dbCharacter is not null)
                {
                    Current.Characters.Add(dbCharacter);
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
