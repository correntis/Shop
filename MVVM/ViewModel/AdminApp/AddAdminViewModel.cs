using Shop.DB;
using Shop.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.ViewModel.AdminApp
{
    public class AddAdminViewModel : ObservableObject
    {
        public event EventHandler AdminAdded;

        public Admin Current { get; set; }

        public RelayCommand AddCommand { get; set; }
        public RelayCommand PassCommand { get; set; }

        public AddAdminViewModel()
        {
            Current = new();

            AddCommand = new(ExecuteAdd);
            PassCommand = new(o =>
            {
                OnAdminAdded();
            });
        }

        public void ExecuteAdd(object parameter)
        {
            using ShopContext context = new();

            Current.CreatedAt = DateTime.Now;
            Current.AdminAccount.Password = BCrypt.Net.BCrypt.HashPassword(Current.AdminAccount.Password);

            context.Admins.Add(Current);
            context.SaveChanges();

            OnAdminAdded();
        }

        public void OnAdminAdded()
        {
            AdminAdded?.Invoke(this, null);
        }
    }
}
