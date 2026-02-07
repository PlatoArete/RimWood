using RimWorld;
using Verse;

namespace RimWood
{
    /// <summary>
    /// DefOf class for RimWood job definitions.
    /// Allows referencing JobDefs from C# code.
    /// </summary>
    [DefOf]
    public static class JobDefOf
    {
        public static JobDef RimWood_LoadEarthKiln;
        public static JobDef RimWood_ExtractCharcoal;

        static JobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
        }
    }
}
