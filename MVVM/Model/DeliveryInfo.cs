using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.MVVM.Model
{
    public class DeliveryInfo : ObservableObject, INotifyDataErrorInfo
    {
        private string city;
        private string street;
        private string house;
        private string apart;
        private string floor;
        private string intercomCode;

        public int Id { get; set; }
        public int UsersId { get; set; }

        [Required(ErrorMessage = "Обязательно поле")]
        [StringLength(50, ErrorMessage = "Не более 50 символов.")]
        public string City 
        {
            get { return city; }
            set
            {
                if (value != city)
                {
                    city = value;
                    OnPropertyChanged(nameof(City));
                }
            }
        }

        [Required(ErrorMessage = "Обязательно поле")]
        [StringLength(100, ErrorMessage = "Не более 100 символов.")]
        public string Street
        {
            get { return street; }
            set
            {
                if (value != street)
                {
                    street = value;
                    OnPropertyChanged(nameof(Street));
                }
            }
        }

        [Required(ErrorMessage = "Обязательно поле")]
        [StringLength(50, ErrorMessage = "*")]
        [RegularExpression(@"^\d+$", ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "*")]
        public string House
        {
            get { return house; }
            set
            {
                if (value != house)
                {
                    house = value;
                    OnPropertyChanged(nameof(House));
                }
            }
        }

        [Required(ErrorMessage = "Обязательно поле")]
        [StringLength(50, ErrorMessage = "*")]
        [RegularExpression(@"^\d+$", ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "*")]
        public string Apart
        {
            get { return apart; }
            set
            {
                if (value != apart)
                {
                    apart = value;
                    OnPropertyChanged(nameof(Apart));
                }
            }
        }

        [Required(ErrorMessage = "Обязательно поле")]
        [StringLength(50, ErrorMessage = "*")]
        [RegularExpression(@"^\d+$", ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "*")]
        public string Floor
        {
            get { return floor; }
            set
            {
                if (value != floor)
                {
                    floor = value;
                    OnPropertyChanged(nameof(Floor));
                }
            }
        }

        [StringLength(50, ErrorMessage = "Не более 50 символов.")]
        public string IntercomCode
        {
            get { return intercomCode; }
            set
            {
                if (value != intercomCode)
                {
                    intercomCode = value;
                    OnPropertyChanged(nameof(IntercomCode));
                }
            }
        }

        [NotMapped]
        public bool SkipRequired { get; set; } = true;

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

        public bool Validate()
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
                            if (SkipRequired && validationResult.ErrorMessage.Contains("Обязательно поле"))
                            {
                                continue;
                            }

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

            foreach (var propertyName in typeof(DeliveryInfo).GetProperties().Select(prop => prop.Name)) 
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            return _errors.Count == 0;
        }

    }
}
