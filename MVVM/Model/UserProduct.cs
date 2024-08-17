using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.Model
{
    public class UserProduct : ObservableObject
    {
        public int Id { get; set; }
        public int UsersId { get; set; }
        public int ProductsId { get; set; }

        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set
            {
                if(value != quantity)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        public User User { get; set; }
        public Product Product { get; set; }
    }
}
