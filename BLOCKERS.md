# BLOCKERS.md — PrestControl

> Bloqueos y decisiones que requieren consulta. Actualizado: 2026-07-10.

## Resueltos

| Fecha | Tema | Resolución |
|---|---|---|
| 2026-07-10 | PTV-300 no estaba en el workspace | ✅ Clonado desde GitHub (repo público `Yuber-X/PTV-300`) junto con POS-400. Ver `PTV300-PATTERNS.md`. |
| 2026-07-10 | `metodo_amortizacion`: ¿qué es `cuota_fija` vs `frances`? | ✅ Yuber confirmó: `cuota_fija` = interés simple dominicano (sobre capital original) y es el DEFAULT. Francés queda implementado como alternativo. |
| 2026-07-10 | Convención de tasa por modalidad | ✅ Decisión (investigación sin convención documentada): tasa siempre MENSUAL, conversión ÷2 quincenal, ÷4 semanal, ÷30 diaria. **Corregible** — confirmar con el cliente real. |
| 2026-07-10 | Nombre PrestaControl vs PrestControl | ✅ PrestControl en todo (UI, namespaces, `prestcontrol_db`). |

## Abiertos (no bloquean, decidir a futuro)

1. **ENUM `cuota.estado`:** el CLAUDE.md define `('pendiente','pagada','vencida','en_mora')` pero la regla §8.4 exige marcar cuotas como canceladas al cancelar un préstamo. Se agregó `'cancelada'` al ENUM del schema. Confirmar que está bien.
2. **Credenciales MySQL del cliente final:** Dev usa root local. En la instalación (Fase 7) se debe crear un usuario MySQL dedicado `prestcontrol` con permisos solo sobre `prestcontrol_db`.
3. **Comportamiento de logout:** hoy "Cerrar sesión" cierra la aplicación. ¿Debería volver a la pantalla de login? (UX menor, decidir en Fase 6).
4. **Negativos en pagos compensatorios:** la regla "nunca modificar un pago" implica pagos negativos; definir en Fase 4 si requieren nota obligatoria (propuesto: sí).
