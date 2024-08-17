using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Shop.MVVM.View.UserApp;

namespace Shop.MVVM.Model
{
    public class Category : ObservableObject, INotifyDataErrorInfo
    {
        public Category() { }
        public Category(int id, string name, ObservableCollection<Product> products)
        {
            Id = id;
            Name = name;
            Products = products;
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(50, ErrorMessage = "Название должно быть короче 200 символов")]
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

        public bool ShowOnMain
        {
            get { return showOnMain; }
            set
            {
                if (value != showOnMain)
                {
                    showOnMain = value;
                    OnPropertyChanged(nameof(ShowOnMain));
                }
            }
        }
        public byte[] Image
        {
            get { return image; }
            set
            {
                if (value != image)
                {
                    image = value;
                    OnPropertyChanged(nameof(Image));
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ObservableCollection<Product> Products
        {
            get { return products; }
            set
            {
                if (value != products)
                {
                    products = value;
                    OnPropertyChanged(nameof(Products));
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ObservableCollection<Character> Characters
        {
            get { return characters; }
            set
            {
                if (value != characters)
                {
                    characters = value;
                    OnPropertyChanged(nameof(Characters));
                }
            }
        }


        private string name;
        private bool showOnMain;
        private byte[] image;
        private ObservableCollection<Product> products = [];
        private ObservableCollection<Character> characters = [];

        public override string ToString()
        {
            return Name;
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

        public bool ValidateCategory()
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

            foreach (var propertyName in typeof(Category).GetProperties().Select(prop => prop.Name))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            return _errors.Count == 0;
        }

        public object Clone()
        {
            var category = new Category()
            {
                Name = this.Name,
                ShowOnMain = this.ShowOnMain,
                Image = [.. this.Image],
                Id = this.Id,
                Products = new([.. Products]),
                Characters = new([.. Characters])
            };

            foreach (var character in category.Characters)
            {
                character.CharacterValues = new([.. character.CharacterValues]);
            }

            return category;
        }
    }
}
