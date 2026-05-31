using DeliveryService.ViewModels;
using System.Windows;
using System.Windows.Media;

namespace DeliveryService.Views
{

    public partial class EntranceView : Window
    {
        public EntranceView(AuthorizationViewModel vw)
        {
            InitializeComponent();
            DataContext = vw;
        }

        private SolidColorBrush ColorFromHex(string hex)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            Login.Background = Brushes.Gray;
            Reg.Background = ColorFromHex("#2563EB");

        }

        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            Reg.Background = Brushes.Gray;
            Login.Background = ColorFromHex("#2563EB");

        }
    }
}