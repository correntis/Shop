using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using Shop.MVVM.ViewModel.AdminApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Shop.AdminApp
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
          
            var vm = new AdminAppViewModel();
            vm.UserQuit += QuitEvent;
            this.DataContext = vm;
        }

        private void Close_Click(object sender, RoutedEventArgs args)
        {
            Close();
        }

        private void QuitEvent(object sender, EventArgs args)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            for (int i = 0; i < Application.Current.Windows.Count - 1; i++)
            {
                Application.Current.Windows[i].Close();
            }
        }

        private bool isOpen = true;
        private void ToggleMenuVisibility(object sender, RoutedEventArgs e)
        {
            if (isOpen)
            {
                DoubleAnimation animation = new(50, TimeSpan.FromSeconds(0.25))
                {
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
                };

                animation.Completed += (s, _) =>
                {
                    OpenMenuState.Visibility = Visibility.Collapsed;
                    CloseMenuState.Visibility = Visibility.Visible;
                };
                SideMenu.BeginAnimation(WidthProperty, animation);
            }
            else
            {
                DoubleAnimation animation = new(200, TimeSpan.FromSeconds(0.25))
                {
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                animation.Completed += (s, _) =>
                {
                    OpenMenuState.Visibility = Visibility.Visible;
                    CloseMenuState.Visibility = Visibility.Collapsed;
                };
                SideMenu.BeginAnimation(WidthProperty, animation);
            }

            isOpen = !isOpen;
        }

        private bool isMaximazed = false;
        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (isMaximazed)
            {
                this.WindowState = WindowState.Normal;
                this.Width = 1080;
                this.Height = 720;

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
