using Shop.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.ViewModel.Authorization
{
    public class AuthViewModel : ObservableObject
    {
        public event EventHandler RequestClose;

        public RelayCommand LoginCommand { get; set; }
        public RelayCommand RegisterCommand { get; set; }

        public LoginViewModel LoginVM { get; set; }
        public RegisterViewModel RegisterVM { get; set; }

        private object currentView;
        public object CurrentView
        {
            get { return currentView; }
            set
            {
                if (currentView != value)
                {
                    currentView = value;
                    OnPropertyChanged(nameof(CurrentView));
                }
            }
        }

        public AuthViewModel()
        {
            LoginVM = new();
            RegisterVM = new();

            LoginVM.RequestClose += OnRequestClose;
            RegisterVM.Registered += OnRegister;

            CurrentView = LoginVM;

            new Thread(DbWarmup).Start();

            LoginCommand = new RelayCommand((o) =>
            {
                CurrentView = LoginVM;
            });

            RegisterCommand = new RelayCommand((o) =>
            {
                CurrentView = RegisterVM;
            });
        }

        private void OnRegister(object sender, EventArgs e)
        {
            LoginCommand?.Execute(null);
        }

        private void OnRequestClose(object sender, EventArgs e)
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private void DbWarmup()
        {
            using ShopContext db = new();
            _ = db.Users.ToList().First();
        }
    }
}
