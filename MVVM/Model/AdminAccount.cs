using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.Model
{
    public class AdminAccount
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int AdminsId { get; set; }

        public virtual Admin Admin{ get; set; }
    }
}
