using Shop.Core;
using Shop.MVVM.Model;
using Shop.MVVM.View.AdminApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.ViewModel.AdminApp
{
    public class AdminAppViewModel : ObservableObject
    {
        public event EventHandler UserQuit;

        public RelayCommand ProductsCommand { get; set; }
        public RelayCommand CategoriesCommand { get; set; }
        public RelayCommand CharacersCommand { get; set; }
        public RelayCommand AccountsCommand { get; set; }
        public RelayCommand RequestsCommand { get; set; }
        public RelayCommand DataProcessingComand { get; set; }

        public RelayCommand QuitCommand { get; set; }

        public ProductsViewModel ProductsVM { get; set; }
        public CategoriesViewModel CategoriesVM { get; set; }
        public CharactersViewModel CharactersVM { get; set; }
        public AccountsViewModel AccountsVM { get; set; }
        public RequestsViewModel RequestsVM{ get; set; }
        public DataProcessingViewModel DataProcessingVM{ get; set; }

        public Admin CurrentAdmin { get; set; }

        private object currentView;
        public object CurrentView
        {
            get { return currentView; }
            set { currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        public AdminAppViewModel()
        {
            CurrentAdmin = Env.Admin;

            ProductsVM = new();
            CategoriesVM = new();
            CharactersVM = new();
            AccountsVM = new();
            RequestsVM = new();
            DataProcessingVM = new();

            CurrentView = ProductsVM;

            ProductsCommand = new RelayCommand(o =>
            {
                _ = ProductsVM.Update();
                CurrentView = ProductsVM;
            });

            CategoriesCommand = new RelayCommand(o =>
            {
                _ = CategoriesVM.Update();
                CurrentView = CategoriesVM;
            });

            CharacersCommand = new RelayCommand(o =>
            {
                _ = CharactersVM.Update();
                CurrentView = CharactersVM;
            });

            AccountsCommand = new RelayCommand(o =>
            {
                _ = AccountsVM.Update();
                CurrentView = AccountsVM;
            });

            RequestsCommand = new RelayCommand(o =>
            {
                _ = RequestsVM.Update();
                CurrentView = RequestsVM;
            });

            DataProcessingComand = new(o =>
            {
                _ = DataProcessingVM.Update();
                CurrentView = DataProcessingVM;
            });

            QuitCommand = new(ExecuteQuit);
        }

        private void ExecuteQuit(object parameter)
        {
            UserQuit?.Invoke(this, null);
        }
    }
}
