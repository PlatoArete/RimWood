using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimWood
{
    /// <summary>
    /// Harmony patch for fuel consumption multipliers.
    /// Patches JobDriver_Refuel to adjust fuel amount based on fuel type before refueling.
    /// </summary>
    [HarmonyPatch(typeof(JobDriver_Refuel), "MakeNewToils")]
    public static class JobDriver_Refuel_MakeNewToils_Patch
    {
        /// <summary>
        /// Transpiler would be complex, so we use a simpler approach:
        /// Patch Refuel(float) to track the last fuel Thing and apply multiplier.
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(JobDriver_Refuel __instance)
        {
            // This approach won't work well - we need to patch at the exact consumption point
            // Let's use a different strategy
        }
    }

    /// <summary>
    /// Patches CompRefuelable.Refuel(float) to apply fuel multipliers.
    /// We store the fuel ThingDef in a static context so we can apply the correct multiplier.
    /// </summary>
    [HarmonyPatch(typeof(CompRefuelable), nameof(CompRefuelable.Refuel), new[] { typeof(float) })]
    public static class CompRefuelable_RefuelFloat_Patch
    {
        // Thread-local storage for the current fuel being used
        [ThreadStatic]
        private static ThingDef currentFuelDef;

        public static void SetCurrentFuel(ThingDef def)
        {
            currentFuelDef = def;
        }

        public static void ClearCurrentFuel()
        {
            currentFuelDef = null;
        }

        /// <summary>
        /// Prefix that modifies the amount parameter based on fuel multiplier.
        /// </summary>
        [HarmonyPrefix]
        public static void Prefix(ref float amount)
        {
            if (currentFuelDef == null)
                return;

            float multiplier = GetFuelMultiplier(currentFuelDef);
            amount *= multiplier;
        }

        /// <summary>
        /// Returns the fuel multiplier for a given ThingDef.
        /// </summary>
        private static float GetFuelMultiplier(ThingDef def)
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
    /// Patches where fuel Things are consumed to track their ThingDef.
    /// This is a workaround since we need to know what fuel type is being used.
    /// </summary>
    [HarmonyPatch(typeof(Toils_Refuel), nameof(Toils_Refuel.FinalizeRefueling))]
    public static class Toils_Refuel_FinalizeRefueling_Patch
    {
        /// <summary>
        /// Prefix that captures the fuel ThingDef before refueling happens.
        /// </summary>
        [HarmonyPrefix]
        public static void Prefix(Pawn actor, JobDriver jobDriver)
        {
            // Get the fuel from the job's carried thing
            if (actor?.carryTracker?.CarriedThing != null)
            {
                CompRefuelable_RefuelFloat_Patch.SetCurrentFuel(actor.carryTracker.CarriedThing.def);
            }
        }

        /// <summary>
        /// Postfix that clears the stored fuel ThingDef.
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix()
        {
            CompRefuelable_RefuelFloat_Patch.ClearCurrentFuel();
        }
    }
}
