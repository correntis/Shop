using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.IO;

namespace Shop.MVVM.Model
{
    public enum ProductStatus
    {
        Upload,
        Processing
    }

    public class Product : ObservableObject, INotifyDataErrorInfo
    {
        public Product()
        {

        }
        public Product(Product other)
        {
            Name = other.Name;
            Brand = other.Brand;
            Code = other.Code;
            Description = other.Description;
            Price = other.Price;
            Producer = other.Producer;
            Country = other.Country;
            Quantity = other.Quantity;
            Image = other.Image;

            Categories = new(other.Categories);
        }


        private string name = string.Empty;
        private string brand = string.Empty;
        private string code = string.Empty;
        private string description = string.Empty;
        private decimal price = 0;
        private string producer = string.Empty;
        private string country = string.Empty;
        private int quantity = 0;
        private byte[]? image = [];
        private int status = 0;

        private ObservableCollection<Category> categories = [];
        private ObservableCollection<CharacterValue> characterValues = [];

        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(200, ErrorMessage = "Название должно быть короче 200 символов")]
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

        [Required(ErrorMessage = "Бренд обязателен")]
        [StringLength(50, ErrorMessage = "Бренд должен быть короче 50 символов")]
        public string Brand
        {
            get { return brand; }
            set
            {
                if (value != brand)
                {
                    brand = value;
                    OnPropertyChanged(nameof(Brand));
                }
            }
        }

        [Required(ErrorMessage = "Артикул обязателен")]
        [StringLength(50, ErrorMessage = "Артикул должен быть короче 50 символов")]
        public string Code
        {
            get { return code; }
            set
            {
                if (value != code)
                {
                    code = value;
                    OnPropertyChanged(nameof(Code));
                }
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                if (value != description)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.1, 1000000, ErrorMessage = "Цена должна быть от 0.1 до 1000000")]
        public decimal Price
        {
            get { return price; }
            set
            {
                if (value != price)
                {
                    price = value;
                    OnPropertyChanged(nameof(Price));
                }
            }
        }

        [Required(ErrorMessage = "Поставщик обязателен")]
        [StringLength(50, ErrorMessage = "Поставщик должен быть короче 50 символов")]
        public string Producer
        {
            get { return producer; }
            set
            {
                if (value != producer)
                {
                    producer = value;
                    OnPropertyChanged(nameof(Producer));
                }
            }
        }

        [StringLength(50, ErrorMessage = "Страна должна быть короче 50 символов")]
        public string Country
        {
            get { return country; }
            set
            {
                if (value != country)
                {
                    country = value;
                    OnPropertyChanged(nameof(Country));
                }
            }
        }

        [Range(0, 1000, ErrorMessage = "Количество должно быть от 0 до 1000")]
        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (value != quantity)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        public byte[]? Image
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

        public int Status
        {
            get { return status; }
            set
            {
                if (value != status)
                {
                    status = value;
                    OnPropertyChanged(nameof(Status));
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
        public virtual ObservableCollection<Review> Reviews { get; set; } = [];

        public virtual ObservableCollection<AdminProduct> AdminProducts { get; set; } = [];
        public virtual ObservableCollection<UserProduct> UserProducts { get; set; } = [];
        public virtual ObservableCollection<OrderItem> OrderItems { get; set; } = [];
        public virtual ObservableCollection<Request> Requests { get; set; } = [];

        public void Copy(Product other)
        {
            Name = other.Name;
            Brand = other.Brand;
            Code = other.Code;
            Description = other.Description;
            Price = other.Price;
            Producer = other.Producer;
            Country = other.Country;
            Quantity = other.Quantity;
            Image = other.Image;

            Categories = new(other.Categories);
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

        public bool ValidateProduct()
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


        public static Product GetDefault(ProductStatus status)
        {
            return new Product
            {
                Name = string.Empty,
                Brand = string.Empty,
                Code = string.Empty,
                Country = string.Empty,
                Producer = string.Empty,
                Price = 0,
                Quantity = 0,
                Image = File.ReadAllBytes("C:\\Users\\Максим\\Desktop\\КП\\Shop\\Images\\default_product.jpg"),
                Status = (int)status
            };
        }
    }
}
