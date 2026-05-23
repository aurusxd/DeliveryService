using DeliveryService.Data;
using DeliveryService.Repositories;
using DeliveryService.Services;
using DeliveryService.ViewModels;
using DeliveryService.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DeliveryService
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            

            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();


            var services = new ServiceCollection();


            // БД
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("Default")));
            

            // Репозитории
            services.AddScoped<OrderRepository>();
            services.AddScoped<CourierRepository>();
            services.AddScoped<ClientRepository>();
            services.AddScoped<FoodCategoryRepository>();
            services.AddScoped<FoodRepository>();
            services.AddScoped<BasketRepository>();
            // Сервисы
            services.AddScoped<WindowsService>();
            services.AddScoped<OrderService>();
            services.AddScoped<CourierService>();
            services.AddScoped<ClientService>();
            services.AddScoped<FoodCategoryService>();
            services.AddScoped<FoodService>();
            services.AddScoped<BasketService>();

            // ViewModels
            services.AddTransient<MainWindowModel>();
            services.AddTransient<ListCouriersViewModel>();
            services.AddTransient<OrderListViewModel>();
            services.AddTransient<NewOrderViewModel>();
            services.AddTransient<RegistrationCourierModel>();
            services.AddTransient<DispatcherViewModel>();
            services.AddTransient<MenuViewModel>();

            // View
            services.AddTransient<MainWindow>();
            services.AddTransient<ListCouriersView>();
            services.AddTransient<OrdersListView>();
            services.AddTransient<NewOrderView>();
            services.AddTransient<RegistrationCourier>();
            services.AddTransient<DispatcherView>();
            services.AddTransient<MenuView>();

            // Собираем контейнер
            Services = services.BuildServiceProvider();

            // Открываем главное окно - пока затычка
            var win = Services.GetRequiredService<MainWindow>();
            win.Show();
        }
    }

}
