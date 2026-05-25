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
using System.Collections.ObjectModel;
using DeliveryService.Models;
using DeliveryService.ViewModels;

namespace DeliveryService.Views
{
    /// <summary>
    /// Логика взаимодействия для MenuView.xaml
    /// </summary>
   
    public partial class MenuView : Window
    {

        public MenuView(MenuViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };
            ((Control)sender).RaiseEvent(eventArg);
            e.Handled = true;
        }
    }
}