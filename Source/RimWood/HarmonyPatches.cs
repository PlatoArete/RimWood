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
    [HarmonyPatch(new Type[] { typeof(Thing), typeof(bool), typeof(bool), typeof(TerrainDef), typeof(List<string>) })]
    public static class SteadyEnvironmentEffects_FinalDeteriorationRate_Patch
    {
        /// <summary>
        /// Postfix patch that cancels deterioration for items actively seasoning.
        /// Logic: If item has CompSeasonable and is actively seasoning (roofed),
        /// set deterioration rate to 0 to prevent HP loss during the seasoning process.
        /// Targets the 5-parameter overload which is where actual deterioration calculation occurs.
        /// Both the 2-parameter wrapper and direct calls flow through this method.
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(Thing t, ref float __result)
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

    /// <summary>
    /// Suppresses the "Deteriorating: 0 / day" inspection line when items are actively seasoning.
    /// Targets the 2-parameter display entry point to clear the reasons list.
    /// </summary>
    [HarmonyPatch(typeof(SteadyEnvironmentEffects), nameof(SteadyEnvironmentEffects.FinalDeteriorationRate))]
    [HarmonyPatch(new Type[] { typeof(Thing), typeof(List<string>) })]
    public static class SteadyEnvironmentEffects_FinalDeteriorationRate_Display_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Thing t, List<string> reasons, ref float __result)
        {
            // Clear reasons list to suppress "Deteriorating: Outdoors (0/day)" inspect line
            // when seasoning is active and deterioration is already zeroed by our 5-param patch
            if (__result == 0f && t.TryGetComp<CompSeasonable>()?.GetMethodMultiplier() > 0f)
            {
                reasons?.Clear();
            }
        }
    }
}
