using Shop.Core;
using Shop.MVVM.Model;
using Shop.MVVM.View.UserApp;
using Shop.MVVM.ViewModel.AdminApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Shop.MVVM.ViewModel.UserApp
{
    public class UserAppViewModel : ObservableObject
    {
        public event EventHandler UserQuit;

        public RelayCommand QuitCommand { get; set; }
        public RelayCommand CategoriesCatalogCommand { get; set; }
        public RelayCommand AccountCommand { get; set; }
        public RelayCommand UserProductsCommand { get; set; }

        public RelayCommand ResetAppCommand { get; set; }


        public CategoriesCatalogViewModel CategoriesVM { get; set; }
        public AccountViewModel AccountVM { get; set; }
        public UserProductsViewModel UserProductsVM { get; set; }


        public ObservableCollection<object> TabViewModels { get; set; } = [];

        public RelayCommand CloseTabCommand { get; set; }

        private int selectedTab = 0;
        public int SelectedTab
        {
            get { return selectedTab; }
            set
            {
                selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

        public UserAppViewModel()
        {
            TabViewModels.CollectionChanged += (sender, e) =>
            {
                SelectedTab = e.NewStartingIndex;
            };

            CategoriesVM = new(TabViewModels);
            AccountVM = new(TabViewModels);
            UserProductsVM = new();

            UserProductsVM.OrderConfirmed += ResetTabs;

            TabViewModels.Add(AccountVM);

            CategoriesCatalogCommand = new(ExecuteCategoriesCatalog);
            AccountCommand = new(ExecuteAccount);
            UserProductsCommand = new(ExecuteUserProducts);
            QuitCommand = new(ExecuteQuit);
            CloseTabCommand = new(ExecuteCloseTab);
            ResetAppCommand = new(ExecuteResetApp);
        }

        private void ExecuteResetApp(object parameter)
        {
            TabViewModels.Clear();
            CategoriesVM = new(TabViewModels);
            AccountVM = new(TabViewModels);
            UserProductsVM = new();

            UserProductsVM.OrderConfirmed += ResetTabs;

            TabViewModels.Add(AccountVM);
        }

        private void ExecuteUserProducts(object parameter)
        {
            var index = TabViewModels.IndexOf(UserProductsVM);
            if (index == -1)
            {
                TabViewModels.Add(UserProductsVM);
            }
            else
            {
                SelectedTab = index;
            }
            //_ = UserProductsVM.Update();
        }

        private void ExecuteAccount(object parameter)
        {
            var index = TabViewModels.IndexOf(AccountVM);
            if (index == -1)
            {
                TabViewModels.Add(AccountVM);
            }
            else
            {
                SelectedTab = index;
            }
        }

        private void ExecuteCloseTab(object parameter)
        {
            TabViewModels.Remove(parameter);
        }

        private void ExecuteCategoriesCatalog(object parameter)
        {
            var index = TabViewModels.IndexOf(CategoriesVM);
            if (index == -1)
            {
                TabViewModels.Add(CategoriesVM);
            }
            else
            {
                SelectedTab = index;
            }
        }

        private void ExecuteQuit(object parameter)
        {
            UserQuit?.Invoke(this, null);
        }

        private async void ResetTabs(object newProducts, EventArgs e)
        {
            await AccountVM.ResetAsync();
            CategoriesVM.ResetProducts(newProducts);
        }
    }
}
