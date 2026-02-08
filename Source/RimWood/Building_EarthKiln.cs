using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWood
{
    /// <summary>
    /// Earth Kiln building that converts seasoned firewood into charcoal over time.
    /// Uses a "load and wait" mechanic similar to the fermenting barrel.
    /// </summary>
    public class Building_EarthKiln : Building
    {
        // Constants
        private const int MaxCapacity = 12; // 12 seasoned firewood required
        private const int TicksToCarbonize = 360000; // 6 days (6 * 60000 ticks)
        private const int CharcoalOutput = 3; // 3 charcoal produced per batch

        // Fields
        private int firewoodCount = 0;
        private int carbonizationProgress = 0;

        // Properties
        public int SpaceLeftForFirewood => MaxCapacity - firewoodCount;
        private float ProgressPercent => carbonizationProgress / (float)TicksToCarbonize;
        public bool Finished => carbonizationProgress >= TicksToCarbonize && firewoodCount >= MaxCapacity;
        public bool Empty => firewoodCount == 0;

        /// <summary>
        /// Tick processing for carbonization progress.
        /// Uses rare tick (250 ticks) for performance.
        /// </summary>
        public override void TickRare()
        {
            base.TickRare();

            if (!Spawned)
                return;

            // Only carbonize if we have a full load
            if (firewoodCount >= MaxCapacity && carbonizationProgress < TicksToCarbonize)
            {
                carbonizationProgress += 250; // Rare tick increment
            }
        }

        /// <summary>
        /// Adds seasoned firewood to the kiln.
        /// Returns the number of firewood actually added.
        /// </summary>
        public int AddFirewood(int count)
        {
            int spaceLeft = SpaceLeftForFirewood;
            int amountToAdd = Mathf.Min(count, spaceLeft);

            if (amountToAdd <= 0)
                return 0;

            // Reset progress when starting a new batch
            if (firewoodCount == 0)
            {
                carbonizationProgress = 0;
            }

            firewoodCount += amountToAdd;
            return amountToAdd;
        }

        /// <summary>
        /// Extracts finished charcoal from the kiln.
        /// Spawns charcoal items and resets the kiln.
        /// </summary>
        public Thing TakeOutCharcoal()
        {
            if (!Finished)
            {
                Log.Error("[RimWood] Attempted to extract charcoal from unfinished kiln.");
                return null;
            }

            // Create charcoal
            Thing charcoal = ThingMaker.MakeThing(ThingDef.Named("RimWood_Charcoal"));
            charcoal.stackCount = CharcoalOutput;

            // Reset kiln
            firewoodCount = 0;
            carbonizationProgress = 0;

            return charcoal;
        }

        /// <summary>
        /// Displays kiln status in the inspection pane.
        /// Shows current load, progress percentage, and time remaining.
        /// </summary>
        public override string GetInspectString()
        {
            StringBuilder sb = new StringBuilder();
            string baseString = base.GetInspectString();
            if (!baseString.NullOrEmpty())
                sb.Append(baseString);

            string status;
            if (Empty)
            {
                status = "Empty - Load 12 seasoned firewood to begin";
            }
            else if (firewoodCount < MaxCapacity)
            {
                status = $"Loaded: {firewoodCount}/{MaxCapacity} seasoned firewood";
            }
            else if (Finished)
            {
                status = "Carbonization complete - Extract charcoal";
            }
            else
            {
                float daysRemaining = (TicksToCarbonize - carbonizationProgress) / 60000f;
                status = $"Carbonizing: {ProgressPercent:P0} ({daysRemaining:F1} days remaining)";
            }

            if (sb.Length > 0)
                sb.AppendLine();
            sb.Append(status);

            return sb.ToString().TrimEndNewlines();
        }

        /// <summary>
        /// Generates float menu options for player interaction.
        /// Allows "Load firewood" and "Extract charcoal" commands.
        /// </summary>
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption option in base.GetFloatMenuOptions(selPawn))
            {
                yield return option;
            }

            // Option to load firewood
            if (!Finished && SpaceLeftForFirewood > 0)
            {
                // Check if pawn can reach and reserve
                if (!selPawn.CanReach(this, PathEndMode.Touch, Danger.Deadly))
                {
                    yield return new FloatMenuOption("Cannot reach earth kiln", null);
                }
                else if (!selPawn.CanReserve(this))
                {
                    yield return new FloatMenuOption("Reserved by someone else", null);
                }
                else
                {
                    yield return new FloatMenuOption("Load seasoned firewood", delegate
                    {
                        Job job = JobMaker.MakeJob(JobDefOf.RimWood_LoadEarthKiln, this);
                        selPawn.jobs.TryTakeOrderedJob(job);
                    });
                }
            }

            // Option to extract charcoal
            if (Finished)
            {
                if (!selPawn.CanReach(this, PathEndMode.Touch, Danger.Deadly))
                {
                    yield return new FloatMenuOption("Cannot reach earth kiln", null);
                }
                else if (!selPawn.CanReserve(this))
                {
                    yield return new FloatMenuOption("Reserved by someone else", null);
                }
                else
                {
                    yield return new FloatMenuOption("Extract charcoal", delegate
                    {
                        Job job = JobMaker.MakeJob(JobDefOf.RimWood_ExtractCharcoal, this);
                        selPawn.jobs.TryTakeOrderedJob(job);
                    });
                }
            }
        }

        /// <summary>
        /// Persists kiln state across save/load.
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref firewoodCount, "firewoodCount", 0);
            Scribe_Values.Look(ref carbonizationProgress, "carbonizationProgress", 0);
        }
    }
}
