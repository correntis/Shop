using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.Model
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string Login { get; set; } 
        public string Password { get; set; }
        public int UsersId { get; set; }

        public virtual User User { get; set; }
    }
}
