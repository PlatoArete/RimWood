# RimWood — Design Document

**Mod Name:** RimWood
**Game:** RimWorld
**Author:** Andy (OpenClaw dev team)
**Version:** Pre-Alpha / Design Phase
**Last Updated:** 2026-02-06

---

## Vision

Improve the realism and gameplay of fires and fuel in RimWorld without micromanagement or undermining early-game pressure. The mod introduces a fuel processing chain that rewards planning and infrastructure investment while keeping the early game scrappy and survival-focused.

**Core Philosophy:** Efficiency over gatekeeping. The system rewards engagement but never punishes players who want a simpler experience.

---

## Materials

### Fuel Products

| Material | Source | Burn Efficiency | Stack Priority | Notes |
|---|---|---|---|---|
| Whole Logs (Wood) | Tree felling | Very low (high consumption) | Lowest | Emergency fuel only. Vanilla wood item. |
| Green Firewood | Tree felling byproduct + chopping block | Baseline (= vanilla wood rates) | Medium | Unseasoned. Beauty penalty + soot filth in radius. Same burn rate as seasoned but with penalties. |
| Seasoned Firewood | Seasoning from green firewood | Baseline (= vanilla wood rates) | Medium | Clean-burning, no penalties. The intended standard fuel. Reward is cleanliness, not longer burn. |
| Charcoal | Earth kiln process | Very high | Highest | 12 seasoned firewood → 3 charcoal (some firewood consumed as process fuel). 3 charcoal = 18 firewood equivalent burn time. Light weight, high stack size — caravan advantage. |

### Other Products

| Material | Source | Purpose | Notes |
|---|---|---|---|
| Wood Fibre Pads | Chopping block (logs → pads) | Passive cooler fuel | Replaces raw wood in passive coolers. Evaporative cooling medium. |

### Stack Sizes & Weight

Heaviest/bulkiest → lightest/most compact:

1. Logs (Wood) — vanilla values
2. Green Firewood / Seasoned Firewood / Wood Fibre Pads
3. Charcoal — lightest, highest stack size (caravan trade advantage)

### Market Values

Market values should reflect labour and time invested. Seasoned firewood and charcoal become decent trade goods, giving wood-rich forest biomes an export economy.

---

## Buildings

### Chopping Block (Stump)

- **Cost:** 1 wood log
- **Purpose:** Central wood processing hub
- **Recipes:**
  - Logs → Green Firewood
  - Logs → Wood Fibre Pads
- **Research:** None (known from start)
- **Notes:** Low-tech workstation. Visually a tree stump. Available from minute one for tribal starts.

### Woodpile

- **Cost:** 20 green firewood (the material IS the building)
- **Purpose:** Mid-tier firewood seasoning
- **Placement:** Outdoors only, no roof required
- **Mechanic:** Construction stacks 20 green firewood into a pile. Seasoning timer runs. When complete, deconstructing/harvesting yields 20 seasoned firewood. Base seasoning time: ~26 days at 20°C (1.5x speed multiplier vs stockpile).
- **Research:** None (known from start)
- **Seasoning Speed:** 1.5x (proper stacking provides airflow)

### Woodshed

- **Cost:** Wood logs (for the frame/structure)
- **Purpose:** Top-tier firewood seasoning + storage container
- **Placement:** Outdoors only
- **Mechanic:** Storage building. Green firewood stored inside seasons into seasoned firewood. Uses the "reverse rot" ticker system. Fastest seasoning method.
- **Research:** Complex Furniture
- **Seasoning Speed:** 2.5x — ~16 days at 20°C (roughly one quadrum). Fastest method.

### Earth Kiln

- **Cost:** Stone blocks
- **Purpose:** Charcoal production
- **Mechanic:** Mech gestator-style production. Load seasoned firewood, kiln processes over time, collect charcoal. Slow but not labour-intensive (no pawn babysitting). Some of the input firewood is consumed as process fuel — this is already factored into the 12 → 3 ratio.
- **Input/Output:** 12 seasoned firewood → 3 charcoal
- **Research:** Smithing

---

## Seasoning System — "Reverse Rot"

Green firewood seasons into seasoned firewood using a `CompProperties_Rottable`-style ticker running in reverse.

### Seasoning Speed by Method

| Method | Speed Multiplier | Effective Time (at 20°C) | Notes |
|---|---|---|---|
| Stockpile (roofed, outdoors) | 1.0x | 40 days | Zero material cost. The "I'm busy surviving" option. |
| Woodpile | 1.5x | ~26 days | Built from green firewood. Proper stacking provides airflow. |
| Woodshed | 2.5x | ~16 days (~1 quadrum) | Storage building. Sheltered + ventilated. Best method. |

### Temperature Scaling

**Formula:** `Temperature Multiplier = (temp - 0°C) / 20°C`, clamped between 0.5x and 2.0x.

- Optimal range: 10–25°C
- Below 0°C: Seasoning slows to 0.5x (stalls but never fully stops)
- Hot/dry biomes: Up to 2.0x cap
- Example: At 40°C in a woodshed, effective time = ~8 days

Final seasoning time = Base 40 days / (Method Multiplier × Temperature Multiplier)

---

## Fuel Assignments by Building

