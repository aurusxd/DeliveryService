using DeliveryService.ViewModels;
using DeliveryService.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            Closing += (s, e) => viewModel.CloseWindowsCommand.Execute(null); 
        }

    }
}