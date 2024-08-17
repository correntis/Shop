using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.Model
{
    public class AdminProduct
    {
        public int Id { get; set; }
        public int AdminsId { get; set; }
        public int ProductsId { get; set; }
        public DateTime Date { get; set; }
        

        public Admin Admin{ get; set; }
        public Product Product { get; set; }
    }
}
