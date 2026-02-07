using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimWood
{
    /// <summary>
    /// Component that handles the "reverse rot" seasoning mechanic for green firewood.
    /// Green firewood gradually transforms into seasoned firewood over time, with speed
    /// influenced by ambient temperature.
    /// </summary>
    public class CompSeasonable : ThingComp
    {
        /// <summary>
        /// Current seasoning progress in ticks.
        /// </summary>
        private int seasoningProgress = 0;

        /// <summary>
        /// Cached reference to CompProperties_Seasonable.
        /// </summary>
        public CompProperties_Seasonable Props => (CompProperties_Seasonable)props;

        /// <summary>
        /// Rare tick frequency (250 ticks) for performance optimization.
        /// </summary>
        public override void CompTickRare()
        {
            base.CompTickRare();

            // Only season if spawned in the world
            if (!parent.Spawned)
                return;

            // Calculate progress increment
            float tempMultiplier = GetTemperatureMultiplier();
            float methodMultiplier = GetMethodMultiplier();
            float progressIncrement = 250f * tempMultiplier * methodMultiplier;

            seasoningProgress += Mathf.RoundToInt(progressIncrement);

            // Check if seasoning is complete
            if (seasoningProgress >= Props.baseTicksToSeason)
            {
                TransformToSeasoned();
            }
        }

        /// <summary>
        /// Calculates temperature multiplier based on ambient temperature.
        /// Formula: (temp - 0°C) / 20°C, clamped between 0.5x and 2.0x.
        /// </summary>
        private float GetTemperatureMultiplier()
        {
            float ambientTemp = parent.AmbientTemperature;
            float multiplier = (ambientTemp - 0f) / 20f;
            return Mathf.Clamp(multiplier, 0.5f, 2.0f);
        }

        /// <summary>
        /// Gets the method multiplier based on storage location.
        /// MVP: Returns 1.0f (stockpile baseline).
        /// TODO (Task 5): Detect Woodpile (1.5x) and Woodshed (2.5x).
        /// </summary>
        private float GetMethodMultiplier()
        {
            // MVP: Stockpile only
            return 1.0f;
        }

        /// <summary>
        /// Transforms the green firewood into seasoned firewood.
        /// Uses destroy/spawn approach to preserve stack count and position.
        /// </summary>
        private void TransformToSeasoned()
        {
            if (Props.targetDef == null)
            {
                Log.Error($"[RimWood] CompSeasonable on {parent.def.defName} has no targetDef configured.");
                return;
            }

            Map map = parent.Map;
            IntVec3 position = parent.Position;
            int stackCount = parent.stackCount;

            // Create seasoned firewood
            Thing seasonedFirewood = ThingMaker.MakeThing(Props.targetDef);
            seasonedFirewood.stackCount = stackCount;

            // Preserve hit points ratio
            if (parent.def.useHitPoints && Props.targetDef.useHitPoints)
            {
                float hpRatio = (float)parent.HitPoints / (float)parent.MaxHitPoints;
                seasonedFirewood.HitPoints = Mathf.RoundToInt(seasonedFirewood.MaxHitPoints * hpRatio);
            }

            // Remove green firewood
            parent.Destroy(DestroyMode.Vanish);

            // Spawn seasoned firewood
            GenSpawn.Spawn(seasonedFirewood, position, map);

            // Optional: Send message to player
            Messages.Message(
                $"{stackCount}x {parent.def.label} has finished seasoning.",
                new LookTargets(seasonedFirewood),
                MessageTypeDefOf.PositiveEvent,
                historical: false
            );
        }

        /// <summary>
        /// Displays seasoning progress in the inspection pane.
        /// Shows percentage and estimated days remaining.
        /// </summary>
        public override string CompInspectStringExtra()
        {
            if (Props.targetDef == null)
                return null;

            float progressPercent = ((float)seasoningProgress / (float)Props.baseTicksToSeason) * 100f;
            float ticksRemaining = Props.baseTicksToSeason - seasoningProgress;

            // Calculate days remaining based on current conditions
            float tempMultiplier = GetTemperatureMultiplier();
            float methodMultiplier = GetMethodMultiplier();
            float effectiveTickRate = 250f * tempMultiplier * methodMultiplier;

            float daysRemaining = 0f;
            if (effectiveTickRate > 0)
            {
                float totalTicksRemaining = ticksRemaining;
                daysRemaining = totalTicksRemaining / 60000f; // 60,000 ticks per day
            }

            return $"Seasoning: {progressPercent:F1}% ({daysRemaining:F1} days remaining)";
        }

        /// <summary>
        /// Handles stack splitting by copying seasoning progress to the split piece.
        /// </summary>
        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);

            // The split piece gets a copy of this comp with the same progress
            CompSeasonable otherComp = piece.TryGetComp<CompSeasonable>();
            if (otherComp != null)
            {
                otherComp.seasoningProgress = this.seasoningProgress;
            }
        }

        /// <summary>
        /// Persists seasoning progress across save/load.
        /// </summary>
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref seasoningProgress, "seasoningProgress", 0);
        }
    }
}
