using Verse;

namespace RimWood
{
    /// <summary>
    /// CompProperties for the seasoning system. Defines the target ThingDef and base duration.
    /// </summary>
    public class CompProperties_Seasonable : CompProperties
    {
        /// <summary>
        /// The ThingDef to transform into when seasoning is complete (e.g., RimWood_SeasonedFirewood).
        /// </summary>
        public ThingDef targetDef;

        /// <summary>
        /// Base ticks to complete seasoning at 20°C with 1.0x method multiplier.
        /// Default: 2,400,000 ticks (40 days).
        /// </summary>
        public int baseTicksToSeason = 2400000;

        public CompProperties_Seasonable()
        {
            compClass = typeof(CompSeasonable);
        }
    }
}
