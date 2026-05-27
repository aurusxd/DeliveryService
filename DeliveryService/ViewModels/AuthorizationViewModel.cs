using DeliveryService.Commands;
using DeliveryService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DeliveryService.ViewModels
{
    public class AuthorizationViewModel : BaseViewModel
    {

        private object _currentView;

        private readonly EntranceViewModel _entranceViewModel;
        private readonly RegistrationViewModel _registrationViewModel;
        public object CurrentView
        {
            get => _currentView;
            set{ SetProperty(ref _currentView, value);}
        }


        public ICommand LoginCommand { get; }
        public ICommand RegCommand { get; }
        public AuthorizationViewModel(
            EntranceViewModel EntViewModel,
            RegistrationViewModel RegistrationViewModel
            )
        {
            _entranceViewModel = EntViewModel;
            _registrationViewModel = RegistrationViewModel;

            CurrentView = _entranceViewModel;

            LoginCommand = new RelayCommand(() => CurrentView = _entranceViewModel);
            RegCommand = new RelayCommand(() => CurrentView = _registrationViewModel);

        }
    }
}

