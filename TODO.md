# TODO.md — PrestControl

> Actualizado: 2026-07-10 (cierre de Fases 2, 3 y 4)

## ✅ Fase 2 — Clientes (COMPLETA)
- [x] Lista con búsqueda por nombre/cédula/teléfono + agregados (préstamos activos, saldo)
- [x] Ficha: 5 métricas + contacto + préstamos del cliente + doble click al detalle
- [x] Formulario nuevo/editar con validación inline (cédula normalizada a 000-0000000-0)
- [x] Soft delete protegido: no se elimina un cliente con préstamos activos
- [x] Auditoría de crear/modificar/eliminar
- [x] "+ Nuevo préstamo" desde la ficha preselecciona el cliente en el wizard
- [x] Fix: LoginWindow crece con el contenido (botón cortado en el wizard inicial)

## ✅ Fase 1 — Cimientos (COMPLETA)
- [x] Solución `.sln` con 8 proyectos src + 2 de tests, regla de dependencias respetada
- [x] `App.config` externo con cadena de conexión (Dev: root local)
- [x] `001_create_schema.sql` ejecutado en MySQL local (8 tablas) + seed + rollback
- [x] `ConexionFactory` (CConexion adaptado: async, config externa, sin UI)
- [x] Autenticación BCrypt cost 12: wizard de cuenta inicial + login + rate-limiting (5 intentos → 5 min)
- [x] `SesionActual` estático simplificado (Id, Username, Nombre, LoginAt, SesionId)
- [x] `AuditoriaService` funcional (con variante transaccional para operaciones multi-paso)
- [x] `MainWindow` con sidebar navegable (8 secciones)
- [x] DESIGN.md aplicado: paleta, tipografía, estilos de botón/input/card/sidebar, MoneyConverter, DateConverter
- [x] `PTV300-PATTERNS.md` documentado

## ✅ Fase 3 — Préstamos y amortización (COMPLETA)
- [x] `AmortizacionService`: interés simple dominicano (default) + sistema francés (40 tests)
- [x] `CuotaEstadoCalculator` (semáforo) con 100% de ramas cubiertas
- [x] `docs/AMORTIZATION.md` con la matemática y la decisión de convención de tasa
- [x] `PrestamoRepository` + `ClienteRepository` + `ContadorRepository` (`SELECT ... FOR UPDATE`)
- [x] `PrestamoService.CrearAsync`: transacción atómica contador → prestamo → N cuotas → auditoría
- [x] Generación de `codigo` P-0001 vía tabla `contador`
- [x] `PrestamoService.CancelarAsync`: cuotas impagas → 'cancelada' (nunca se borran)
- [x] Wizard "Nuevo préstamo" con vista previa de amortización EN VIVO + resumen
- [x] Lista de préstamos (búsqueda, agregados por SQL, pills de estado)
- [x] Detalle de préstamo (métricas, contrato, tabla de cuotas con semáforo e indicador rojo en vencidas)
- [x] Navegación por páginas (ContentControl + DataTemplates): lista → detalle → cobros

## ✅ Fase 4 — Cobros (COMPLETA)
- [x] `PagoService`: pago exacto, abono parcial (interés primero), adelanto en cascada,
      liquidación anticipada (exonera interés futuro) — 17 tests de la lógica pura
- [x] `numero_recibo` atómico R-000001 (contador + FOR UPDATE, dentro de la transacción del cobro)
- [x] Transacción de cobro: cuotas FOR UPDATE → N pagos → update cuotas → estado préstamo → auditoría
- [x] Módulo Cobros: selector de préstamo activo, cuotas pendientes con semáforo,
      atajos (cuota completa / liquidar), preview de distribución, pagos recientes
- [x] Recibo 80mm como visual WPF (patrón imagen POS-400): vista previa + impresión (PrintVisual) + PDF (PdfSharp)
- [x] Tests de integración contra `prestcontrol_test`: flujo completo crear → abonar → adelantar → liquidar; cancelación

## ✅ Fase 5 — Dashboard (COMPLETA)
- [x] 4 KPIs: capital colocado, cobros del mes (delta vs mes anterior), clientes activos, morosidad RD$ + %
- [x] Panel de alertas de cobro con semáforo y botón "Cobrar" → Cobros preseleccionado
- [x] Gráfico de cobros diarios del mes (LiveChartsCore 2.0.5)
- [x] Últimos movimientos (10 pagos recientes)

## ⏳ Fases 6, 7 (pendientes)
- [ ] Fase 6: 6 reportes + Historial auditoría + Configuración (cambio password, respaldo mysqldump)
- [ ] Fase 6 · Configuración — **pedidos de Yuber 2026-07-10** (ver `Claude Active/PrestaControl/Nuevos detalles a agregar.txt`):
  - [ ] Tamaño de texto configurable: Pequeño / Mediano / Grande (escala tipográfica global; el usuario percibe las letras muy pequeñas)
  - [ ] Exportar/Importar TODOS los datos (clientes, préstamos, cuotas, pagos, historial) a Excel para migrar de PC
  - [ ] Export automático a Excel con lapso ajustable (default mensual), con switch activar/desactivar en Configuración
- [ ] Fase 6: Historial con filtros por fecha, entidad y acción (pedido de Yuber: igual que Préstamos/Clientes)
- [ ] Fase 7: Instalador (Inno Setup) + INSTALL.md + manual de usuario
- [ ] Fase 7: **Documentación final detallada e increíblemente fácil de entender** (pedido explícito de Yuber 2026-07-10): guía de uso pantalla por pantalla con capturas, en lenguaje no técnico
- [ ] Logout que regrese al login sin cerrar la app (hoy cierra la app — decisión pendiente)
- [ ] Empaquetar fuente Inter .ttf (hoy usa fallback Segoe UI)
- [ ] Sidebar: marcar el ítem activo cuando la navegación no viene de un click (detalle/nuevo)
- [ ] Pago compensatorio negativo (corrección de errores) con nota obligatoria — regla definida, UI pendiente