| Building | Accepted Fuels | Notes |
|---|---|---|
| Torches | Green Firewood, Seasoned Firewood | Green causes beauty penalty + soot filth |
| Braziers | Green Firewood, Seasoned Firewood | Green causes beauty penalty + soot filth |
| Campfires | Logs, Green Firewood, Seasoned Firewood | Logs accepted as last-resort early-game fuel (very high consumption) |
| Fuelled Stoves | Green Firewood, Seasoned Firewood, Charcoal | Charcoal most efficient. Logs allowed if "Logs On" setting enabled (default: on) |
| Fuelled Smithy | Green Firewood, Seasoned Firewood, Charcoal | Charcoal most efficient |
| Passive Coolers | Wood Fibre Pads | Dedicated cooling material, separate from fire economy |

### Green Firewood Penalties

- **Beauty penalty** in surrounding tiles (smoky, unpleasant)
- **Soot filth** spawns on nearby tiles (similar to butchering/cooking filth)
- Penalties are mild — soft incentive to upgrade, not punishment

---

## Research Tree

Minimal footprint. Most of the system is available immediately.

| Technology | Prerequisites | Unlocks | Tier |
|---|---|---|---|
| *(None needed)* | — | Green Firewood, Seasoned Firewood, Chopping Block, Woodpile | Available from start |
| Passive Cooler | — | Wood Fibre Pads | Neolithic |
| Complex Furniture | — | Woodshed | Neolithic |
| Smithing | — | Earth Kiln, Charcoal | Medieval |

**Note:** Passive coolers are confirmed behind research in vanilla ([wiki reference](https://rimworldwiki.com/wiki/Passive_cooler)), so Wood Fibre Pads unlock naturally alongside them.

**Mod Compatibility Note:** Some mods add research for very early technologies. May need compatibility patches to hook Chopping Block / Woodpile into those trees for mods that expect it.

---

## Tree Felling Byproducts

When a pawn fells a tree:

- **Primary yield:** Wood logs (vanilla, unchanged)
- **Byproduct yield:** 15% of log yield, rounded down. Minimum 2 green firewood per tree.

### Examples

| Tree | Log Yield | Green Firewood (15%) |
|---|---|---|
| Pine | 27 logs | 4 green firewood |
| Oak | 46 logs | 6 green firewood |
| Small bush | ~8 logs | 2 green firewood (minimum) |

This ensures players have *something* burnable from day one without needing the chopping block first, scaling naturally with tree size.

---

## Trader Integration

Any trader who currently sells wood in their stock generator will also sell:

- Seasoned Firewood
- Wood Fibre Pads

This ensures ice sheet / sea ice colonies can access the fuel chain through trade, same as they currently buy wood.

---

## Mod Settings

| Setting | Default | Description |
|---|---|---|
| Allow logs in workbenches | ON | When ON, fuelled stoves/smithies accept logs (at poor efficiency). When OFF, forces full fuel chain engagement. |

**Philosophy:** "Logs On" as default. The mod is about efficiency, not gatekeeping. Players who want the full experience can toggle logs off.

---

## Mod Compatibility Strategy

- **XML Patching:** Use XPath patches for all vanilla building modifications (campfires, torches, stoves, smithies, passive coolers). Avoid direct def replacements.
- **ThingCategoryDef:** Define a `ThingCategoryDef` for processable wood products so other mods can register their plank/wood items into the RimWood processing chain.
- **Flammability:** All new items use same flammability as vanilla wood. No special fire behaviour — vanilla systems handle risk naturally.

### Priority Compatibility Targets (Post-MVP)

- **Medieval Overhaul** — Plank integration. MO's plank creation could produce green firewood as byproduct. Primary inspiration for this mod.
- **Vanilla Furniture Expanded** — Fireplace fuel compatibility
- **Other fireplace/hearth mods** — Fuel assignment patches
- **Mods with plank systems** — Hook into ThingCategoryDef for cross-mod firewood production

---

## Scope & Phasing

### MVP (v1.0)

- All three fuel materials (Green Firewood, Seasoned Firewood, Charcoal)
- All four buildings (Chopping Block, Woodpile, Woodshed, Earth Kiln)
- Fuel assignments for vanilla buildings
- Reverse-rot seasoning system with temperature scaling
- Tree felling byproduct (green firewood)
- Trader integration
- Mod settings (logs on/off)
- Placeholder art

### Phase 2

- Wood Fibre Pads + passive cooler integration
- Medieval Overhaul compatibility (plank → firewood)
- Other mod compatibility patches
- Proper art assets (artist needed)

### Stretch Goals

- Other mods' plank systems producing firewood
- Charcoal as ingredient in smithing recipes (not just fuel)
- Expanded mod settings

---

## Art Requirements

All items and buildings need textures. Placeholder art for MVP, with plans to commission or recruit an artist.

- **Green Firewood** — rough-cut wood pieces, possibly with bark/leaf bits
- **Seasoned Firewood** — lighter coloured, dry-looking clean split logs
- **Charcoal** — dark/black chunks
- **Wood Fibre Pads** — fibrous mat/pad texture
- **Chopping Block** — tree stump with axe marks
- **Woodpile** — stacked firewood (outdoor structure)
- **Woodshed** — small roofed open-sided structure
- **Earth Kiln** — dome/mound shaped clay structure

---

## Open Questions

- Charcoal as smithing recipe ingredient (stretch goal) — e.g. "smelt steel requires X charcoal as flux/fuel ingredient" rather than just charcoal being a better fuel in the workbench. This would make charcoal a crafting material, not just a fuel source. Complex to implement as it touches other mods' recipes.
- Earth Kiln processing time — how many days/hours to convert 12 firewood into 3 charcoal?
- Exact stone block cost for Earth Kiln construction
- Exact wood log cost for Woodshed construction
- Chopping block work speed — how long to process logs into green firewood?