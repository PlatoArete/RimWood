# Epic: RimWood Project Implementation

This epic tracks the end-to-end implementation of the RimWood mod, from initial scaffolding to a feature-complete release.

## General Approach
- **Incremental Development:** Build and test in small, functional increments.
- **In-Game Testing:** Every major milestone should be verifiable by launching RimWorld with the mod.
- **XML-First, C#-Second:** Implement basic items and buildings via XML before adding complex logic via C#.

---

## Phase 1: Foundation & MVP Materials (Current Focus)

### [x] Task 1: Project Scaffolding
- [x] Create folder structure: `About/`, `Defs/`, `Assemblies/`, `Source/`, `Textures/`.
- [x] Create `About.xml` with mod metadata.
- [x] Create `LoadFolders.xml` if needed.

### [x] Task 2: Basic Fuel Materials (XML)
- [x] Define `Green Firewood` (ThingDef).
- [x] Define `Seasoned Firewood` (ThingDef).
- [x] Define `Charcoal` (ThingDef).
- [x] Define `RimWood_Fuel` (ThingCategoryDef).

### [x] Task 3: The Chopping Block (XML)
- [x] Define `ChoppingBlock` (BuildingDef).
- [x] Create Recipe: `Logs -> Green Firewood`.
- [x] Create Recipe: `Logs -> Wood Fibre Pads` (Placeholder for Phase 2).

---

## Phase 2: Core Mechanics (C# & Advanced XML)

### [ ] Task 4: Reverse-Rot Seasoning System
- [ ] Create C# Project and setup references.
- [ ] Implement `CompSeasonable` (based on `CompRottable` logic).
- [ ] Apply `CompSeasonable` to Green Firewood.
- [ ] Implement seasoning speed scaling by temperature.

### [ ] Task 5: The Woodpile & Woodshed
- [ ] Define `Woodpile` building (Constructed from Green Firewood).
- [ ] Implement "Unpack on Seasoned" logic or manual harvesting.
- [ ] Define `Woodshed` storage building.
- [ ] Implement Seasoning Speed Multipliers for specific buildings.

### [ ] Task 6: Earth Kiln
- [ ] Define `EarthKiln` building.
- [ ] Implement processing logic (Mech gestator style or standard bill).

---

## Phase 3: Integration & Polish

### [ ] Task 7: Vanilla Overrides (Patches)
- [ ] Patch `Campfire`, `Stove`, `Smithy` to accept new fuels.
- [ ] Implement `Logs On/Off` mod setting logic.

### [ ] Task 8: Tree Felling Byproducts
- [ ] Harmony patch for tree felling to drop Green Firewood.

### [ ] Task 9: Visuals & Soot
- [ ] Implement "Soot Filth" generation for Green Firewood.
- [ ] Add beauty penalties to Green Firewood.
- [ ] Finalize placeholder textures (or integrate final art).

---

## Phase 4: Expansion & Compatibility

### [ ] Task 10: Wood Fibre Pads & Passive Coolers
- [ ] Implement Pads and patch Passive Coolers.

### [ ] Task 11: Mod Compatibility
- [ ] Medieval Overhaul patches.
- [ ] VFE patches.
