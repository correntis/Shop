using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections;

namespace Shop.MVVM.Model
{
    // Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
    // var user = db.UsersAccounts.Include(ua => ua.User).ToList().FirstOrDefault(ua => BCrypt.Net.BCrypt.Verify(psw, ua.Password))?.User;

    public class User : ObservableObject, INotifyDataErrorInfo
    {
        public User()
        {
            
        }


        private string name = string.Empty;
        private string surname = string.Empty;
        private string phone = string.Empty;
        private string email = string.Empty;
        private byte[] avatar = [0];
        private ObservableCollection<UserProduct> userProducts = [];

        public int Id { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно быть длиннее 50 символов")]
        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна быть длиннее 50 символов")]
        public string Surname
        {
            get { return surname; }
            set
            {
                if (value != surname)
                {
                    surname = value;
                    OnPropertyChanged(nameof(Surname));
                }
            }
        }

        [Required(ErrorMessage = "Телефон обязателен")]
        [StringLength(50, ErrorMessage = "Телефон не должен быть длиннее 30 символов")]
        [RegularExpression("^\\+\\d{3}(29|44)\\d{7}$", ErrorMessage ="Неверный формат номера")]
        public string Phone
        {
            get { return phone; }
            set
            {
                if (value != phone)
                {
                    phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }

        [Required(ErrorMessage = "Email обязательнен")]
        [StringLength(50, ErrorMessage = "Email не должен быть длиннее 50 символов")]
        [EmailAddress(ErrorMessage ="Недопустимый Email")]
        public string Email
        {
            get { return email; }
            set
            {
                if (value != email)
                {
                    email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public byte[] Avatar
        {
            get { return avatar; }
            set
            {
                if (value != avatar)
                {
                    avatar = value;
                    OnPropertyChanged(nameof(Avatar));
                }
            }
        }

        private UserAccount userAccount;
        public virtual UserAccount UserAccount
        {
            get { return userAccount; }
            set { userAccount = value; OnPropertyChanged(nameof(UserAccount)); }
        }

        private DeliveryInfo deliveryInfo;
        public virtual DeliveryInfo DeliveryInfo
        {
            get { return deliveryInfo; }
            set { deliveryInfo = value; OnPropertyChanged(nameof(DeliveryInfo)); }
        }

        private ObservableCollection<Review> reviews = [];
        public virtual ObservableCollection<Review> Reviews
        {
            get { return reviews; }
            set { reviews = value; OnPropertyChanged(nameof(Reviews)); }
        }

        private ObservableCollection<Order> orders = [];
        public virtual ObservableCollection<Order> Orders
        {
            get { return orders; }
            set { orders = value; OnPropertyChanged(nameof(Orders)); }
        }

        public virtual ObservableCollection<UserProduct> UserProducts
        {
            get { return userProducts; }
            set
            {
                if(value != userProducts)
                {
                    userProducts = value;
                    OnPropertyChanged(nameof(UserProducts));
                }
            }
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

        public bool ValidateUser()
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

            foreach (var propertyName in typeof(User).GetProperties().Select(prop => prop.Name))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            return _errors.Count == 0;
        }

        public bool ShortValidateUser()
        {
            var name = ValidateUserProperty(nameof(Name));
            var surname = ValidateUserProperty(nameof(Surname));
            var phone = ValidateUserProperty(nameof(Phone));

            return name && surname && phone;
        }

        private bool ValidateUserProperty(string propertyName)
        {
            _errors.Remove(propertyName);

            var validationContext = new ValidationContext(this) { MemberName = propertyName };
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateProperty(GetType().GetProperty(propertyName).GetValue(this), validationContext, validationResults))
            {
                var errorMessages = validationResults.Select(vr => vr.ErrorMessage).ToList();
                _errors[propertyName] = errorMessages;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            return !_errors.ContainsKey(propertyName);
        }
    }
}
