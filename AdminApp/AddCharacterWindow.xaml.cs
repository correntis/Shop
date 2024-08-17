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
    /// Логика взаимодействия для AddCharacterWindow.xaml
    /// </summary>
    public partial class AddCharacterWindow : Window
    {
        public EventHandler AddEventHandler { get; set; }

        public AddCharacterWindow(EventHandler eventHandler = null, Character character = null)
        {
            InitializeComponent();

            var vm = new AddCharacterViewModel(character);
            vm.CharacterAddedEvent += Close_Event;
            vm.CharacterAddedEvent += eventHandler;
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
    }
}
