using DeliveryService.Utils;
using DeliveryService.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for OrderAcceptView.xaml
    /// </summary>
    public partial class OrderAcceptView : Window
    {
        public OrderAcceptView(OrderAcceptViewModel vm)    
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MapInitializer.Reset();
            await MapInitializer.Initialize(Map);

        }
    }
}
