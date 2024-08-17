using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.Model
{
    public enum OrderStatus
    {
        Processing,
        Delivery,
        Compeleted
    }

    public class Order : ObservableObject
    {
        public int Id { get; set; }
        public int UsersId { get; set; }
        public DateTime Date { get; set; }
        public int Status { get; set; }
        public decimal Price { get; set; }

        public virtual User User { get; set; }
        public virtual ObservableCollection<OrderItem> OrderItems { get; set; }
    }
}
