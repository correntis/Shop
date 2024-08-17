using Microsoft.Win32;
using Shop.DB;
using Shop.MVVM.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Prism;
using Prism.Mvvm;
using System.Runtime.CompilerServices;
using System.Collections;
using Shop.MVVM;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Controls;
using Shop.MVVM.ViewModel.AdminApp;

namespace Shop.AdminApp
{

    public partial class AddProductWindow : Window
    {
        public EventHandler AddEventHandler { get; set; }

        public AddProductWindow(EventHandler eventHandler = null, Product product = null)
        {
            InitializeComponent();

            var vm = new AddProductViewModel(product);
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

        private void QuantityTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text,e.Text.Length - 1))
            {
                e.Handled = true;   
            }
        }

        private void PriceTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string decimalSeparator = ".";

            if (!char.IsDigit(e.Text, e.Text.Length - 1) && e.Text != decimalSeparator)
            {
                e.Handled = true;
            }
            else
            {
                if (e.Text == decimalSeparator)
                {
                    if (sender is TextBox textBox && (textBox.Text.Contains(decimalSeparator) || textBox.Text.Length == 0))
                    {
                        e.Handled = true;
                    }
                }
            }
        }
    }
}
