-- =============================================================
-- PrestControl — Usuario MySQL dedicado para la instalación final
-- Script: 003_crear_usuario_dedicado.sql
-- Ejecutar como root UNA sola vez en la PC del cliente.
--
-- ⚠ IMPORTANTE: cambiá 'CAMBIAR-ESTA-CLAVE' por una contraseña real
--   y usá esa misma contraseña en el App.config de PrestControl.
--   La aplicación NUNCA debe correr como root en producción.
-- =============================================================

CREATE USER IF NOT EXISTS 'prestcontrol'@'localhost'
  IDENTIFIED BY 'CAMBIAR-ESTA-CLAVE';

-- Permisos solo sobre la base de datos de la aplicación (nada más)
GRANT SELECT, INSERT, UPDATE ON prestcontrol_db.* TO 'prestcontrol'@'localhost';

-- Necesarios para respaldar desde Configuración (mysqldump)
GRANT LOCK TABLES, SHOW VIEW ON prestcontrol_db.* TO 'prestcontrol'@'localhost';

FLUSH PRIVILEGES;

-- Verificación rápida:
-- SHOW GRANTS FOR 'prestcontrol'@'localhost';
--
-- Nota: NO se otorga DELETE (la app nunca borra: usa soft deletes) ni DROP/ALTER.
-- La restauración de respaldos desde Configuración requiere privilegios de
-- estructura (DROP/CREATE); hacela con root o otorgá temporalmente:
--   GRANT ALL PRIVILEGES ON prestcontrol_db.* TO 'prestcontrol'@'localhost';
