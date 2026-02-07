using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimWood
{
    /// <summary>
    /// Harmony patches for RimWood mod.
    /// Prevents deterioration of items that are actively seasoning.
    /// </summary>
    [HarmonyPatch(typeof(SteadyEnvironmentEffects), nameof(SteadyEnvironmentEffects.FinalDeteriorationRate))]
    [HarmonyPatch(new Type[] { typeof(Thing), typeof(List<string>) })]
    public static class SteadyEnvironmentEffects_FinalDeteriorationRate_Patch
    {
        /// <summary>
        /// Postfix patch that cancels deterioration for items actively seasoning.
        /// Logic: If item has CompSeasonable and is actively seasoning (roofed),
        /// set deterioration rate to 0 to prevent HP loss during the seasoning process.
        /// Targets the 2-parameter public entry point method added in RimWorld 1.6.
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(Thing t, List<string> reasons, ref float __result)
        {
            // Check if thing has seasoning comp
            CompSeasonable comp = t.TryGetComp<CompSeasonable>();
            if (comp == null)
                return;

            // If actively seasoning (method multiplier > 0, meaning roofed/sheltered),
            // cancel deterioration
            if (comp.GetMethodMultiplier() > 0f)
            {
                __result = 0f;
            }

            // If method multiplier is 0 (unroofed/stalled), allow normal deterioration
            // This ensures wood left exposed will "rot" as intended by the design
        }
    }
}
