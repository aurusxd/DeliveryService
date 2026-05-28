using DeliveryService.Models;
using DeliveryService.Utils;
using DeliveryService.ViewModels;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeliveryService.Views
{
    /// <summary>
    /// Interaction logic for dispatcherPage.xaml
    /// </summary>
    public partial class dispatcherPage : UserControl
    {
        /// <summary>
        /// Специальный токен для защиты от дубликации симуляций маршрутов
        /// </summary>
        private CancellationTokenSource? _simulationCts;
        public dispatcherPage()
        {
            InitializeComponent();
            MapInitializer.CoordinatesRoute += RouteSimulate;

        }
        /// <summary>
        /// Симуляция маршрута
        /// </summary>
        /// <param name="points">Список списков координат маршрута</param>
        /// <returns></returns>
        public async Task RouteSimulate(List<List<double>> points)
        {
            if (DataContext is DispatcherViewModel vm)
            {
                _simulationCts?.Cancel();
                _simulationCts?.Dispose();

                _simulationCts = new CancellationTokenSource();
                var token = _simulationCts.Token;
                foreach (var point in points)
                {
                    if (token.IsCancellationRequested) return;

                    await Map.CoreWebView2.ExecuteScriptAsync(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "MoveCourier({0}, {1})",
                            point[0],
                            point[1]));
                    await vm.SaveCoords(point[0], point[1]);
                    await Task.Delay(300, token);
                }
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await MapInitializer.Initialize(Map);


            if (DataContext is DispatcherViewModel vm)
            {
                vm.OrderSelected -= OnOrderSelected;
                vm.CourierSelected -= OnCourierSelected;

                vm.OrderSelected += OnOrderSelected;
                vm.CourierSelected += OnCourierSelected;
            }
        }

        private async void OnOrderSelected(Order order)
        {
            System.Diagnostics.Debug.WriteLine($"OnOrderSelected вызван");
            await Map.CoreWebView2.ExecuteScriptAsync(

            string.Format(CultureInfo.InvariantCulture,
                    "DrawRoute({0}, {1}, {2}, {3}, false)",
                    order.Lat_From, order.Lon_From, order.Lat_To, order.Lon_To));
        }

        private async void OnCourierSelected(double latFrom, double lonFrom,double latTo, double lonTo,double courLat,double courLon)
        {
            System.Diagnostics.Debug.WriteLine($"OnCourierSelected вызван: {latFrom}, {lonFrom}");
            await Map.CoreWebView2.ExecuteScriptAsync(
                string.Format(CultureInfo.InvariantCulture,
                    "DrawRoute({0}, {1}, {2}, {3}, true)",
                    latFrom, lonFrom, latTo, lonTo));

            await Map.CoreWebView2.ExecuteScriptAsync(
            string.Format(CultureInfo.InvariantCulture,
                "AddMark({0}, {1})",
                courLat, courLon));
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DispatcherViewModel vm)
            {
                MapInitializer.Reset();
                vm.OrderSelected -= OnOrderSelected;
                vm.CourierSelected -= OnCourierSelected;
                MapInitializer.CoordinatesRoute -= RouteSimulate;


            }
        }
    }
}
