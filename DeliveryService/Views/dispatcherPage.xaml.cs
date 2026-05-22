using DeliveryService.Utils;
using DeliveryService.ViewModels;
using System;
using System.Collections.Generic;
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
        public dispatcherPage()
        {
            InitializeComponent();
            MapInitializer.CoordinatesRoute += RouteSimulate;
            Loaded += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine(
                    $"dispatcherPage загружен, DataContext: {DataContext?.GetType().Name}");
            };
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

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await MapInitializer.Initialize(Map);


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

                vm.CourierSelected += async (Lat_From, Lan_From, Lat_To, Lan_To) =>
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
