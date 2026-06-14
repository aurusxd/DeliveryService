using DeliveryService.Models;
using DeliveryService.Services;
using DeliveryService.Utils;
using DeliveryService.ViewModels;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;


namespace DeliveryService.Views
{
    /// <summary>
    /// Interaction logic for dispatcherPage.xaml
    /// </summary>
    public partial class dispatcherPage : UserControl
    {
        private SimulationService _simulationService;
        public dispatcherPage()
        {
            InitializeComponent();

        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await MapInitializer.Initialize(Map).ConfigureAwait(false);


            if (DataContext is DispatcherViewModel vm)
            {
                _simulationService = vm.SimulationService;
                vm.OrderSelected -= OnOrderSelected;
                vm.CourierSelected -= OnCourierSelected;

                vm.OrderSelected += OnOrderSelected;
                vm.CourierSelected += OnCourierSelected;

                _simulationService.CourierMoved -= OnCourierMoved;
                _simulationService.CourierMoved += OnCourierMoved;
                MapInitializer.CoordinatesRoute += OnRouteReceived;

            }
        }

        private async void OnCourierMoved(double Lat, double Lon)
        {

            await Map.CoreWebView2.ExecuteScriptAsync(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "MoveCourier({0}, {1})",
                    Lat,
                    Lon)).ConfigureAwait(false);
        }

        private async Task OnRouteReceived(List<List<double>> points)
        {
            if (DataContext is DispatcherViewModel vm)
                await _simulationService.StartAsync(points, vm.SelectedCourier).ConfigureAwait(false);
        }

        private async void OnOrderSelected(Order order)
        {
            System.Diagnostics.Debug.WriteLine($"OnOrderSelected вызван");
            await Map.CoreWebView2.ExecuteScriptAsync(

            string.Format(CultureInfo.InvariantCulture,
                    "DrawRoute({0}, {1}, {2}, {3}, false)",
                    order.Lat_From, order.Lon_From, order.Lat_To, order.Lon_To)).ConfigureAwait(false);
        }

        private async void OnCourierSelected(double latFrom, double lonFrom, double latTo, double lonTo, double courLat, double courLon)
        {
            await Map.CoreWebView2.ExecuteScriptAsync(
                string.Format(CultureInfo.InvariantCulture,
                    "DrawRoute({0}, {1}, {2}, {3}, true)",
                    latFrom, lonFrom, latTo, lonTo)).ConfigureAwait(false);

            await Map.CoreWebView2.ExecuteScriptAsync(
            string.Format(CultureInfo.InvariantCulture,
                "AddMark({0}, {1})",
                courLat, courLon)).ConfigureAwait(false);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DispatcherViewModel vm)
            {
                MapInitializer.Reset(Map);
                vm.OrderSelected -= OnOrderSelected;
                vm.CourierSelected -= OnCourierSelected;
                MapInitializer.CoordinatesRoute -= OnRouteReceived;
                _simulationService.CourierMoved -= OnCourierMoved;
                Map.Dispose();


            }
        }
    }
}
