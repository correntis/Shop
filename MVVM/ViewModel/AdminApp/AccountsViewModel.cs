using Microsoft.EntityFrameworkCore;
using Shop.DB;
using Shop.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.ViewModel.AdminApp
{
    public class AccountsViewModel : ObservableObject, IUpdatable
    {
        private ObservableCollection<Admin> admins;
        public ObservableCollection<Admin> Admins
        {
            get { return admins; }
            set { admins = value; OnPropertyChanged(nameof(Admins)); }
        }

        public RelayCommand AddCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }

        private object currentView;
        public object CurrentView
        {
            get { return currentView; }
            set { currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        public AccountsViewModel()
        {
            _ = ResetAdminsAsync();
            AddCommand = new(ExecuteAdd);
            RemoveCommand = new(ExecuteRemove);
        }

        private async Task ResetAdminsAsync()
        {
            await Task.Run(() =>
            {
                using ShopContext context = new();

                Admins = new(context.Admins.ToList().OrderByDescending(a => a.Id));
            });
        }

        private void ExecuteAdd(object parameter)
        {
            var vm = new AddAdminViewModel();
            vm.AdminAdded += CloseViewHandler;
            CurrentView = vm;
        }

        private async void ExecuteRemove(object parameter)
        {
            if (parameter is Admin admin)
            {
                using ShopContext context = new();

                var adminToDelete = context.Admins.Include(a => a.AdminAccount).FirstOrDefault(a => a.Id == admin.Id);

                context.Admins.Remove(adminToDelete);

                context.SaveChanges();

                await ResetAdminsAsync();
            }
        }

        private async void CloseViewHandler(object sender, EventArgs args)
        {
            await ResetAdminsAsync();
            CurrentView = null;
        }

        public async Task Update()
        {
            await ResetAdminsAsync();
        }
    }
}
