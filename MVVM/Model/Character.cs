using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.Model
{
    public class Character : ObservableObject, INotifyDataErrorInfo
    {
        private string name;
        private ObservableCollection<Category> categories = new();
        private ObservableCollection<CharacterValue> characterValues = new();

        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(50, ErrorMessage = "Название должно быть меньше 50 символов")]
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ObservableCollection<Category> Categories
        {
            get { return categories; }
            set
            {
                if (value != categories)
                {
                    categories = value;
                    OnPropertyChanged(nameof(Categories));
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ObservableCollection<CharacterValue> CharacterValues
        {
            get { return characterValues; }
            set
            {
                if (value != characterValues)
                {
                    characterValues = value;
                    OnPropertyChanged(nameof(CharacterValues));
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

        public bool ValidateCharacter()
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
                                _errors[propertyName] = new List<string> { validationResult.ErrorMessage };
                            }
                        }
                    }
                }
            }

            foreach (var propertyName in typeof(Product).GetProperties().Select(prop => prop.Name))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            return _errors.Count == 0;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
