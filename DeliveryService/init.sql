-- =============================================
-- Создание пользователя delivery_user
-- =============================================
DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'delivery_user') THEN
        CREATE USER delivery_user WITH PASSWORD '123';
    END IF;
END
$$;
 
GRANT ALL PRIVILEGES ON DATABASE delivery_db TO delivery_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO delivery_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO delivery_user;
 