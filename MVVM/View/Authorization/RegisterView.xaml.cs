﻿using System;
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

using Shop.MVVM.ViewModel.Authorization;
using Xceed.Wpf.Toolkit;

namespace Shop.MVVM.View.Authorization
{
    /// <summary>
    /// Логика взаимодействия для RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WatermarkPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var dataContext = this.DataContext as RegisterViewModel;

            if (dataContext is not null)
            {
                dataContext.Password = ((WatermarkPasswordBox)sender).Password;
            }
        }
    }
}
