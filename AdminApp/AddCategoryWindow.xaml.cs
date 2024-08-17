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
    /// Логика взаимодействия для AddCategoryWindow.xaml
    /// </summary>
    public partial class AddCategoryWindow : Window
    {
        public EventHandler AddEventHandler { get; set; }

        public AddCategoryWindow(EventHandler eventHandler = null, Category category = null)
        {
            InitializeComponent();

            var vm = new AddCategoryViewModel(category);
            vm.ProductAddedEvent += Close_Event;
            vm.ProductAddedEvent += eventHandler;
            this.DataContext = vm;
        }

        private void Close_Click(object sender, RoutedEventArgs args)
        {
            Close();
        }

        private void Close_Event(object sender, EventArgs args)
        {
            Close_Click(this, null);
        }

        private bool isMaximazed = false;
        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (isMaximazed)
            {
                this.WindowState = WindowState.Normal;
                this.Width = 1080;
                this.Height = 800;

                isMaximazed = false;
            }
            else
            {
                this.WindowState = WindowState.Maximized;

                isMaximazed = true;
            }
        }
    }
}
