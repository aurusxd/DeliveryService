using DeliveryService.ViewModels;
using System.Windows;


namespace DeliveryService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

        }

    }
}