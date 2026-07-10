# TODO.md — PrestControl

> Actualizado: 2026-07-10 (cierre de Fase 1 + núcleo de Fase 3)

## ✅ Fase 1 — Cimientos (COMPLETA)
- [x] Solución `.sln` con 8 proyectos src + 2 de tests, regla de dependencias respetada
- [x] `App.config` externo con cadena de conexión (Dev: root local)
- [x] `001_create_schema.sql` ejecutado en MySQL local (8 tablas) + seed + rollback
- [x] `ConexionFactory` (CConexion adaptado: async, config externa, sin UI)
- [x] Autenticación BCrypt cost 12: wizard de cuenta inicial + login + rate-limiting (5 intentos → 5 min)
- [x] `SesionActual` estático simplificado (Id, Username, Nombre, LoginAt, SesionId)
- [x] `AuditoriaService` funcional (con variante transaccional para operaciones multi-paso)
- [x] `MainWindow` con sidebar navegable (8 secciones, páginas placeholder)
- [x] DESIGN.md aplicado: paleta, tipografía, estilos de botón/input/card/sidebar, MoneyConverter, DateConverter
- [x] `PTV300-PATTERNS.md` documentado

## ✅ Fase 3 (núcleo) — Amortización (ADELANTADA, COMPLETA)
- [x] `AmortizacionService`: interés simple dominicano (default) + sistema francés
- [x] `CuotaEstadoCalculator` (semáforo) con 100% de ramas cubiertas
- [x] 40 tests unitarios verdes, incluye los 4 casos obligatorios del CLAUDE.md y el caso del mockup (75,000 → 8,461.91)
- [x] `docs/AMORTIZATION.md` con la matemática y la decisión de convención de tasa

## 🔜 Fase 3 (restante) — Préstamos UI + persistencia
- [ ] `PrestamoRepository` + transacción atómica (prestamo + N cuotas + auditoría)
- [ ] Generación de `codigo` P-0001 vía tabla `contador` con `SELECT ... FOR UPDATE`
- [ ] Wizard "Nuevo préstamo" con vista previa de amortización en vivo (mockup 5)
- [ ] Lista de préstamos + detalle con tabla de cuotas (mockups 4)

## 🔜 Fase 4 — Cobros
- [ ] `PagoService`: pago exacto, abono parcial (interés primero), adelanto, liquidación anticipada
- [ ] `numero_recibo` atómico (contador + FOR UPDATE)
- [ ] Recibo 80mm (patrón imagen del POS-400) + PDF
- [ ] Módulo Pagos (mockup 6)

## ⏳ Fases 2, 5, 6, 7 (post-12-julio, modelos menores)
- [ ] Fase 2: CRUD Clientes completo
- [ ] Fase 5: Dashboard con KPIs + LiveChartsCore
- [ ] Fase 6: 6 reportes + Historial auditoría + Configuración (cambio password, respaldo mysqldump)
- [ ] Fase 7: Instalador (Inno Setup) + INSTALL.md + manual de usuario
- [ ] Logout que regrese al login sin cerrar la app (hoy cierra la app — decisión pendiente)
- [ ] Empaquetar fuente Inter .ttf (hoy usa fallback Segoe UI)
