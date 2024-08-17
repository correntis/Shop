using MaterialDesignColors;
using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using Shop.MVVM.View.UserApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Xceed.Wpf.Toolkit;

namespace Shop.MVVM.ViewModel.AdminApp
{
    public class EditPhotosViewModel : ObservableObject
    {
        public event EventHandler Changed; 
        public event EventHandler Closed;

        public Product Product { get; set; }

        public RelayCommand ChoosePhotoCommand { get; set; }

        private byte[] selectedPhoto;
        public byte[] SelectedPhoto
        {
            get { return selectedPhoto; }
            set { selectedPhoto = value; OnPropertyChanged(nameof(SelectedPhoto)); }
        }

        private ObservableCollection<byte[]> images = [];
        public ObservableCollection<byte[]> Images
        {
            get { return images; }
            set { images = value; OnPropertyChanged(nameof(Images)); }
        }

        private bool isLoaded = false;
        public bool IsLoaded
        {
            get { return isLoaded; }
            set { isLoaded = value; OnPropertyChanged(nameof(IsLoaded)); }
        }

        public EditPhotosViewModel(Product product)
        {
            Product = product;

            LoadImages();

            ChoosePhotoCommand = new(ExecuteChoosePhoto);
        }

        private async void LoadImages()
        {
            var search = new GoogleSearch(Product);
            var images = await search.FindImagesAsync();

            if (images.Count == 0)
            { 
                OnClosed();
                MessageBox.Show("Не удалось найти фото.");
            }
            else
            {
                Images = [.. images];
            }
            IsLoaded = true;
        }

        private async void ExecuteChoosePhoto(object parameter)
        {
            if (parameter is byte[] image)
            {
                using ShopContext context = new();

                var User = await context.FindProductAsync(Product.Id);

                User.Image = image;

                context.SaveChanges();

                OnChanged();
            }
        }

        private void OnChanged()
        {
            OnClosed();
            Changed?.Invoke(this, null);
        }


        private void OnClosed()
        {
            Closed?.Invoke(this, null);
        }
    }
}
