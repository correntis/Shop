using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Shop.MVVM.ViewModel.Authorization
{
    public class PasswordHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordHelper),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPasswordProperty, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is Xceed.Wpf.Toolkit.WatermarkPasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= HandlePasswordChanged;
                if (!(bool)passwordBox.GetValue(UpdatingPasswordProperty))
                {
                    passwordBox.Password = (string)e.NewValue;
                }
                passwordBox.PasswordChanged += HandlePasswordChanged;
            }
        }

        private static readonly DependencyProperty UpdatingPasswordProperty =
            DependencyProperty.RegisterAttached(
                "UpdatingPassword",
                typeof(bool),
                typeof(PasswordHelper),
                new PropertyMetadata(false));

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as Xceed.Wpf.Toolkit.WatermarkPasswordBox;
            passwordBox.SetValue(UpdatingPasswordProperty, true);
            SetBoundPassword(passwordBox, passwordBox.Password);
            passwordBox.SetValue(UpdatingPasswordProperty, false);
        }
    }
}
