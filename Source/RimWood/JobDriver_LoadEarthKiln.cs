using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimWood
{
    /// <summary>
    /// Job driver for hauling seasoned firewood to the earth kiln.
    /// Pawn will find seasoned firewood, haul it to the kiln, and load it.
    /// </summary>
    public class JobDriver_LoadEarthKiln : JobDriver
    {
        private const TargetIndex KilnInd = TargetIndex.A;
        private const TargetIndex FirewoodInd = TargetIndex.B;

        private Building_EarthKiln Kiln => (Building_EarthKiln)job.GetTarget(KilnInd).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            // Reserve the kiln
            if (!pawn.Reserve(Kiln, job, 1, -1, null, errorOnFailed))
                return false;

            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // Fail conditions
            this.FailOnDespawnedNullOrForbidden(KilnInd);
            this.FailOn(() => Kiln.SpaceLeftForFirewood <= 0);

            // Find and reserve firewood
            yield return Toils_General.Do(delegate
            {
                // Find seasoned firewood in storage
                Thing firewood = GenClosest.ClosestThingReachable(
                    pawn.Position,
                    pawn.Map,
                    ThingRequest.ForDef(ThingDef.Named("RimWood_SeasonedFirewood")),
                    PathEndMode.ClosestTouch,
                    TraverseParms.For(pawn),
                    9999f,
                    validator: (Thing t) => !t.IsForbidden(pawn) && pawn.CanReserve(t)
                );

                if (firewood == null)
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                // Calculate how much to take
                int countToTake = UnityEngine.Mathf.Min(Kiln.SpaceLeftForFirewood, firewood.stackCount);
                job.count = countToTake;
                job.SetTarget(FirewoodInd, firewood);

                // Reserve the firewood
                pawn.Reserve(firewood, job, 1, countToTake);
            });

            // Go to firewood
            yield return Toils_Goto.GotoThing(FirewoodInd, PathEndMode.ClosestTouch)
                .FailOnDespawnedNullOrForbidden(FirewoodInd);

            // Pick up firewood
            yield return Toils_Haul.StartCarryThing(FirewoodInd, putRemainderInQueue: false, subtractNumTakenFromJobCount: true);

            // Go to kiln
            yield return Toils_Goto.GotoThing(KilnInd, PathEndMode.Touch);

            // Load firewood into kiln
            yield return Toils_General.Wait(200).WithProgressBarToilDelay(KilnInd);

            yield return new Toil
            {
                initAction = delegate
                {
                    Thing carriedThing = pawn.carryTracker.CarriedThing;
                    if (carriedThing != null)
                    {
                        int countAdded = Kiln.AddFirewood(carriedThing.stackCount);

                        if (countAdded > 0)
                        {
                            carriedThing.stackCount -= countAdded;
                            if (carriedThing.stackCount <= 0)
                            {
                                carriedThing.Destroy();
                            }
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
