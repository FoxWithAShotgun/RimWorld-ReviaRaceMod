using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviaRace.Helpers;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace ReviaRace.JobDrivers
{
    public class SacrificePrisonerJobDriver : JobDriver
    {
        public readonly TargetIndex iPawn = TargetIndex.A;
        public readonly TargetIndex iSacrificeBuilding = TargetIndex.B; // Any sacrifice building
        public readonly TargetIndex iPrisoner = TargetIndex.C; // Prisoner

        public int TicksLeft { get; set; } = (int)TicksMax;
        public const float TicksMax = 300;

        public Pawn Sacrificer
        {
            get
            {
                switch ((Thing)job.GetTarget(iPawn))
                {
                    case Pawn pawnSacrificer:
                        return pawnSacrificer;
                    default:
                        return null;
                }
            }
        }

        public Pawn Prisoner
        {
            get
            {
                switch((Thing)job.GetTarget(iPrisoner))
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
            return pawn.Reserve(Prisoner, job, 1, -1, null, errorOnFailed) &&
                   pawn.Reserve(SacrificeSpot, job, 1, -1, null);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.job.count = 1;

            this.FailOnDestroyedOrNull(iPrisoner);
            this.FailOnDestroyedOrNull(iSacrificeBuilding);
            this.FailOnAggroMentalState(iPrisoner);
            
            yield return Toils_Goto.GotoThing(iPrisoner, PathEndMode.OnCell);
            yield return Toils_Haul.StartCarryThing(iPrisoner, true, false);
            yield return Toils_Goto.GotoThing(iSacrificeBuilding, SacrificeSpot.InteractionCell);

            var dropSacrificePrisoner = Toils_Reserve.Release(iPrisoner);
            yield return dropSacrificePrisoner;

            var doSacrificePrisoner = new Toil
            {
                socialMode = RandomSocialMode.Off,
            };
            doSacrificePrisoner.AddFailCondition(() => Prisoner.Dead);
            doSacrificePrisoner.defaultCompleteMode = ToilCompleteMode.Never;
            doSacrificePrisoner.AddPreTickAction(() =>
            {
                --TicksLeft;
                if (TicksLeft <= 0)
                {
                    ReadyForNextToil();
                }
            });
            yield return doSacrificePrisoner.WithProgressBar(iPawn, () => (float)TicksLeft / TicksMax);
            yield return Toils_Reserve.Release(iSacrificeBuilding);
            var afterSacrificePrisoner = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Instant,
                initAction = () =>
                {
                    var heartBpr = Prisoner.RaceProps.body.AllParts.Find(bpr => bpr.def.defName == "Heart");
                    var dInfo = new DamageInfo(Defs.HeartExtraction, 99999f, armorPenetration: 999f, hitPart: heartBpr);
                    Prisoner.Kill(dInfo);
                    ThoughtUtility.GiveThoughtsForPawnExecuted(Prisoner, PawnExecutionKind.GenericBrutal);

                    // Apply negative relations modifier.
                    if (pawn.Faction != null && Prisoner.Faction != null)
                    {
                        Faction playerFaction = pawn.Faction;
                        Faction prisonerFaction = Prisoner.Faction;
                        string reason = "GoodwillChangedReason_RemovedBodyPart".Translate(heartBpr.Label);
                        GlobalTargetInfo? LookTargets = pawn;
                        playerFaction.TryAffectGoodwillWith(prisonerFaction, -25);
                    }

                    // Apply thoughts to pawns.
                    foreach (var mapPawn in Map.mapPawns.FreeColonistsAndPrisoners)
                    {
                        MemoryThoughtHandler memoryHandler = new MemoryThoughtHandler(mapPawn);
                        if (mapPawn.IsPrisoner)
                        {
                            memoryHandler.TryGainMemory(Defs.SacrificedFear);
                        }
                        else if (mapPawn.IsColonist && mapPawn.IsRevia())
                        {
                            memoryHandler.TryGainMemory(Defs.SacrificedPositive);
                        }
                        else if (mapPawn.IsColonist &&
                                 mapPawn.IsCannibal() || mapPawn.IsPsychopath() || mapPawn.IsBloodlust())
                        {
                            memoryHandler.TryGainMemory(Defs.SacrificedNegative);
                        }
                    }

                    // Spawn the product.
                    GenSpawn.Spawn(Defs.Bloodstone, Prisoner.Position, Map);
                    TaleRecorder.RecordTale(Defs.TaleSacrificed, new[] { Sacrificer, Prisoner });
                }
            };
            yield return afterSacrificePrisoner;
        }
    }
}
