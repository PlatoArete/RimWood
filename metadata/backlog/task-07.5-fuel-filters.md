# Task 7.5: Per-building Fuel Filters

## User Story
As a player, I want to manually configure which fuels are accepted by specific buildings so that I can optimize my fuel usage (e.g., save charcoal for the forge and use firewood for heating).

## Acceptance Criteria
- [ ] XPath patches enable `<showFilter>true</showFilter>` for all refuelable vanilla buildings.
- [ ] Players can open a "Fuel" tab or button on buildings like Campfires, Stoves, and Smithies.
- [ ] The filter respects the global "Allow logs in fuel buildings" setting (if the setting blocks logs, they should not appear or be selectable in the filter).
- [ ] Logic confirmed for modded buildings (compatibility check).
