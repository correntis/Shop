using Shop.MVVM.Model;
using Shop.MVVM.ViewModel.UserApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Shop.UserApp
{
    /// <summary>
    /// Логика взаимодействия для RequestWindow.xaml
    /// </summary>
    public partial class RequestWindow : Window
    {
        public event EventHandler Sended;

        public RequestWindow(Product product)
        {
            InitializeComponent();
            var vm = new RequestWindowViewModel(product);
            vm.CloseEvent += CloseEvent;
            vm.CloseEvent += OnSended;
            this.DataContext = vm;
        }

        public void CloseEvent(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs args)
        {
            Close();
        }

        private void OnSended(object sender, EventArgs args)
        {
            Sended?.Invoke(this, null);
        }
    }
}
