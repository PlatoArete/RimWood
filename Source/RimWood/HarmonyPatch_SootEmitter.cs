using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimWood
{
    /// <summary>
    /// Additional Postfix on CompRefuelable.Refuel(List&lt;Thing&gt;) that writes the dirty-fuel
    /// flag on the building's CompSootEmitter based on whether Green Firewood was loaded.
    ///
    /// Runs alongside CompRefuelable_RefuelList_Patch (fuel multipliers) — Harmony executes
    /// both postfixes independently and in registration order, so there is no conflict.
    ///
    /// Flag is set true  → Green Firewood was the fuel loaded.
    /// Flag is set false → Any other fuel type was loaded (clean burn, no soot).
    /// </summary>
    [HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.Refuel))]
    [HarmonyPatch(new[] { typeof(List<Thing>) })]
    public static class CompRefuelable_RefuelList_SootPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CompRefuelable __instance, List<Thing> fuelThings)
        {
            if (fuelThings == null || fuelThings.Count == 0)
                return;

            var sootEmitter = __instance.parent?.TryGetComp<CompSootEmitter>();
            if (sootEmitter == null)
                return;

            bool isGreenFirewood = fuelThings[0]?.def?.defName == "RimWood_GreenFirewood";
            sootEmitter.SetDirtyFueled(isGreenFirewood);
        }
    }
}
