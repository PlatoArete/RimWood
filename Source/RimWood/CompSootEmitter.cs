using RimWorld;
using UnityEngine;
using Verse;

namespace RimWood
{
    public class CompProperties_SootEmitter : CompProperties
    {
        public CompProperties_SootEmitter()
        {
            compClass = typeof(CompSootEmitter);
        }
    }

    /// <summary>
    /// Parallel comp that tracks whether a building was last fueled with Green Firewood
    /// and periodically spawns FilthDirt nearby while that fuel is still burning.
    ///
    /// isDirtyFueled is set/cleared by HarmonyPatch_SootEmitter when CompRefuelable.Refuel
    /// is called, based on the fuel type being loaded.
    /// Soot is emitted on TickRare while isDirtyFueled is true and the building has fuel.
    /// </summary>
    public class CompSootEmitter : ThingComp
    {
        private bool isDirtyFueled;

        // Chance per TickRare (250 ticks) to spawn one soot tile.
        // At 25% this averages one soot tile every ~1000 ticks (~16 in-game minutes).
        private const float SootChancePerTick = 0.25f;

        private static ThingDef s_filthDirt;
        private static ThingDef FilthDirtDef
        {
            get
            {
                if (s_filthDirt == null)
                    s_filthDirt = DefDatabase<ThingDef>.GetNamed("FilthDirt", errorOnFail: false);
                return s_filthDirt;
            }
        }

        public void SetDirtyFueled(bool dirty)
        {
            isDirtyFueled = dirty;
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (!isDirtyFueled)
                return;

            // Stop emitting once the building has burned through its fuel
            var refuelable = parent.TryGetComp<CompRefuelable>();
            if (refuelable != null && !refuelable.HasFuel)
                return;

            if (!Rand.Chance(SootChancePerTick))
                return;

            // Pick a random cell within 1 tile of the building footprint
            CellRect area = parent.OccupiedRect().ExpandedBy(1);
            IntVec3 cell = area.RandomCell;

            if (cell.InBounds(parent.Map))
            {
                ThingDef filth = FilthDirtDef;
                if (filth != null)
                    FilthMaker.TryMakeFilth(cell, parent.Map, filth);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref isDirtyFueled, "isDirtyFueled", false);
        }
    }
}
