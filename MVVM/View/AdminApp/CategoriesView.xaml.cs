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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shop.MVVM.View.AdminApp
{
    /// <summary>
    /// Логика взаимодействия для CategoriesView.xaml
    /// </summary>
    public partial class CategoriesView : UserControl
    {
        public CategoriesView()
        {
            InitializeComponent();
        }

        private void OnNextClicked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as CategoriesViewModel)?.PagingCategories.MoveToNextPage();
        }

        private void OnPreviousClicked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as CategoriesViewModel)?.PagingCategories.MoveToPreviousPage();
        }
    }
}
