# Changelog — PrestControl

Formato: [Keep a Changelog](https://keepachangelog.com/es/1.0.0/). Fechas en hora de República Dominicana.

## [0.3.1] — 2026-07-10 · Pulido de UI (10 observaciones de Yuber)

### Added
- **Totales del grid de Préstamos**: 4 cards (capital prestado, por cobrar en activos, total cobrado, préstamos activos) que se recalculan con cada búsqueda/filtro.
- **Filtros rápidos**: por estado en Préstamos (Todos/Activos/Pagados/Cancelados) y por situación en Clientes (con/sin préstamos activos, con saldo pendiente).
- **Iconos en el sidebar** con glifos nativos Segoe MDL2 Assets (sin dependencias).

### Changed
- El **sidebar refleja la navegación interna**: crear un préstamo te lleva al detalle y ahora marca "Préstamos" (antes quedaba en "Nuevo préstamo").
- **ScrollBars delgados (6px)** en toda la app, con thumb redondeado y hover — discretos pero funcionales.
- **Encabezados de tabla centrados** y **scroll horizontal** en las tablas cuando las columnas no caben (el usuario también puede redimensionar columnas).
- La ventana principal abre **maximizada** por defecto.
- Botón de cerrar sesión ahora es **circular** con hover indigo.

### Pendiente registrado
- Documentación final detallada y fácil de entender → Fase 7. Filtros del Historial → Fase 6.

## [0.3.0] — 2026-07-10 · Fase 2 (Clientes) + ajustes de UI

### Added
- **Módulo Clientes completo**: lista con búsqueda (nombre/cédula/teléfono) y agregados por SQL, ficha con 5 métricas (total prestado, cobrado, saldo, préstamos activos, cuotas vencidas) + datos de contacto + sus préstamos, formulario nuevo/editar con validación inline.
- **`ClienteService`**: normalización de cédula dominicana (11 dígitos → `000-0000000-0`; pasaportes se aceptan tal cual), unicidad de cédula amigable, soft delete **bloqueado si hay préstamos activos**, auditoría de crear/modificar/eliminar.
- Flujo ficha → "+ Nuevo préstamo" con el cliente preseleccionado en el wizard.
- Tests: 8 unitarios de normalización de cédula + 2 de integración (CRUD protegido y métricas).

### Fixed
- **LoginWindow**: `SizeToContent="Height"` — en el wizard de primer arranque el botón "Crear cuenta" quedaba cortado por la altura fija (reporte de Yuber).

### Notas
- Pedidos nuevos registrados para Fase 6 (Configuración): tamaño de texto (Pequeño/Mediano/Grande) y exportar/importar datos a Excel con export automático programable. Ver TODO.md.

## [0.2.0] — 2026-07-10 · Fase 3 completa (Préstamos) + Fase 4 (Cobros)

### Added
- **Persistencia de préstamos**: `PrestamoRepository`, `ClienteRepository` (lectura), `PagoRepository`, `ContadorRepository` (correlativos atómicos con `SELECT ... FOR UPDATE`).
- **`PrestamoService`**: creación atómica (contador → prestamo → N cuotas → auditoría en UNA transacción, código P-0001) y cancelación (cuotas impagas → `'cancelada'`, jamás se borran).
- **`PagoService`**: los 4 escenarios de cobro — pago exacto, abono parcial (primero interés, luego capital), adelanto en cascada y liquidación anticipada (cuotas futuras pagan solo capital; el interés futuro se exonera). Todo cobro es una transacción con cuotas bloqueadas (`FOR UPDATE`) y `numero_recibo` R-000001 atómico.
- **UI Préstamos**: lista con búsqueda y agregados en SQL, detalle con métricas + tabla de cuotas con semáforo (indicador rojo lateral en vencidas), wizard "Nuevo préstamo" con vista previa de amortización EN VIVO.
- **UI Cobros**: selector de préstamo activo, cuotas pendientes, atajos (cuota completa / liquidar), preview de cómo se distribuye el monto antes de confirmar, historial de pagos recientes.
- **Recibo 80mm** (patrón imagen del POS-400): el mismo visual se muestra, se imprime (`PrintVisual`) y se exporta a PDF (PdfSharp 6.1.1, rasterizado a 192 DPI).
- **Navegación por páginas**: ContentControl + DataTemplates (ViewModel → View); flujos lista → detalle → cobros cableados por eventos.
- **`IDialogService`** inyectable (confirmaciones/errores testeables; MessageBox solo en la capa App).
- Estilos DESIGN.md: tablas (header uppercase, filas 44px, hover, selección indigo), pills de semáforo y de estado de préstamo, botones Destructivo/Terciario.
- **Tests**: 17 nuevos de distribución de pagos (57 unitarios en total) + 2 de integración contra `prestcontrol_test` (BD real recreada por corrida: flujo completo crear → abonar → adelantar → liquidar, y cancelación).

### Decisiones
- **Liquidación anticipada** exonera el interés de cuotas futuras (solo se cobra su capital pendiente). Corregible con el cliente — ver BLOCKERS.md.
- **Recibos multi-cuota**: `pago.numero_recibo` es UNIQUE por fila; un cobro que afecta varias cuotas genera un recibo por cuota y el impreso agrupa la operación bajo el primer número.
- PdfSharp 6.1.1 para el PDF del recibo (el mismo visual WPF rasterizado — papel y archivo idénticos).

## [0.1.0] — 2026-07-10 · Fase 1 (Cimientos) + Fase 3 núcleo (Amortización)

### Added
- Solución .NET 8 con arquitectura por capas: App / Views / ViewModels / Models / Services / Data / Common / Printing + 2 proyectos de tests (xUnit + FluentAssertions 7).
- Esquema MySQL `prestcontrol_db` (8 tablas: usuario, sesion, cliente, prestamo, cuota, pago, auditoria, contador) con scripts de creación, seed Dev y rollback.
- `AmortizacionService`: interés simple dominicano (default) y sistema francés, con redondeo AwayFromZero y ajuste en última cuota. 100% decimal, sin double.
- `CuotaEstadoCalculator`: semáforo de cobros (al día / por vencer / vencida / en mora / pagada / cancelada) con 100% de ramas testeadas.
- 40 tests unitarios verdes (incluye validación exacta contra el mockup: 75,000 al 5% × 12 → cuota 8,461.91).
- Autenticación mono-usuario: wizard de primer arranque, login BCrypt cost 12, rate-limiting (5 intentos → bloqueo 5 min), registro de sesiones, cambio de contraseña.
- `AuditoriaService` con variante transaccional para operaciones multi-paso.
- Shell WPF: LoginWindow + MainWindow con sidebar de 240px (8 secciones) según DESIGN.md (paleta indigo, tipografía, estilos de botones/inputs/cards, converters de moneda RD$ y fecha).
- Serilog a archivo rotativo diario en `logs/`.
- Documentación: `docs/AMORTIZATION.md`, `PTV300-PATTERNS.md`, `TODO.md`, `BLOCKERS.md`.

### Decisiones
- Tasa de interés se ingresa MENSUAL y se convierte por período (÷2 quincenal, ÷4 semanal, ÷30 diaria) — ver AMORTIZATION.md §1.
- ENUM `cuota.estado` amplía la spec con `'cancelada'` (exigido por la regla de cancelación de préstamos).
- FluentAssertions fijado en 7.2.0 (la v8 requiere licencia comercial de pago).
