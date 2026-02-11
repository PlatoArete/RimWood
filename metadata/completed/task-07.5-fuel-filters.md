# Task 7.5: Per-building Fuel Filters (C# Implementation)

## User Story
As a player, I want to manually configure which fuels are accepted by specific buildings so that I can optimize my fuel usage (e.g., save charcoal for the forge and use firewood for heating).

## Acceptance Criteria
- [ ] **Custom Component:** A new C# `ThingComp` (e.g., `CompFuelFilter`) that holds a `ThingFilter` for fuel types.
- [ ] **XPath Integration:** This comp is added to all vanilla refuelable buildings via patches.
- [ ] **UI Gizmo:** Buildings with this comp display a "Fuel Filter" gizmo/button that opens a standard RimWorld filter dialog.
- [ ] **Logic Hook:** Harmony patch on `CompRefuelable` to ensure it respects the `ThingFilter` in our custom comp during refueling jobs.
- [ ] **Persistence:** Filter settings are saved and loaded correctly per building.
- [ ] **Global Override:** The global "Allow logs" mod setting still acts as a hard-stop, even if the per-building filter would otherwise allow them.

## Technical Notes
- **Avoid:** Do not attempt to use `<showFilter>` in XML (deprecated/non-existent in 1.6).
- **Avoid:** Do not replace `CompRefuelable` with a custom subclass, as this breaks mod compatibility. Use a parallel component instead.