using Shop.MVVM.ViewModel.AdminApp;
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
    /// Логика взаимодействия для CategoriesCatalogView.xaml
    /// </summary>
    public partial class CategoriesCatalogView : UserControl
    {
        public CategoriesCatalogView()
        {
            InitializeComponent();
        }

        private void WatermarkTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var temporaryElement = new FrameworkElement();
                temporaryElement.Focus();

                (sender as WatermarkTextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                (this.DataContext as CategoriesCatalogViewModel)?.FilterCommand?.Execute(this);
            }
        }
    }
}
