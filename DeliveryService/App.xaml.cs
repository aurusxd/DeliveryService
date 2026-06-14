using DeliveryService.Data;
using DeliveryService.Repositories;
using DeliveryService.Services;
using DeliveryService.ViewModels;
using DeliveryService.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Windows;

namespace DeliveryService
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection();

            services.AddLogging(builder => builder.AddSerilog());

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("Default")));

            services.AddScoped<OrderRepository>();
            services.AddScoped<CourierRepository>();
            services.AddScoped<ClientRepository>();
            services.AddScoped<FoodCategoryRepository>();
            services.AddScoped<FoodRepository>();
            services.AddScoped<BasketRepository>();

            services.AddSingleton<SessionService>();
            services.AddSingleton<WindowsService>();
            services.AddScoped<SimulationService>();
            services.AddScoped<OrderService>();
            services.AddScoped<CourierService>();
            services.AddScoped<ClientService>();
            services.AddScoped<FoodCategoryService>();
            services.AddScoped<FoodService>();
            services.AddScoped<BasketService>();

            services.AddTransient<MainWindowModel>();
            services.AddTransient<ListCouriersViewModel>();
            services.AddTransient<OrderListViewModel>();
            services.AddTransient<NewOrderViewModel>();
            services.AddTransient<RegistrationCourierModel>();
            services.AddTransient<DispatcherViewModel>();
            services.AddTransient<MenuViewModel>();
            services.AddTransient<EntranceViewModel>();
            services.AddTransient<RegistrationViewModel>();
            services.AddTransient<OrderAcceptViewModel>();
            services.AddTransient<AuthorizationViewModel>();

            services.AddTransient<MainWindow>();
            services.AddTransient<NewOrderView>();
            services.AddTransient<RegistrationCourier>();
            services.AddTransient<EntranceView>();
            services.AddTransient<MenuView>();
            services.AddTransient<OrderAcceptView>();

            Services = services.BuildServiceProvider();

            Log.Information("Приложение запущено");

            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();

            var startupScope = Services.CreateScope();
            var win = startupScope.ServiceProvider.GetRequiredService<EntranceView>();
            win.Closed += (_, _) => startupScope.Dispose();
            win.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Приложение закрыто");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}