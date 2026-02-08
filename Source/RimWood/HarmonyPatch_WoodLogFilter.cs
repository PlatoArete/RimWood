using HarmonyLib;
using RimWorld;
using Verse;

namespace RimWood
{
    /// <summary>
    /// Harmony patch to dynamically filter WoodLog from fuel based on mod settings.
    /// When "Allow logs in fuel buildings" is disabled, WoodLog is rejected as fuel.
    /// Patches ThingFilter.Allows which is used by fuel filters to determine valid items.
    /// </summary>
    [HarmonyPatch(typeof(ThingFilter), nameof(ThingFilter.Allows), new[] { typeof(Thing) })]
    public static class ThingFilter_Allows_Patch
    {
        /// <summary>
        /// Postfix that blocks WoodLog from being accepted as fuel if the mod setting disallows it.
        /// Only applies to refuelable building fuel filters.
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(ThingFilter __instance, Thing t, ref bool __result)
        {
            // Only proceed if the vanilla check already passed
            if (!__result)
                return;

            // Check if this is WoodLog
            if (t?.def?.defName != "WoodLog")
                return;

            // Check mod setting
            if (RimWoodModController.settings == null || RimWoodModController.settings.allowLogsInBuildings)
                return; // Setting is ON or not loaded - allow logs

            // Check if this filter belongs to a refuelable comp
            // We need to be careful not to block WoodLog in ALL filters, only fuel filters
            // The best way is to check if this filter allows our RimWood fuel categories
            // If it does, it's likely a fuel filter we patched
            if (__instance.Allows(ThingDefOf.RimWood_SeasonedFirewood) ||
                __instance.Allows(ThingDefOf.RimWood_Charcoal))
            {
                // This is a fuel filter that we've patched - block WoodLog
                __result = false;
            }
        }
    }
}
