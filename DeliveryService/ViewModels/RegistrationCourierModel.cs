using DeliveryService.Commands;
using DeliveryService.Models;
using DeliveryService.Services;
using System.Windows;
using System.Windows.Input;

namespace DeliveryService.ViewModels
{
    /// <summary>
    /// Логика взаимодействия пользователя и базы данных с RegistrationCourier
    /// </summary>
    public class RegistrationCourierModel : BaseViewModel
    {
        private readonly CourierService _courierService;

        /// <summary>
        /// Имя курьера
        /// </summary>
        private string _courierName;
        /// <summary>
        /// Номер курьера
        /// </summary>
        private string _courierPhone;
        /// <summary>
        /// Номер курьера, "очищенный" от всего, кроме цифр
        /// </summary>
        private string _cleanedPhoneNumber;
        /// <summary>
        /// Тип транспортного средства
        /// </summary>
        private string _vehicleType;

        /// <summary>
        /// Имя курьера
        /// </summary>
        public string CourierName
        {
            get => _courierName;
            set => SetProperty(ref _courierName, value);
        }
        /// <summary>
        /// Номер курьера
        /// </summary>
        public string CourierPhone
        {
            get => _courierPhone;
            set => SetProperty(ref _courierPhone, value);
        }
        /// <summary>
        /// Тип транспортного средства
        /// </summary>
        public string VehicleType
        {
            get => _vehicleType;
            set => SetProperty(ref _vehicleType, value);
        }

        /// <summary>
        /// Команда создания и сохранения курьера 
        /// </summary>
        public ICommand SaveCommand { get; }
        /// <summary>
        /// Команда закрытия окна
        /// </summary>
        public ICommand CloseCommand { get; }


        public RegistrationCourierModel(CourierService courierService)
        {
            _courierService = courierService;

            SaveCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(SaveCourierAsync, "Ошибка сохранения курьера"),
                canExecute: () => !IsBusy
            );

            CloseCommand = new RelayCommand(_ => CloseWindow(false));
        }


        /// <summary>
        /// Проверка валидации CourierName и VehicleType
        /// </summary>
        /// <returns>true, если все поля валидны, иначе false</returns>
        private bool ValidateProperty()
        {
            if (string.IsNullOrWhiteSpace(CourierName))
            {
                ErrorMessage = "Введите имя клиента";
                return false;
            }
            if (string.IsNullOrWhiteSpace(VehicleType))
            {
                ErrorMessage = "Укажите тип транспорта";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверка валидации CourierPhone и "очишение" от не-цифр
        /// </summary>
        /// <returns>true, если валиден, иначе false</returns>
        private bool ValidatePhoneNumber()
        {
            if (string.IsNullOrWhiteSpace(CourierPhone))
            {
                ErrorMessage = "Введите номер телефона";
                _cleanedPhoneNumber = null;
                return false;
            }

            string cleaned = new string(CourierPhone.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(cleaned))
            {
                ErrorMessage = "Номер телефона должен содержать хотя бы одну цифру";
                _cleanedPhoneNumber = null;
                return false;
            }
            if (cleaned.Length < 10 || cleaned.Length > 11)
            {
                ErrorMessage = "Номер телефона должен содержать 10–11 цифр";
                _cleanedPhoneNumber = null;
                return false;
            }

            _cleanedPhoneNumber = cleaned;
            return true;
        }

        /// <summary>
        /// Создание курьера и сохранение в базу данных
        /// </summary>
        private async Task SaveCourierAsync()
        {
            ErrorMessage = null;

            if (!ValidateProperty())
                return;

            #region На данный момент этот регион работает с ошибками
            if (!ValidatePhoneNumber())
                return;

            if (!long.TryParse(_cleanedPhoneNumber, out long phoneNumber))
            {
                ErrorMessage = "Номер телефона должен содержать только цифры";
                return;
            }
            #endregion


            Courier courier = new Courier
            {
                Name = CourierName,
                CourierPhone = phoneNumber.ToString(),
                Vehicle_Type = VehicleType
            };

            bool success = await _courierService.AddCourierAsync(courier);

            if (success)
                CloseWindow(true);
            else
            {
                ErrorMessage = "Не удалось выполнить команду";
                return;
            }
        }

        /// <summary>
        /// Закрытие окна
        /// </summary>
        /// <param name="result">Результат работы окна</param>
        private void CloseWindow(bool result)
        {
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            if (window != null)
            {
                window.DialogResult = result;
                window.Close();
            }
        }
    }
}