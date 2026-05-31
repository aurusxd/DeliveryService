using DeliveryService.Models;
using DeliveryService.Services;
using DeliveryService.Utils;
using DeliveryService.ViewModels;
using GalaSoft.MvvmLight.Helpers;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
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
        private SimulationService _simulationService;
        public dispatcherPage()
        {
            InitializeComponent();

        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await MapInitializer.Initialize(Map);


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
                    Lon));
        }

        private async Task OnRouteReceived(List<List<double>> points)
        {
            if (DataContext is DispatcherViewModel vm)
                await _simulationService.StartAsync(points, vm.SelectedCourier);
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
                Map.Dispose();


            }
        }
    }
}
