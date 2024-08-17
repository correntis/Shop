using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.Model
{
    // Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
    // var user = db.UsersAccounts.Include(ua => ua.User).ToList().FirstOrDefault(ua => BCrypt.Net.BCrypt.Verify(psw, ua.Password))?.User;

    public enum AdminRole
    {
        Manager,
        CEO
    }

    public class Admin : ObservableObject
    {
        public int Id { get; set; }
        public int Role { get; set; }
        public DateTime CreatedAt { get; set; }


        private int added;
        public int Added
        {
            get { return added; }
            set { added = value; OnPropertyChanged(nameof(Added)); }
        }

        private int deleted;
        public int Deleted
        {
            get { return deleted; }
            set { deleted = value; OnPropertyChanged(nameof(Deleted)); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(nameof(Name)); }
        }

        public virtual AdminAccount AdminAccount { get; set; } = new();
        public virtual ObservableCollection<AdminProduct> AdminProducts { get; set; } = [];
    }
}
