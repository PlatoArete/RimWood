using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace RimWood
{
    public class CompProperties_FuelFilter : CompProperties
    {
        public CompProperties_FuelFilter()
        {
            compClass = typeof(CompFuelFilter);
        }
    }

    /// <summary>
    /// Per-building fuel filter component.
    /// Stores a ThingFilter that lets players restrict which fuels a specific building accepts.
    /// The comp is a parallel companion to CompRefuelable — it never replaces it.
    /// </summary>
    public class CompFuelFilter : ThingComp
    {
        private ThingFilter fuelFilter = new ThingFilter();
        private bool initialized;

        public ThingFilter FuelFilter => fuelFilter;

        /// <summary>
        /// On first placement (not a load), copy the building's CompRefuelable fuel filter
        /// as the starting allowance so everything the building supports is allowed by default.
        /// </summary>
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!initialized)
            {
                var refuelable = parent.TryGetComp<CompRefuelable>();
                if (refuelable?.Props?.fuelFilter != null)
                    fuelFilter.CopyAllowancesFrom(refuelable.Props.fuelFilter);
                initialized = true;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref initialized, "initialized", false);
            Scribe_Deep.Look(ref fuelFilter, "fuelFilter");
            if (fuelFilter == null)
                fuelFilter = new ThingFilter();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action
            {
                defaultLabel = "Fuel filter",
                defaultDesc = "Configure which fuels this building will accept.",
                icon = TexCommand.ForbidOff,
                action = () =>
                {
                    var refuelable = parent.TryGetComp<CompRefuelable>();
                    ThingFilter parentFilter = refuelable?.Props?.fuelFilter;
                    Find.WindowStack.Add(new Dialog_FuelFilter(fuelFilter, parentFilter));
                }
            };
        }
    }
}
