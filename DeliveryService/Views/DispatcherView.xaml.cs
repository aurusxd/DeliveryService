using DeliveryService.DTO;
using DeliveryService.Models;
using DeliveryService.Utils;
using DeliveryService.ViewModels;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;


namespace DeliveryService.Views
{
    /// <summary>
    /// Логика взаимодействия для DispatcherView.xaml
    /// </summary>
    public partial class DispatcherView : Window
    {

        public DispatcherView()
        {
            
        }
        public DispatcherView(DispatcherViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            MapInitializer.CoordinatesRoute += RouteSimulate;
        }

        public async Task RouteSimulate(List<List<double>> points)
        {
            foreach (var point in points)
            {

                await Map.CoreWebView2.ExecuteScriptAsync(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "MoveCourier({0}, {1})",
                        point[0],
                        point[1]));

                await Task.Delay(300);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await MapInitializer.Initialize(Map);
            Map.CoreWebView2.OpenDevToolsWindow();
  
            if (DataContext is DispatcherViewModel vm)
            {
                vm.OrderSelected += async order =>
                {

                    await Map.CoreWebView2.ExecuteScriptAsync(
                        "clearObjects()"
                        );

                    await Map.CoreWebView2.ExecuteScriptAsync(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "DrawRoute({0}, {1}, {2}, {3})",
                            order.Lat_From, order.Lon_From, order.Lat_To, order.Lon_To));
                };

                vm.CourierSelected += async (Lat_From,Lan_From,Lat_To,Lan_To) =>
                {
                    await Map.CoreWebView2.ExecuteScriptAsync(
                        "clearObjects()"
                        );

                    await Map.CoreWebView2.ExecuteScriptAsync(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "DrawRoute({0}, {1}, {2}, {3})",
                            Lat_From, Lan_From, Lat_To, Lan_To));

                    await Map.CoreWebView2.ExecuteScriptAsync(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "AddMark({0}, {1})",
                        Lat_From, Lan_From));
                };
            }
        }


    }
}
