using BCrypt.Net;
using Shop.AdminApp;
using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using Shop.UserApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Shop.MVVM.ViewModel.UserApp;
using Shop.MVVM.ViewModel.AdminApp;

namespace Shop.MVVM.ViewModel.Authorization
{
    public class LoginViewModel : ObservableObject, INotifyDataErrorInfo
    {
        private string login;
        private string password;

        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(100, ErrorMessage = "Не более 100 символов")]
        //[EmailAddress]
        public string Login
        {
            get { return login; }
            set
            {
                if (value != login)
                {
                    login = value;
                    OnPropertyChanged(nameof(Login));
                }
            }
        }

        [Required(ErrorMessage = "Обязательное поле")]
        public string Password
        {
            get { return password; }
            set
            {
                if (value != password)
                {
                    password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }


        public event EventHandler RequestClose;

        public RelayCommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            Login = string.Empty;
            Password = string.Empty;

            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        public async void ExecuteLogin(object parameter)
        {
            if (!ValidateLoginViewModel())
            {
                return;
            }

            var user = await Core.Authorization.FindUserAsync(Login);
            if (user is not null)
            {
                if (BCrypt.Net.BCrypt.Verify(Password, user.UserAccount.Password))
                {
                    OpenUserWindow(user);
                    return;
                }
                else
                {
                    SetError(nameof(Password), "Неверный пароль");
                    return;
                }
            }

            var admin = await Core.Authorization.FindAdminAsync(Login);
            if (admin is not null)
            {
                if (BCrypt.Net.BCrypt.Verify(Password, admin.AdminAccount.Password))
                {
                    OpenAdminWindow(admin);
                    return;
                } 
                else
                {
                    SetError(nameof(Password), "Неверный пароль");
                    return;
                }
            }

            if (user is null || admin is null)
            {
                SetError(nameof(Login), "Неверный логин");
            }

        }

        private void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private void OpenUserWindow(User user)
        {
            Env.User = user;

            var userWindow = new UserWindow();

            userWindow.Show();

            OnRequestClose();
        }

        private void OpenAdminWindow(Admin admin)
        {
            Env.Admin = admin;

            var adminWindow = new AdminWindow();

            adminWindow.Show();

            OnRequestClose();
        }





        // VALIDATION
        private readonly Dictionary<string, List<string>> _errors = [];
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public bool HasErrors => _errors.Count != 0;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.TryGetValue(propertyName, out List<string>? value))
                return Enumerable.Empty<string>();

            return value;
        }

        public void SetError(string propertyName, string message)
        {
            if (_errors.TryGetValue(propertyName, out var errorList))
            {
                errorList.Add(message);
            }
            else
            {
                _errors[propertyName] = [message];
            }

            OnPropertyChanged(propertyName);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public bool ValidateLoginViewModel()
        {
            _errors.Clear();

            var validationContext = new ValidationContext(this);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(this, validationContext, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    if (validationResult.MemberNames != null)
                    {
                        foreach (var propertyName in validationResult.MemberNames)
                        {
                            if (_errors.TryGetValue(propertyName, out var errorList))
                            {
                                errorList.Add(validationResult.ErrorMessage);
                            }
                            else
                            {
                                _errors[propertyName] = [validationResult.ErrorMessage];
                            }
                        }
                    }
                }
            }

            foreach (var propertyName in typeof(LoginViewModel).GetProperties().Select(prop => prop.Name))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            return _errors.Count == 0;
        }
    }
}
