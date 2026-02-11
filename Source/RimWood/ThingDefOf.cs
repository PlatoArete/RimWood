using RimWorld;
using Verse;

namespace RimWood
{
    /// <summary>
    /// DefOf class for RimWood thing definitions.
    /// Allows referencing ThingDefs from C# code with compile-time safety.
    /// </summary>
    [DefOf]
    public static class ThingDefOf
    {
        public static ThingDef RimWood_GreenFirewood;
        public static ThingDef RimWood_SeasonedFirewood;
        public static ThingDef RimWood_Charcoal;

        static ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
        }
    }
}
