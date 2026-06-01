# 🚚 DeliveryService

Система онлайн доставки заказов. Разработано на C# WPF.

## Возможности

- **Диспетчерская** — интерактивная карта с позициями курьеров и точками доставки в реальном времени
- **Управление заказами** — создание, назначение курьера, отслеживание статусов
- **Курьеры** — список, статус онлайн/офлайн, статистика по выполненным заказам
- **Симуляция доставки** — визуализация движения курьера по маршруту на карте

## Технологии

| Слой | Технология |
|---|---|
| UI | WPF (.NET 8), XAML |
| Паттерн | MVVM |
| БД | PostgreSQL + Entity Framework Core |
| Карта | Yandex Maps API |
| DI | Microsoft.Extensions.DependencyInjection |

## Структура проекта

```
DeliveryService/                          # корень репозитория
├── DeliveryService.slnx
└── DeliveryService/
   ├── App.xaml
    ├── App.xaml.cs                       # DI
├── appsettings.json                     
    ├── Models/                           # сущности БД
    │   ├── Order.cs
    │   ├── Courier.cs
    │   ├── Client.cs
    │   ├── OrderStatusHistory.cs
    │   ├── RoutePoint.cs
    │   ├── Food.cs
    │   ├── Categories.cs
    │   └── Backet.cs                      
    │
    ├── Data/                             # EF Core
    │   ├── AppDbContext.cs
    │   
    │
    ├── Migrations/                       # миграции
    │   └── …
    │
    ├── Repositories/                     # слой доступа к данным
    │   ├── OrderRepository.cs
    │   ├── CourierRepository.cs
    │   ├── ClientRepository.cs
    │   ├── FoodRepository.cs
    │   ├── FoodCategoryRepository.cs
    │   └── BasketRepository.cs
    │
    ├── Services/                         # бизнес-логика
    │   ├── OrderService.cs
    │   ├── CourierService.cs
    │   ├── ClientService.cs
    │   ├── SimulationService.cs
    │   ├── FoodService.cs
    │   ├── FoodCategoryService.cs
    │   ├── BasketService.cs
    │   ├── SessionService.cs
    │   └── WindowsService.cs
    │
    ├── DTO/                              # объекты передачи данных
    │   ├── OrderDTO.cs
    │   ├── CourierDTO.cs
    │   ├── AddressDTO.cs
    │   └── CoordinatesDTO.cs
    │
    ├── ViewModels/                       # MVVM
    │   ├── BaseViewModel.cs
    │   ├── MainWindowModel.cs
    │   ├── EntranceViewModel.cs
    │   ├── AuthorizationViewModel.cs
    │   ├── RegistrationViewModel.cs
    │   ├── RegistrationCourierModel.cs
    │   ├── DispatcherViewModel.cs
    │   ├── OrderListViewModel.cs
    │   ├── ListCouriersViewModel.cs
    │   ├── NewOrderViewModel.cs
    │   ├── MenuViewModel.cs
    │   └── OrderAcceptViewModel.cs
    │
    ├── Views/                            # XAML-экраны
    │   ├── MainWindow.xaml
    │   ├── EntranceView.xaml
    │   ├── LoginPage.xaml
    │   ├── RegisterPage.xaml
    │   ├── RegistrationCourier.xaml
    │   ├── MenuView.xaml
    │   ├── NewOrderView.xaml
    │   ├── OrderAcceptView.xaml
    │   ├── ordersPage.xaml
    │   ├── dispatcherPage.xaml
    │   └── listCouriersPage.xaml
    │
    ├── Commands/                         # RelayCommand для MVVM
    │   ├── RelayCommand.cs
    │   └── RelayCommandAsync.cs
    │
    ├── Utils/                            # конвертеры, карта, скрипт
    │   ├── StatusToVisibilityConverter.cs
    │   ├── CourierStatusConverter.cs
    │   ├── MapInitializer.cs
    │   └── script.js
    │
    ├── Styles/                           # общие стили WPF
    │   ├── ButtonStyles.xaml
    │   └── TextStyles.xaml
    │
    └── Images/                           # ресурсы меню
        ├── cheesecake.png
        ├── coffee.png
        ├── croissant.png
        └── plov.png
```

## Запуск

### Требования

- Visual Studio 2022
- .NET 8 SDK
- PostgreSQL 15+

### Установка

1. Клонировать репозиторий:
   ```bash
   git clone https://github.com/aurusxd/DeliveryService
   cd DeliveryService
   ```

2. Создать базу данных в PostgreSQL:
   ```sql
   CREATE DATABASE delivery_db;
   ```

3. Настроить строку подключения в `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "Default": "Host=localhost;Port=5432;Database=delivery_db;Username=postgres;Password=ваш пароль от postgres"
     }
   }
   ```

4. Открыть `DeliveryService.sln` в Visual Studio и запустить (F5)

## Команда

| Участник | Зона ответственности |
|---|---|
| aurusxd | Backend + Frontend + TL|
| DanilKozlov1 | Backend |
| s-k-1-n-1 | Frontend |
| romchik-ww | Frontend |


---
