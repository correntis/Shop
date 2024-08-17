using Shop.Core;
using Shop.MVVM.Model;
using Shop.MVVM.ViewModel.UserApp;
using System.Windows;
using System.Windows.Media.Animation;

namespace Shop.UserApp
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public UserWindow()
        {
            InitializeComponent();
            var viewModel = new UserAppViewModel();
            viewModel.UserQuit += QuitEvent;
            this.DataContext = viewModel;
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
                this.Width = 1200;
                this.Height = 800;

                isMaximazed = false;
            }
            else
            {
                this.WindowState = WindowState.Maximized;

                isMaximazed = true;
            }
        }

        private void ScrollViewer_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
