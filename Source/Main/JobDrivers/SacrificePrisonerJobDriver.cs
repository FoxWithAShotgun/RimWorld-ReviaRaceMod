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
            return pawn.Reserve(Prisoner, this.job) &&
                   pawn.Reserve(SacrificeSpot, this.job);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(iPrisoner);
            this.FailOnDestroyedOrNull(iSacrificeBuilding);
            this.FailOnAggroMentalState(iPrisoner);
            
            yield return Toils_Goto.GotoThing(iPrisoner, PathEndMode.OnCell);
            yield return Toils_Haul.StartCarryThing(iPrisoner, true, false);
            yield return Toils_Goto.GotoThing(iSacrificeBuilding, SacrificeSpot.InteractionCell);
            yield return Toils_Reserve.Release(iPrisoner);
            
            var doSacrificePrisoner = new Toil
            {
                socialMode = RandomSocialMode.Off,
            };
            doSacrificePrisoner.initAction = () =>
            {
                Sacrificer.carryTracker.TryDropCarriedThing(SacrificeSpot.InteractionCell, ThingPlaceMode.Direct, out var thing, null);
                Prisoner.jobs.StartJob(JobMaker.MakeJob(Defs.PrisonerWait));
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
            yield return doSacrificePrisoner.WithProgressBar(iPawn, () => (TicksMax - (float)TicksLeft) / TicksMax);

            var afterSacrificePrisoner = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Instant,
                initAction = () =>
                {
                    var heartBpr = Prisoner.RaceProps.body.AllParts.Find(bpr => bpr.def.defName == "Heart");
                    var dInfo = new DamageInfo(Defs.HeartExtraction, 99999f, armorPenetration: 999f, hitPart: heartBpr);
                    Prisoner.Kill(dInfo);
                    ThoughtUtility.GiveThoughtsForPawnExecuted(Prisoner, Sacrificer, PawnExecutionKind.GenericBrutal);

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
                    Utils.PostSacrifide(Map);

                    // Spawn the product, or handle one of the other effects.
                    // Find a good spot to spawn the product.
                    IntVec3 spawnPos = CellFinder.FindNoWipeSpawnLocNear(
                        Prisoner.Position,
                        Map,
                        Defs.Bloodstone,
                        Rot4.Random, 6
                        );

                    if (Map.GameConditionManager.ConditionIsActive(Defs.Eclipse))
                    {
                        var missingParts = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
                        if (missingParts.Count > 0)
                        {
                            var missing = missingParts.RandomElement();
                            pawn.health.hediffSet.hediffs.Remove(missing);
                            var maxHp = missing.Part.def.GetMaxHealth(pawn);
                            
                            var injuryHediff = pawn.health.AddHediff(HediffDefOf.Cut, missing.Part, new DamageInfo(DamageDefOf.Rotting, 1.0f, 10000.00f));
                            var permHediffComp = injuryHediff.TryGetComp<HediffComp_GetsPermanent>();
                            injuryHediff.Severity = maxHp / 2.0f;
                            permHediffComp.IsPermanent = true;
                        }
                        else 
                        {
                            var thing = GenSpawn.Spawn(Defs.Bloodstone, spawnPos, Map);
                            thing.stackCount += 2;
                        }
                    }
                    else if (Map.GameConditionManager.ConditionIsActive(Defs.SolarFlare))
                    {
                        var permInjuries = pawn.health.hediffSet.hediffs.Where(h => h.IsPermanent() && !(h is Hediff_MissingPart)).ToList();
                        if (permInjuries.Count > 0)
                        {
                            var randomInjury = permInjuries.RandomElement();
                            randomInjury.Severity -= randomInjury.Part.depth == BodyPartDepth.Inside ? 2.0f : 4.0f;
                        }
                        else
                        {
                            var thing = GenSpawn.Spawn(Defs.Bloodstone, spawnPos, Map);
                            thing.stackCount += 1;
                        }
                    }
                    else
                    {
                        var thing = GenSpawn.Spawn(Defs.Bloodstone, spawnPos, Map);
                    }

                    var bloodthirst = Sacrificer.needs.TryGetNeed<BloodthirstNeed>();
                    if (bloodthirst != null)
                    {
                        bloodthirst.CurLevel += 0.80f * Prisoner.BodySize;
                    }
                    TaleRecorder.RecordTale(Defs.TaleSacrificed, new[] { Sacrificer, Prisoner });

                    // Find a good spot to move the prisoner corpse.
                    IntVec3 movePos = CellFinder.FindNoWipeSpawnLocNear(
                        Prisoner.Position,
                        Map,
                        Prisoner.Corpse.def,
                        Rot4.Random, 6);
                    Prisoner.Corpse.Position = movePos;
                }
            };
            yield return afterSacrificePrisoner;

            
        }
    }
}
