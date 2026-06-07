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

Архитектура и структура проекта
Общая архитектура
Проект построен по типовой архитектуре многоуровневого приложения:

API слой — прием запросов и формирование ответов (Controllers)
Бизнес-логика — сервисы и менеджеры
Доступ к данным — Entity Framework Core, миграции
Хранилище данных — SQL Server

```
DeliveryService/                          # корень репозитория
├── DeliveryService.slnx
└── DeliveryService/
   ├── App.xaml
    ├── App.xaml.cs                       # DI
├── appsettings.json                     
    ├── Models/                           # сущности БД             
    │
    ├── Data/                             # EF Core
    │   
    │
    ├── Migrations/                       # миграции
    │   
    │
    ├── Repositories/                     # слой доступа к данным
    │  
    │
    ├── Services/                         # бизнес-логика
    │  
    │
    ├── DTO/                              # объекты передачи данных
    │ 
    │
    ├── ViewModels/                       # MVVM
    │   
    │
    ├── Views/                            # XAML-экраны
    │  
    │
    ├── Commands/                         # RelayCommand для MVVM
    │   
    │
    ├── Utils/                            # конвертеры, карта, скрипт
    │  
    │
    ├── Styles/                           # общие стили WPF
    │   
    │
    └── Images/                           # ресурсы меню
       
```
## Требования к системе
Операционная система: Windows 10/11, Linux (Ubuntu 20.04+), MacOS
Процессор: не менее 2-ядерного CPU
ОЗУ: минимум 4 ГБ RAM
Дополнительное ПО: .NET 7 SDK, SQL Server (локально или через Docker)


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

---

### Альтернатива: запуск через Docker

Создайте файл `docker-compose.yml`:

```yaml
version: '3.4'

services:
  api:
    build: .
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__Default=Server=db;Database=DeliveryDB;User Id=sa;Password=YOUR_PASSWORD
  db:
    image: mcr.microsoft.com/mssql/server
    environment:
      SA_PASSWORD: "YourStrong@Password"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
```

Запуск команды:

```bash
docker-compose up -d
```

---


4. Открыть `DeliveryService.sln` в Visual Studio и запустить (F5)

## Команда

| Участник | Зона ответственности |
|---|---|
| aurusxd | Backend + Frontend + TL|
| DanilKozlov1 | Backend |
| s-k-1-n-1 | Frontend |
| romchik-ww | Frontend |


---
##Руководство администратора (Deployment & Recovery)

1. Требования к серверу
ОС: Windows Server 2019 или Linux Ubuntu 20.04+
Процессор: минимум 4 ядра
ОЗУ: не менее 8 ГБ
Версия SQL Server или другой совместимый СУБД
2. Развертывание на боевом сервере
Установите .NET 7 Runtime/SDK.
Настройте IIS (для ASP.NET Core) или запускать через systemd/nginx.
Настройте переменные окружения (см. раздел выше).
Настройте автоматический запуск через службу systemd или аналог.
3. Конфигурация обратного прокси (Nginx / Apache)
Пример Nginx:

nginx

server {
    listen 80;
    server_name yourdomain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}


### Регламент действий при восстановлении работы базы данных

**Проблема** | **Меры**
---|---
Служба PostgreSQL не запускается | Откройте `services.msc`, найдите PostgreSQL и перезапустите службу. Если при этом возникает ошибка, ознакомьтесь с лог-файлом по пути `C:\Program Files\PostgreSQL\15\data\log\`.
Закончилось место на диске C: | Выполните очистку диска: удалите временные файлы, старые дампы и ненужные файлы.
База данных повреждена, есть резервная копия | Восстановите базу командой: `psql -U postgres -d delivery_db -f backup.sql`.
Отсутствует резервная копия | В случае отсутствия резервной копии можно попробовать восстановить структуру базы через Visual Studio — команда `Update-Database`. В этом случае данные заказов, курьеров и истории доставок будут утеряны.

---
