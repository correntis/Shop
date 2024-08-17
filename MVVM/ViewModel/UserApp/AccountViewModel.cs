using Microsoft.EntityFrameworkCore;
using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using Shop.MVVM.ViewModel.AdminApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace Shop.MVVM.ViewModel.UserApp
{
    public class AccountViewModel: ObservableObject, IUpdatable
    {
        public string Title { get; set; } = "Личный кабинет";

        private bool showChangePassword = false;
        public bool ShowChangePassword
        {
            get { return showChangePassword; }
            set { showChangePassword = value; OnPropertyChanged(nameof(ShowChangePassword)); }
        }

        private string oldPassword;
        public string OldPassword
        {
            get { return oldPassword; }
            set { oldPassword = value; OnPropertyChanged(nameof(OldPassword)); }
        }
        private string newPassword;
        public string NewPassword
        {
            get { return newPassword; }
            set { newPassword = value; OnPropertyChanged(nameof(NewPassword)); }
        }
        private bool errorPassword = false;
        public bool ErrorPassword
        {
            get { return errorPassword; }
            set { errorPassword = value; OnPropertyChanged(nameof(ErrorPassword)); }
        }

        public ShopContext Context { get; set; }

        private User user;
        public User User
        {
            get { return user; }
            set { user = value; OnPropertyChanged(nameof(User)); }
        }
        public RelayCommand ChangePhotoCommand { get; set; }
        public RelayCommand SaveInfoCommand { get; set; }
        public RelayCommand SavePasswordCommand { get; set; }
        public RelayCommand ShowPasswordCommand { get; set; }


        public ObservableCollection<object> TabControls { get; set; } = [];


        public AccountViewModel(ObservableCollection<object> tabControls)
        {
            Context = new ShopContext();
            User = Env.User;
            TabControls = tabControls;

            _ = ResetAsync();

            ChangePhotoCommand = new(ExecuteChangePhoto);
            SaveInfoCommand = new(ExecuteSaveInfo);
            ShowPasswordCommand = new(ExecuteShowPassword);
            SavePasswordCommand = new(ExecuteSavePassword);
        }

        private void ExecuteSavePassword(object parameter)
        {
            ValidatePasswords();

            if (ErrorPassword)
            {
                return;
            }

            User.UserAccount.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);

            _ = Context.SaveChangesAsync();

            OldPassword = string.Empty;
            NewPassword = string.Empty;

            ExecuteShowPassword(null);

        }

        private async void ExecuteSaveInfo(object parameter)
        {
            if (User.ValidateUser() && User.DeliveryInfo.Validate())
            {   
                await Context.SaveChangesAsync();

                Env.User = User;

                foreach (var tab in TabControls)
                {
                    if (tab is ProductCardViewModel productCard)
                    {
                        await productCard.UpdateUser();
                    } else if (tab is UserProductsViewModel userProducts)
                    {
                        await userProducts.Update();
                    }
                }
            }
        }

        private void ExecuteShowPassword(object parameter)
        {
            ShowChangePassword = !ShowChangePassword;
        }

        private void ExecuteChangePhoto(object parameter)
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
                    User.Avatar = File.ReadAllBytes(selectedImagePath);
                }
                else
                {
                    MessageBox.Show("Неверный формат.");
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

        private void ValidatePasswords()
        {
            if (string.IsNullOrEmpty(OldPassword) || string.IsNullOrEmpty(NewPassword))
            {
                ErrorPassword = true;
            } 
            else if (!BCrypt.Net.BCrypt.Verify(OldPassword, User.UserAccount.Password))
            {
                ErrorPassword = true;
            }
        }

        public async Task ResetAsync()
        {
            Context = new();
            User = await Context.FindUserToAccountAsync(Env.User.Id);
            User.Orders = new(User.Orders.OrderByDescending(o => o.Id));
        }

        public async Task Update()
        {
            await ResetAsync();
        }
    }
}
