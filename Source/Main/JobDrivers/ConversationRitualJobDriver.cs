using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviaRace.Helpers;
using ReviaRace.Needs;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace ReviaRace.JobDrivers
{
    public class ConversationRitualJobDriver : JobDriver
    {
        //public readonly TargetIndex iPawn = TargetIndex.A;
        public readonly TargetIndex iSacrificeBuilding = TargetIndex.A; // Any sacrifice building
        public readonly TargetIndex BloodsoneIndex = TargetIndex.B;
        public readonly TargetIndex iConverting = TargetIndex.C; // Prisoner

        public int TicksLeft { get; set; } = (int)TicksMax;
        public const float TicksMax = 300;

        public Pawn Sacrificer
        {
            get
            {
                return pawn;
            }
        }

        public Pawn ConvertingPawn
        {
            get
            {
                switch ((Thing)job.GetTarget(iConverting))
                {
                    case Pawn pawnPrisoner:
                        return pawnPrisoner;
                    default:
                        return null;
                }
            }
        }

        public Building SacrificeSpot
        {
            get
            {
                return (Building)job.GetTarget(iSacrificeBuilding);
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(ConvertingPawn, this.job, 1, -1, null, errorOnFailed) &&
                   pawn.Reserve(SacrificeSpot, this.job) &&
                   job.GetTarget(BloodsoneIndex).Thing == null || pawn.Reserve(job.GetTarget(BloodsoneIndex).Thing, this.job);
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(iSacrificeBuilding);
            this.FailOnAggroMentalState(iConverting);
            var convertingPawn = ConvertingPawn;

            foreach (Toil toil2 in JobDriver_DoBill.CollectIngredientsToils(TargetIndex.B, TargetIndex.A, TargetIndex.C, false, true, false))
            {
                yield return toil2;
            }
            Toil toil = ToilMaker.MakeToil("SetTargets");
            toil.AddFinishAction(() =>
            {
                job.SetTarget(iConverting, convertingPawn);
                job.SetTarget(BloodsoneIndex, SacrificeSpot.Map.thingGrid.ThingsListAt(SacrificeSpot.InteractionCell).FirstOrDefault(x => x.def == Defs.Bloodstone));
                this.FailOnDestroyedNullOrForbidden(BloodsoneIndex);
            });
            yield return toil;
            yield return Toils_Reserve.Reserve(BloodsoneIndex,1,-1,null);
            Toil goToTakee = Toils_Goto.GotoThing(iConverting, PathEndMode.ClosestTouch)
                .FailOnDespawnedNullOrForbidden(TargetIndex.A)
                .FailOnDespawnedNullOrForbidden(TargetIndex.C)
                .FailOn(() => !this.pawn.CanReach(this.SacrificeSpot, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.ByPawn))
                .FailOnSomeonePhysicallyInteracting(iConverting)
                ;

            yield return goToTakee;
            yield return Toils_Haul.StartCarryThing(iConverting, false, false, false, true);
            yield return Toils_Goto.GotoThing(iSacrificeBuilding, SacrificeSpot.InteractionCell);
            yield return Toils_Reserve.Release(iConverting);


            var doSacrificePrisoner = new Toil
            {
                socialMode = RandomSocialMode.Off,
            };
            doSacrificePrisoner.initAction = () =>
            {
                Sacrificer.carryTracker.TryDropCarriedThing(SacrificeSpot.InteractionCell, ThingPlaceMode.Direct, out var thing, null);
                ConvertingPawn.jobs.StartJob(JobMaker.MakeJob(Defs.PrisonerWait));
            };
            doSacrificePrisoner.AddFailCondition(() => ConvertingPawn.Dead);
            doSacrificePrisoner.defaultCompleteMode = ToilCompleteMode.Never;
            doSacrificePrisoner.AddPreTickAction(() =>
            {
                --TicksLeft;
                if (TicksLeft <= 0)
                {
                    ReadyForNextToil();
                }
            });
            yield return doSacrificePrisoner.WithProgressBar(iConverting, () => (TicksMax - (float)TicksLeft) / TicksMax);

            var afterSacrificePrisoner = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Instant,
                initAction = () =>
                {
                    ConvertingPawn.genes.SetXenotype(Defs.XenotypeDef);
                    (ConvertingPawn.health.AddHediff(HediffDef.Named("CatatonicBreakdown")) as HediffWithComps).TryGetComp<HediffComp_Disappears>().ticksToDisappear=100_000;
                    ConvertingPawn.jobs.StopAll();
                    if (job.GetTarget(TargetIndex.B).Thing.stackCount <= StaticModVariables.BloodstonesCountForConversation)
                        job.GetTarget(TargetIndex.B).Thing.Destroy(DestroyMode.Vanish);
                    else job.GetTarget(TargetIndex.B).Thing.stackCount -= StaticModVariables.BloodstonesCountForConversation;
                }
            };
            yield return afterSacrificePrisoner;


        }
    }
}
