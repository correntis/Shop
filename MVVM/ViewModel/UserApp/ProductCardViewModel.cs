using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using Shop.MVVM.ViewModel.AdminApp;
using Shop.UserApp;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Shop.MVVM.ViewModel.UserApp
{
    public class ProductCardViewModel : ObservableObject
    {
        public string Title { get; set; }

        public Product Current { get; set; }

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

        private User user;
        public User User
        {
            get { return user; }
            set { user = value; OnPropertyChanged(nameof(User)); }
        }

        private string reviewText = string.Empty;
        public string ReviewText
        {
            get { return reviewText; }
            set { reviewText = value;OnPropertyChanged(nameof(ReviewText)); }
        }

        private int quantity = 1;
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; OnPropertyChanged(nameof(Quantity)); }
        }

        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand AddReviewCommand { get; set; }
        public RelayCommand RemoveReviewCommand { get; set; }
        public RelayCommand CreateRequestCommand { get; set; }

        public ObservableCollection<object> TabControls { get; set; }


        public ProductCardViewModel(ObservableCollection<object> tabControls, Product product)
        {
            TabControls = tabControls;
            Title = product.Name;
            Current = product;
            User = Env.User;

            Current.Reviews = new(Current.Reviews.OrderBy(r => r.Id == User.Id).ThenByDescending(r => r.Date));

            AddReviewCommand = new(ExecuteAddReview);
            RemoveReviewCommand = new(ExecuteRemoveReview);
            CreateRequestCommand = new(ExecuteCreateRequest);
            AddProductCommand = new(ExecuteAddProduct);
        }
        private void ExecuteCreateRequest(object parameter)
        {
            if (Current.Quantity > 0)
            {
                return;
            }

            var window = new RequestWindow(Current);
            window.Sended += UpdateUserEventHandler;
            window.Show();
        }

        private async void ExecuteAddProduct(object parameter)
        {
            using ShopContext context = new();

            var user = context.Users.Include(up => up.UserProducts).FirstOrDefault(u => u.Id == User.Id);
            if (user.UserProducts.Any(up => up.ProductsId == Current.Id))
            {
                ShowErrorMessage = true;
                await Task.Delay(1000);
                ShowErrorMessage = false;

                return;
            }
            
            var product = context.Products.Include(p => p.UserProducts).FirstOrDefault(p => p.Id == Current.Id);
            var userProduct = new UserProduct()
            {
                User = user,
                Product = product,
                Quantity = Quantity
            };

            context.UsersProducts.Add(userProduct);
            context.SaveChanges();

            foreach (var tab in TabControls)
            {
                if (tab is IUpdatable updatable)
                {
                    await updatable.Update();
                }
            }

            ShowSuccessMessage = true;
            await Task.Delay(1000);
            ShowSuccessMessage = false;
        }

        private void ExecuteRemoveReview(object parameter)
        {
            if (parameter is Review review)
            {
                Current.Reviews.Remove(review);

                using var context = new ShopContext();

                var reviewToDelete = context.Reviews
                    .Include(r => r.User)
                    .Include(r => r.Product)
                    .FirstOrDefault(r => r.Id == review.Id);

                context.Reviews.Remove(reviewToDelete);

                context.SaveChangesAsync();
            }
        }

        private async void ExecuteAddReview(object parameter)
        {
            if (ReviewText.Length >= 100)
            {
                using var context = new ShopContext();

                var user = context.Users.Include(u => u.Reviews).FirstOrDefault(u => u.Id == User.Id);
                var product = context.Products.Include(p => p.Reviews).FirstOrDefault(p => p.Id == Current.Id);

                var reviewDb = new Review()
                {
                    Date = DateTime.Now,
                    Product = product,
                    User = user,
                    Value = ReviewText
                };

                context.Reviews.Add(reviewDb);

                _ = await context.SaveChangesAsync();


                var review = new Review()
                {
                    Id = reviewDb.Id,
                    Date = DateTime.Now,
                    Product = Current,
                    User = User,
                    Value = ReviewText,
                    UsersId = User.Id
                };

                Current.Reviews.Insert(0, review);

                ReviewText = string.Empty;
            }
        }

        public async Task UpdateUser()
        {
            await Task.Run(() =>
            {
                User = Env.User;

                foreach (var review in Current.Reviews)
                {
                    if (review.User.Id == User.Id)
                    {
                        review.User.Name = User.Name;
                        review.User.Surname = User.Surname;
                        review.User.Avatar = User.Avatar;
                        review.User.Email = User.Email;
                    }
                }
            });
        }

        private async void UpdateUserEventHandler(object sender, EventArgs e)
        {
            await UpdateUser();

            foreach (var tab in TabControls)
            {
                if (tab is IUpdatable updatable)
                {
                    await updatable.Update();
                }
            }
        }
    }
}
