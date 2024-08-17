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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

namespace Shop.MVVM.View.UserApp
{
    /// <summary>
    /// Логика взаимодействия для ProductsCatalogView.xaml
    /// </summary>
    public partial class ProductsCatalogView : UserControl
    {
        public ProductsCatalogView()
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

                (this.DataContext as ProductsCatalogViewModel)?.FilterCommand?.Execute(this);
            }
        }

        private void CheckListBox_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            var vm = (DataContext as ProductsCatalogViewModel);

            if (vm is not null)
            {
                if (e.IsSelected)
                {
                    vm.SelectedValues.Add((CharacterValue)e.Item);   
                }
                else
                {
                    vm.SelectedValues.Remove((CharacterValue)e.Item);
                }
            }
        }

        private void RangeSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var vm = (DataContext as ProductsCatalogViewModel);

            vm?.FilterCommand?.Execute(this);
        }

     
    }
}
