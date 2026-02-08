using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimWood
{
    /// <summary>
    /// Harmony patches for fuel consumption multipliers.
    /// Makes different fuel types contribute different amounts when refueling buildings.
    /// </summary>
    public static class FuelMultiplierSystem
    {
        /// <summary>
        /// Returns the fuel multiplier for a given ThingDef.
        /// </summary>
        public static float GetFuelMultiplier(ThingDef def)
        {
            if (def == null)
                return 1.0f;

            // WoodLog: 0.75x efficiency
            if (def.defName == "WoodLog")
                return 0.75f;

            // Green/Seasoned Firewood: 1.0x (baseline)
            if (def.defName == "RimWood_GreenFirewood" || def.defName == "RimWood_SeasonedFirewood")
                return 1.0f;

            // Charcoal: 6.0x efficiency
            if (def.defName == "RimWood_Charcoal")
                return 6.0f;

            // Default: baseline
            return 1.0f;
        }
    }

    /// <summary>
    /// Patches CompRefuelable.Refuel(List<Thing>) to track fuel type and set multiplier.
    /// </summary>
    [HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.Refuel))]
    [HarmonyPatch(new Type[] { typeof(List<Thing>) })]
    public static class CompRefuelable_RefuelList_Patch
    {
        /// <summary>
        /// Prefix that calculates and stores the fuel multiplier based on fuel items.
        /// </summary>
        [HarmonyPrefix]
        public static void Prefix(List<Thing> fuelThings)
        {
            if (fuelThings == null || fuelThings.Count == 0)
            {
                CompRefuelable_RefuelFloat_Patch.SetMultiplier(1.0f);
                return;
            }

            // Get multiplier from first fuel item (assume homogeneous fuel for simplicity)
            Thing firstFuel = fuelThings[0];
            if (firstFuel == null)
            {
                CompRefuelable_RefuelFloat_Patch.SetMultiplier(1.0f);
                return;
            }

            float multiplier = FuelMultiplierSystem.GetFuelMultiplier(firstFuel.def);
            CompRefuelable_RefuelFloat_Patch.SetMultiplier(multiplier);
        }

        /// <summary>
        /// Postfix that clears the stored multiplier.
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix()
        {
            CompRefuelable_RefuelFloat_Patch.ClearMultiplier();
        }
    }

    /// <summary>
    /// Patches CompRefuelable.Refuel(float) to apply the stored fuel multiplier.
    /// </summary>
    [HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.Refuel))]
    [HarmonyPatch(new Type[] { typeof(float) })]
    public static class CompRefuelable_RefuelFloat_Patch
    {
        // Thread-local storage for the current fuel multiplier
        [ThreadStatic]
        private static float currentMultiplier = 1.0f;

        public static void SetMultiplier(float multiplier)
        {
            currentMultiplier = multiplier;
        }

        public static void ClearMultiplier()
        {
            currentMultiplier = 1.0f;
        }

        /// <summary>
        /// Prefix that modifies the amount parameter based on stored multiplier.
        /// </summary>
        [HarmonyPrefix]
        public static void Prefix(ref float amount)
        {
            amount *= currentMultiplier;
        }
    }
}
