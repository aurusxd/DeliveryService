using DeliveryService.Utils;
using DeliveryService.ViewModels;
using System.Windows;


namespace DeliveryService.Views
{
    /// <summary>
    /// Interaction logic for NewOrderView.xaml
    /// </summary>
    public partial class NewOrderView : Window
    {
        public NewOrderView(NewOrderViewModel viewModel)
        {
            InitializeComponent();
            MapInitializer.Initialize(Map);
            DataContext = viewModel;
            MapInitializer.AddressSelected += OnAddressSelected;
        }

        private void OnAddressSelected(double lat, double lon, string address)
        {
            if (DataContext is NewOrderViewModel vm)
            {
                vm.SetSelectedAddress(lat, lon, address);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            MapInitializer.AddressSelected -= OnAddressSelected;
            base.OnClosed(e);
            Map.Dispose();
        }
    }
}
