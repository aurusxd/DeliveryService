#!/bin/bash
# Использование: ./restore.sh ДАТА_ИЗ_ИМЕНИ_ФАЙЛА
DATE=$1
DB_NAME="delivery_db"
DB_USER="postgres"
export PGPASSWORD="424242"

BACKUP_DIR="backups"

# 1. Восстановление БД
dropdb -h localhost -U $DB_USER $DB_NAME
createdb -h localhost -U $DB_USER $DB_NAME
psql -h localhost -U $DB_USER -d $DB_NAME -f "$BACKUP_DIR/db_backup_$DATE.sql"

# 2. Восстановление медиа-файлов
if [ -f "$BACKUP_DIR/media_backup_$DATE.tar.gz" ]; then
    rm -rf ./uploads
    tar -xzf "$BACKUP_DIR/media_backup_$DATE.tar.gz"
fi

echo "Система успешно восстановлена из бэкапа: $DATE"