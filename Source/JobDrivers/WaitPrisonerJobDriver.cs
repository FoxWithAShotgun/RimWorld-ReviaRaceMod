using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace ReviaRace.JobDrivers
{
    public class WaitPrisonerJobDriver : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true; // No reservations.
        }

        private Pawn Prisoner => (Pawn)TargetA;
        private TargetIndex PrisonerIndex => TargetIndex.A;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(PrisonerIndex);
            this.FailOn(() => Prisoner.Dead);

            yield return new Toil
            {
                initAction = () =>
                {
                    Prisoner.Reserve(pawn.Position, job);
                    pawn.pather.StopDead();
                    var driver = pawn.jobs.curDriver;
                    pawn.jobs.posture = PawnPosture.LayingOnGroundFaceUp;
                    driver.asleep = false;
                },
                defaultCompleteMode = ToilCompleteMode.Never,
            };
        }
    }
}
