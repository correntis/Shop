using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.Model
{
    public class CharacterValue : ObservableObject
    {
        private ObservableCollection<Product> products;
        private Character character;
        private string _value;

        public int Id { get; set; }
        public int CharactersId { get; set; }

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
        public virtual Character Character
        {
            get { return character; }
            set
            {
                if (value != character)
                {
                    character = value;
                    OnPropertyChanged(nameof(Character));
                }
            }
        }
        public virtual string Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
