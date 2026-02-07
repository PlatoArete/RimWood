# Task 4: Reverse-Rot Seasoning System (C#)

## User Story
As a player, I want Green Firewood to eventually turn into Seasoned Firewood while sitting in a stockpile so that I don't always need specialized buildings.

## Acceptance Criteria
- [ ] C# assembly targets **.NET Framework 4.8** and loads without errors.
- [ ] `CompProperties_Seasonable` defined with `targetDef` and `baseTicksToSeason` (2,400,000).
- [ ] `CompSeasonable` implements `CompTickRare` for performance-friendly progress tracking.
- [ ] Seasoning progress persists through save/load via `PostExposeData`.
- [ ] Speed scales via `Mathf.Clamp((temp - 0f) / 20f, 0.5f, 2.0f)`.
- [ ] Inspection string shows seasoning percentage and estimated days remaining.
- [ ] Merging stacks averages seasoning progress based on stack count.
- [ ] Green Firewood transforms into Seasoned Firewood (1:1 ratio) upon 100% progress.
- [ ] `GetMethodMultiplier()` placeholder returns 1.0f (Stockpile MVP).
