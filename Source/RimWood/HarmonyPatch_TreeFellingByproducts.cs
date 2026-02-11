using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimWood
{
    /// <summary>
    /// Spawns a small amount of Green Firewood as a byproduct whenever a wood-producing
    /// plant is felled. Applies to any plant with harvestTag == "Wood" (all vanilla trees
    /// and any modded tree that follows the standard convention).
    ///
    /// Drop: Max(2, Floor(harvestYield * 0.15)), placed at the felled tree's position.
    /// Position and map are cached in the Prefix because PlantCollected may destroy
    /// the plant before the Postfix runs.
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.PlantCollected))]
    public static class Plant_PlantCollected_Patch
    {
        [ThreadStatic]
        private static IntVec3 s_position;
        [ThreadStatic]
        private static Map s_map;
        [ThreadStatic]
        private static int s_firewoodAmount;

        [HarmonyPrefix]
        public static void Prefix(Plant __instance)
        {
            // Reset cache
            s_position = IntVec3.Invalid;
            s_map = null;
            s_firewoodAmount = 0;

            // Only wood-producing plants (covers all vanilla trees and modded trees
            // that follow the standard convention of harvestTag = "Wood")
            if (__instance.def?.plant?.harvestTag != "Wood")
                return;

            // Calculate firewood amount from the def's base yield
            int logYield = Mathf.RoundToInt(__instance.def.plant.harvestYield);
            s_firewoodAmount = Mathf.Max(2, Mathf.FloorToInt(logYield * 0.15f));

            // Cache position and map before PlantCollected can destroy the plant
            s_position = __instance.Position;
            s_map = __instance.Map;
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (s_map == null || !s_position.IsValid || s_firewoodAmount <= 0)
                return;

            Thing firewood = ThingMaker.MakeThing(ThingDefOf.RimWood_GreenFirewood);
            firewood.stackCount = s_firewoodAmount;
            GenPlace.TryPlaceThing(firewood, s_position, s_map, ThingPlaceMode.Near);

            // Clear cache
            s_map = null;
        }
    }
}
