using Shop.MVVM.Model;
using Shop.MVVM.ViewModel.AdminApp;
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

namespace Shop.AdminApp
{
    /// <summary>
    /// Логика взаимодействия для EditPhotosWindow.xaml
    /// </summary>
    public partial class EditPhotosWindow : Window
    {
        public EditPhotosWindow(Product product, EventHandler handler)
        {
            InitializeComponent();
            var vm = new EditPhotosViewModel(product);
            vm.Changed += handler;
            vm.Closed += CloseEvent;
            this.DataContext = vm;
        }

        private void Close_Click(object sender, RoutedEventArgs args)
        {
            Close();
        }

        private void CloseEvent(object sender, EventArgs args)
        {
            Close();
        }
    }
}
