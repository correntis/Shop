using Microsoft.EntityFrameworkCore;
using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using Shop.MVVM.View.UserApp;
using Shop.MVVM.ViewModel.AdminApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Shop.MVVM.ViewModel.UserApp
{
    public class UserProductsViewModel : ObservableObject, IUpdatable
    {
        public event EventHandler OrderConfirmed;

        public string Title { get; set; } = "Корзина";

        private User user;
        public User User
        {
            get { return user; }
            set { user = value; OnPropertyChanged(nameof(User)); }
        }
        public ShopContext Context { get; set; }

        public RelayCommand ChangeFormVisibilityCommand { get; set; }
        public RelayCommand ConfirmOrderCommand { get; set; }
        public RelayCommand RemoveProductCommand { get; set; }

        private string buttonContent = "Заполнить форму";
        public string ButtonContent
        {
            get { return buttonContent; }
            set { buttonContent = value; OnPropertyChanged(nameof(ButtonContent)); }
        }
        private bool isFormShow = false;
        public bool IsFormShow
        {
            get { return isFormShow; }
            set 
            {
                if (value)
                {
                    ButtonContent = "Скрыть";
                }
                else
                {
                    ButtonContent = "Заполнить форму";
                }

                isFormShow = value;
                OnPropertyChanged(nameof(IsFormShow)); 
            }   
        }

        private bool showErrorMessage = false;
        public bool ShowErrorMessage
        {
            get { return showErrorMessage; }
            set { showErrorMessage = value; OnPropertyChanged(nameof(ShowErrorMessage)); }
        }

        private bool showSuccessMessage = false;
        public bool ShowSuccessMessage
        {
            get { return showSuccessMessage; }
            set { showSuccessMessage = value; OnPropertyChanged(nameof(ShowSuccessMessage)); }
        }


        private double price = 0;
        public double Price
        {
            get { return price; }
            set { price = value; OnPropertyChanged(nameof(Price)); }
        }

        public UserProductsViewModel()
        {
            Context = new();

            User = Env.User;

            _ = InitializeUserAsync();

            ChangeFormVisibilityCommand = new(ExecuteChangeFormVisibility);
            ConfirmOrderCommand = new(ExecuteConfirmOrder);
            RemoveProductCommand = new(ExecuteRemoveProduct);
        }

        private void ExecuteRemoveProduct(object parameter)
        {
            if (parameter is UserProduct product)
            {
                User.UserProducts.Remove(product);
                Context.SaveChangesAsync();
            }
        }

        private void ExecuteChangeFormVisibility(object parameter)
        {
            IsFormShow = !IsFormShow;
        }

        private async void ExecuteConfirmOrder(object parameter)
        {
            await ConfirmOrder();
        }

        
        public async Task ConfirmOrder()
        {
            User.DeliveryInfo.SkipRequired = false;

            var userValidation = User.ShortValidateUser();
            var infoValidation = User.DeliveryInfo.Validate();

            if (!userValidation && !infoValidation)
            {
                return;
            }
            
            ExecuteChangeFormVisibility(null);

            ObservableCollection<UserProduct> userProduct = new([.. User.UserProducts.Where(up => up.Product.Quantity > 0)]);
            ObservableCollection<OrderItem> orderItems = new([..
            userProduct
                .Select(up => new OrderItem(){
                        Product = up.Product,
                        Quantity = up.Quantity
                })
            ]);

            if (userProduct.Count == 0)
            {
                await ShowError();
                return;
            }

            var price = orderItems.Sum(oi => oi.Product.Price * oi.Quantity);

            var order = new Order
            {
                Price = price,
                Status = 0,
                OrderItems = orderItems,
                Date = DateTime.Now,
                User = User
            };

            Context.Orders.Add(order);


            foreach (var product in userProduct)
            {
                product.Product.Quantity -= product.Quantity;
                User.UserProducts.Remove(product);
            }

            await Context.SaveChangesAsync();
            Price = 0;
            ShowSuccess();
            OnOrderConfirmed(userProduct);
        }

        private async Task ShowError()
        {
            ShowErrorMessage = true;
            await Task.Delay(1000);
            ShowErrorMessage = false;
        }

        private async Task ShowSuccess()
        {
            ShowSuccessMessage = true;
            await Task.Delay(1000);
            ShowSuccessMessage = false;
        }


        private void OnOrderConfirmed(object newProducts)
        {
            OrderConfirmed?.Invoke(newProducts, EventArgs.Empty);
        }

        private async Task InitializeUserAsync()
        {
            User = await Context.FindUserToUserProductsAsync(User.Id);
            Price = (double)User.UserProducts.Where(up => up.Product.Quantity > 0).Sum(up => up.Product.Price * up.Quantity);
        }

        public async Task ResetAsync()
        {
            Context = new();
            User = await Context.FindUserToUserProductsAsync(User.Id);
            Price = (double)User.UserProducts.Where(up => up.Product.Quantity > 0).Sum(up => up.Product.Price * up.Quantity);
        }

        public async Task Update()
        {
            await ResetAsync();
        }
    }
}
