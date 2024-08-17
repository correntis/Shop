using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shop.MVVM.Model
{
    public class Review : ObservableObject
    {
        private string _value;
        private DateTime date;

        public int Id { get; set; }
        public int UsersId { get; set; }
        public int ProductsId { get; set; }
        public string Value
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

        public DateTime Date
        {
            get { return date; }
            set
            {
                if (value != date)
                {
                    date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}
