using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.Model
{
    public enum RequestStatus
    {
        Unread,
        Processing,
        Completed,
        All
    }

    public class Request : ObservableObject
    {
        public int Id { get; set; }
        public int UsersId { get; set; }
        public int ProductsId { get; set; }
        public DateTime Date { get; set; }

        private int status;
        public int Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}
