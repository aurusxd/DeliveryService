using DeliveryService.Commands;
using DeliveryService.Models;
using DeliveryService.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DeliveryService.ViewModels
{
    /// <summary>
    /// Логика взаимодействия пользователя и базы данных с MenuView
    /// </summary>
    public class MenuViewModel : BaseViewModel, IDisposable
    {
        private string _userName;
        private readonly WindowsService _windowsService;
        private readonly SessionService _sessionService;

        private readonly FoodCategoryService _categoryService;
        private readonly FoodService _foodService;
        private readonly BasketService _basketService;


        /// <summary>
        /// Событие, нужное для закрывания окна
        /// </summary>
        public event Action? CloseRequested;


        /// <summary>
        /// Список категорий
        /// </summary>
        private ObservableCollection<Categories> _categories;
        /// <summary>
        /// Список всей еды
        /// </summary>
        private ObservableCollection<Food> _menuItems;
        /// <summary>
        /// Список корзины пользователя
        /// </summary>
        private ObservableCollection<Basket> _basketItems;
        /// <summary>
        /// Полная сумма
        /// </summary>
        private decimal _totalPrice;



        /// <summary>
        /// Список категорий
        /// </summary>
        public ObservableCollection<Categories> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }
        /// <summary>
        /// Список всей еды
        /// </summary>
        public ObservableCollection<Food> MenuItems
        {
            get => _menuItems;
            set => SetProperty(ref _menuItems, value);
        }
        /// <summary>
        /// Список корзины пользователя
        /// </summary>
        public ObservableCollection<Basket> BasketItems
        {
            get => _basketItems;
            set => SetProperty(ref _basketItems, value);
        }
        /// <summary>
        /// Полная сумма
        /// </summary>
        public decimal TotalPrice
        {
            get => _totalPrice;
            set => SetProperty(ref _totalPrice, value);
        }

        /// <summary>
        /// Имя аккаунта клиента
        /// </summary>
        public string Username
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        /// <summary>
        /// Команда загрузки данных
        /// </summary>
        public ICommand LoadDataCommand { get; }
        /// <summary>
        /// Команда загрузки еды по категории
        /// </summary>
        public ICommand LoadMenuItemsCommand { get; }
        /// <summary>
        /// Команда добавления в корзину
        /// </summary>
        public ICommand AddToBasketCommand { get; }
        /// <summary>
        /// Команда удаления объекта из корзины
        /// </summary>
        public ICommand RemoveFromBasketCommand { get; }
        /// <summary>
        /// Команда создания заказа
        /// </summary>
        public ICommand CreateOrderCommand { get; }

        /// <summary>
        /// Команда выхода из аккаунта
        /// </summary>
        public ICommand LogoutCommand { get; }



        public MenuViewModel(WindowsService windowsService, SessionService sessionService,
            FoodCategoryService foodCategoryService, FoodService foodService, BasketService basketService)
        {
            _windowsService = windowsService;
            _sessionService = sessionService;

            _categoryService = foodCategoryService;
            _foodService = foodService;
            _basketService = basketService;

            Categories = new ObservableCollection<Categories>();
            MenuItems = new ObservableCollection<Food>();
            BasketItems = new ObservableCollection<Basket>();
            Username = _sessionService.CurrentClient.Name;

            this.CloseRequested += () => _windowsService.CloseWindow(this);

            LoadDataCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(LoadDataAsync, "Ошибка загрузки данных"),
                canExecute: () => !IsBusy
            );

            LoadMenuItemsCommand = new RelayCommandAsync(
                execute: async (parameter) =>
                {
                    if (parameter == null)
                        await TryRunTaskAsync(LoadMenuAsync, "Ошибка загрузки объектов еды").ConfigureAwait(false);
                    else
                    {
                        int categoryId = Convert.ToInt32(parameter);
                        await TryRunTaskAsync(() => LoadMenuFromCategoryAsync(categoryId), "Ошибка загрузки еды данной категории").ConfigureAwait(false);
                    }
                },
                canExecute: _ => !IsBusy
            );

            AddToBasketCommand = new RelayCommandAsync(
                execute: async (parameter) =>
                {
                    if (int.TryParse(parameter?.ToString(), out int foodId))
                        await TryRunTaskAsync(() => AddToBasketAsync(foodId, 1), "Ошибка загрузки объектов корзины").ConfigureAwait(false);
                    else
                        ErrorMessage = "Ошибка опериции";
                },
                canExecute: _ => !IsBusy
            );

            RemoveFromBasketCommand = new RelayCommandAsync(
                execute: async (parameter) =>
                {
                    if (int.TryParse(parameter?.ToString(), out int basketId))
                        await TryRunTaskAsync(() => RemoveBasketItemAsync(basketId), "Ошибка в удалении объекта корзины").ConfigureAwait(false);
                    else
                        ErrorMessage = "Ошибка опериции";
                },
                canExecute: _ => !IsBusy
            );

            CreateOrderCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(OpenNewOrder, "Ошибка открытия NewOrderView"),
                canExecute: () => !IsBusy
            );

            LogoutCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(Logout, "Ошибка выхода из аккаунта"),
                canExecute: () => !IsBusy
                );

            _sessionService.CurrentUserChanged += OnCurrentUserChanged;

            LoadDataCommand.Execute(null);
        }


        /// <summary>
        /// Заполнение коллекции
        /// </summary>
        /// <typeparam name="T">Тип коллекции</typeparam>
        /// <param name="collection">Коллекция</param>
        /// <param name="values">Значения для заполнения</param>
        private static void FillList<T>(ObservableCollection<T> collection, List<T> values)
        {
            collection.Clear();
            foreach (T item in values)
                collection.Add(item);
        }

        /// <summary>
        /// Загрузка всех категорий
        /// </summary>
        private async Task LoadCategoriesAsync()
        {
            var list = await _categoryService.GetAllAsync().ConfigureAwait(false);
            FillList(Categories, list);
        }
        /// <summary>
        /// Загрузка всех объектов еды
        /// </summary>
        private async Task LoadMenuAsync()
        {
            var items = await _foodService.GetAllAsync().ConfigureAwait(false);
            FillList(MenuItems, items);
        }
        /// <summary>
        /// Загрузка объектов еды по категории
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        private async Task LoadMenuFromCategoryAsync(int categoryId)
        {
            var items = await _foodService.GetAllFromCategoryAsync(categoryId).ConfigureAwait(false);
            FillList(MenuItems, items);
        }
        /// <summary>
        /// Загрузка объектов корзины
        /// </summary>
        private async Task LoadBasketAsync()
        {
            var (userBasket, totalPrice) = await _basketService.GetUserActiveBasketAsync(_sessionService.CurrentClient.Id).ConfigureAwait(false);

            FillList(BasketItems, userBasket);
            TotalPrice = totalPrice;
        }
        /// <summary>
        /// Добавление в корзину
        /// </summary>
        /// <param name="foodId">ID объекта еды</param>
        /// <param name="quantity">Количество</param>
        private async Task AddToBasketAsync(int foodId, int quantity)
        {
            bool success = await _basketService.AddOrUpdateBasketItemAsync(_sessionService.CurrentClient.Id, foodId, quantity).ConfigureAwait(false);
            if (success)
                await LoadBasketAsync().ConfigureAwait(false);
            else
                ErrorMessage = "Не удалось добавить товар в корзину";
        }
        /// <summary>
        /// Удаление из корзины
        /// </summary>
        /// <param name="basketItemId">ID объекта корзины</param>
        private async Task RemoveBasketItemAsync(int basketItemId)
        {
            bool success = await _basketService.RemoveItemAsync(basketItemId).ConfigureAwait(false);
            if (success)
                await LoadBasketAsync().ConfigureAwait(false);
            else
                ErrorMessage = "Не удалось удалить из корзины";
        }
        /// <summary>
        /// Загрузка данных
        /// </summary>
        private async Task LoadDataAsync()
        {
            await LoadCategoriesAsync().ConfigureAwait(false);
            await LoadMenuAsync().ConfigureAwait(false);

            if (_sessionService.CurrentClient != null)
                await LoadBasketAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Перезагрузка при изменении текущего пользователя
        /// </summary>
        private async void OnCurrentUserChanged()
        {
            if (IsBusy) return;
            await LoadDataAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Открытие окна NewOrderView для создания заказа
        /// </summary>
        private async Task OpenNewOrder()
        {
            if (_sessionService.CurrentClient == null)
            {
                ErrorMessage = "Id пользователя нет";
                return;
            }
            if (BasketItems.Count == 0)
            {
                ErrorMessage = "Корзина пуста";
                return;
            }

            bool? success = _windowsService.OpenNewOrder();
            if (success == true)
                await LoadBasketAsync().ConfigureAwait(false);
            else
                ErrorMessage = "Не удалось создать заказ";
        }

        /// <summary>
        /// Выход из аккаунта
        /// </summary>
        /// <returns></returns>
        private async Task Logout()
        {
            _sessionService.CurrentClient = null;
            _sessionService.CurrentOrder = null;
            _windowsService.OpenEntrance();
            CloseRequested?.Invoke();
        }

        /// <summary>
        ///Действия, происходящие при закрытии окна
        /// </summary>
        public void Dispose()
        {
            _sessionService.CurrentUserChanged -= OnCurrentUserChanged;
        }
    }
}