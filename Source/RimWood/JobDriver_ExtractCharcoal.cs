using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimWood
{
    /// <summary>
    /// Job driver for extracting finished charcoal from the earth kiln.
    /// Pawn will go to the kiln, extract charcoal, and haul it to storage.
    /// </summary>
    public class JobDriver_ExtractCharcoal : JobDriver
    {
        private const TargetIndex KilnInd = TargetIndex.A;

        private Building_EarthKiln Kiln => (Building_EarthKiln)job.GetTarget(KilnInd).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(Kiln, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // Fail conditions
            this.FailOnDespawnedNullOrForbidden(KilnInd);
            this.FailOn(() => !Kiln.Finished);

            // Go to kiln
            yield return Toils_Goto.GotoThing(KilnInd, PathEndMode.Touch);

            // Extract charcoal (with animation delay)
            yield return Toils_General.Wait(200).WithProgressBarToilDelay(KilnInd);

            // Take out charcoal and carry it
            yield return new Toil
            {
                initAction = delegate
                {
                    Thing charcoal = Kiln.TakeOutCharcoal();
                    if (charcoal != null)
                    {
                        GenPlace.TryPlaceThing(charcoal, pawn.Position, pawn.Map, ThingPlaceMode.Near);

                        // Start hauling to stockpile
                        StoragePriority storagePriority = StoreUtility.CurrentStoragePriorityOf(charcoal);
                        IntVec3 storeCell;
                        if (StoreUtility.TryFindBestBetterStoreCellFor(charcoal, pawn, pawn.Map, storagePriority, pawn.Faction, out storeCell))
                        {
                            job.count = charcoal.stackCount;
                            job.SetTarget(TargetIndex.B, charcoal);
                            job.SetTarget(TargetIndex.C, storeCell);
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };

            // Pick up the charcoal
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);

            // Reserve storage space
            yield return Toils_Reserve.Reserve(TargetIndex.C);

            // Go to storage
            yield return Toils_Haul.CarryHauledThingToContainer();

            // Place in storage
            yield return Toils_Haul.DepositHauledThingInContainer(TargetIndex.C, TargetIndex.None);
        }
    }
}
