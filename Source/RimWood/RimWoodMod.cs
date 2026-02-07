using HarmonyLib;
using Verse;

namespace RimWood
{
    /// <summary>
    /// Main mod class that initializes Harmony patches when the mod loads.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class RimWoodMod
    {
        static RimWoodMod()
        {
            // Apply all Harmony patches for the RimWood mod
            var harmony = new Harmony("platoarete.rimwood");
            harmony.PatchAll();

            Log.Message("[RimWood] Harmony patches applied successfully.");
        }
    }
}
