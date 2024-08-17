using Shop.MVVM.Model;
using Shop.MVVM.ViewModel.AdminApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
using Xceed.Wpf.Toolkit;

namespace Shop.AdminApp
{
    /// <summary>
    /// Логика взаимодействия для EditCharactersWindow.xaml
    /// </summary>
    public partial class EditCharactersWindow : Window
    {
        public EventHandler AddEventHandler { get; set; }

        public EditCharactersWindow(EventHandler eventHandler = null, Product product = null)
        {
            InitializeComponent();

            var vm = new EditCharactersViewModel(product);
            vm.ProductChangedEvent += Close_Event;
            vm.ProductChangedEvent += eventHandler;
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


        private void NewChar_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is WatermarkTextBox textBox)
            {
                if (textBox.DataContext is Character character)
                {

                    var parameter = new Tuple<int, string>(character.Id, textBox.Text);

                    (this.DataContext as EditCharactersViewModel)?.AddNewCharacterCommand?.Execute(parameter);

                    textBox.Text = string.Empty;
                }
            }
        }
    }
}
