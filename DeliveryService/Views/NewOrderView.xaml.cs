using DeliveryService.Utils;
using DeliveryService.ViewModels;
using GMap.NET;
using GMap.NET.MapProviders;
using System;
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
using System.Windows.Shapes;

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
