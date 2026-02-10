using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimWood
{
    /// <summary>
    /// Hooks WorkGiver_Refuel.JobOnThing so the per-building CompFuelFilter is consulted
    /// when a pawn searches for fuel to haul to a building.
    ///
    /// Pattern: ThreadStatic reference to the current building's CompFuelFilter is set in
    /// a Prefix before the WorkGiver searches for fuel, and cleared in the Postfix.
    /// A second patch on ThingFilter.Allows then checks the per-building filter while the
    /// WorkGiver is actively looking, so only accepted fuel types will be considered.
    /// </summary>
    [HarmonyPatch(typeof(WorkGiver_Refuel), nameof(WorkGiver_Refuel.JobOnThing))]
    public static class WorkGiver_Refuel_JobOnThing_Patch
    {
        [ThreadStatic]
        private static CompFuelFilter s_currentFilter;

        // Exposed so ThingFilter_Allows_PerBuilding_Patch can read it.
        internal static CompFuelFilter CurrentFilter => s_currentFilter;

        [HarmonyPrefix]
        public static void Prefix(Thing t)
        {
            s_currentFilter = t?.TryGetComp<CompFuelFilter>();
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            s_currentFilter = null;
        }
    }

    /// <summary>
    /// Adds a per-building filter check on top of the existing CompRefuelable fuel filter.
    /// Only active while WorkGiver_Refuel.JobOnThing is on the call stack (ThreadStatic guard).
    ///
    /// Calls Allows(ThingDef) — not Allows(Thing) — to avoid re-entering this patch.
    /// The global "Allow logs" hard-stop is handled separately in HarmonyPatch_WoodLogFilter.cs
    /// and remains effective regardless of the per-building filter setting.
    /// </summary>
    [HarmonyPatch(typeof(ThingFilter), nameof(ThingFilter.Allows), new[] { typeof(Thing) })]
    public static class ThingFilter_Allows_PerBuilding_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Thing t, ref bool __result)
        {
            if (!__result)
                return;

            CompFuelFilter comp = WorkGiver_Refuel_JobOnThing_Patch.CurrentFilter;
            if (comp == null)
                return;

            // Use the ThingDef overload to avoid re-triggering this Thing overload patch.
            if (!comp.FuelFilter.Allows(t.def))
                __result = false;
        }
    }
}
