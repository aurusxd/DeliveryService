using DeliveryService.ViewModels;
using System.Windows;

namespace DeliveryService.Views
{
    /// <summary>
    /// Логика взаимодействия для RegistrationCourier.xaml
    /// </summary>
    public partial class RegistrationCourier : Window
    {
        public RegistrationCourier(RegistrationCourierModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
