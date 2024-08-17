using Shop.MVVM.ViewModel.Authorization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace Shop.MVVM.View.UserApp
{
    /// <summary>
    /// Логика взаимодействия для AccountView.xaml
    /// </summary>
    public partial class AccountView : UserControl
    {
        public AccountView()
        {
            InitializeComponent();
        }

        private void OldPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var dataContext = this.DataContext as AccountViewModel;

            if (dataContext is not null)
            {
                dataContext.OldPassword = ((WatermarkPasswordBox)sender).Password;
            }
        }

        private void NewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var dataContext = this.DataContext as AccountViewModel;

            if (dataContext is not null)
            {
                dataContext.NewPassword = ((WatermarkPasswordBox)sender).Password;
            }
        }
    }
}
