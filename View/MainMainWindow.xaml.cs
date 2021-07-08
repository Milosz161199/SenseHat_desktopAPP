using LedDisplayExample.Model;
using System.Windows;


namespace LedDisplayExample.View
{
    /// <summary>
    /// Logika interakcji dla klasy MainMainWindow.xaml
    /// </summary>
    public partial class MainMainWindow : Window
    {
        bool isMenuVisible = true;
        bool justOneTime = true;
        private ConfigParams config = new ConfigParams();
        public MainMainWindow()
        {
            InitializeComponent();
            if (justOneTime) {
                config.ReadConfigDataFromFile();
                justOneTime = false;
            }
            Loaded += MyWindow_Loaded;
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new GraphView());
        }

        private void Btn_chart_Click(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new GraphView());
        }

        private void Btn_led_Click(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new MainWindow());
        }

        private void Btn_config_Click(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new ConfigView());
        }

        private void Btn_thp_Click(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new GraphTHPView());
        }
        private void Btn_joystick_Click(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new JoystickView());
        }
        private void Btn_list_Click(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(new ListView());
        }
        private void MenuBtn_Click(object sender, RoutedEventArgs e)
        {
            isMenuVisible = !isMenuVisible;

            if (isMenuVisible)
                this.Menu.Visibility = Visibility.Visible;
            else
                this.Menu.Visibility = Visibility.Collapsed;
        }

        private void btn_thp_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
