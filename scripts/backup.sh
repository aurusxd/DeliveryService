#!/bin/bash
# Настройки подключения на основе вашего appsettings.json
DB_HOST="localhost"
DB_NAME="delivery_db"
DB_USER="postgres"
export PGPASSWORD="424242"

# Директории
BACKUP_DIR="backups"
DATE=$(date +%Y_%m_%d_%H_%M_%S)

# Дамп базы данных
pg_dump -h $DB_HOST -U $DB_USER $DB_NAME > "$BACKUP_DIR/db_backup_$DATE.sql"

# Архивирование медиа-файлов (если папка называется uploads)
if [ -d "./uploads" ]; then
    tar -czf "$BACKUP_DIR/media_backup_$DATE.tar.gz" ./uploads
fi

echo "Бэкап успешно создан: $DATE"