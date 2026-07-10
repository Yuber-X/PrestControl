# Changelog — PrestControl

Formato: [Keep a Changelog](https://keepachangelog.com/es/1.0.0/). Fechas en hora de República Dominicana.

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
