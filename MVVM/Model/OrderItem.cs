using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.Model
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrdersId { get; set; }
        public int ProductsId { get; set; }
        public int Quantity { get; set; }

        public Product Product { get; set; }
    }
}
