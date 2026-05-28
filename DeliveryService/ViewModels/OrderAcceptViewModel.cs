using DeliveryService.Commands;
using DeliveryService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DeliveryService.ViewModels
{
    public class OrderAcceptViewModel : BaseViewModel
    {
        private readonly WindowsService _windowService;
        public ICommand ReturnCommand { get; set; }
        public OrderAcceptViewModel(WindowsService windowService)
        {
            _windowService = windowService;
            ReturnCommand = new RelayCommand(
                execute: () => TryRunTaskAsync(ReturnToMenu, "Ошибка возврата в меню"),
                canExecute: () => !IsBusy);
        }

        private async Task ReturnToMenu()
        {
            _windowService.CloseWindows();
            _windowService.OpenMenu();
        }
    }
}
