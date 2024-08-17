using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.ViewModel.UserApp
{
    public class RequestWindowViewModel : ObservableObject
    {
        public event EventHandler CloseEvent;

        public RelayCommand SendRequestCommand { get; set; }

        public Product Product { get; set; }
        public User User { get; set; }

        public RequestWindowViewModel(Product product) 
        {
            User = new User() 
            {
                Name = Env.User.Name,
                Surname = Env.User.Surname,
                Phone = Env.User.Phone,
                Email = Env.User.Email,
                Id = Env.User.Id,
            };

            Product = product;

            SendRequestCommand = new(ExecuteSendRequest);
        }

        public async void ExecuteSendRequest(object parameter)
        {
            if (!User.ValidateUser())
            {
                return;
            }

            using ShopContext context = new();

            var user = context.Users.FirstOrDefault(u => u.Id == User.Id);
            var product = context.Products.FirstOrDefault(p => p.Id == Product.Id);

            user.Name = new(User.Name); // эти строки
            user.Surname = new(User.Surname); // вызывают 
            user.Phone = new(User.Phone); // ошибки
            user.Email = new(User.Email); // 

            Env.User.Name = User.Name;
            Env.User.Surname = User.Surname;
            Env.User.Phone = User.Phone;
            Env.User.Email = User.Email;

            await context.SaveChangesAsync();

            var request = new Request()
            {
                Product = product,
                User = user,
                Status = 0,
                Date = DateTime.Now
            };

            context.Requests.Add(request);
            await context.SaveChangesAsync();

            OnClosing();
        }

        private void OnClosing()
        {
            CloseEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
