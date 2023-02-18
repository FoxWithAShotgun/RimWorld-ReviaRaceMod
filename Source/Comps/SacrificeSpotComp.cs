using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.AI;
using ReviaRace.Helpers;

namespace ReviaRace.Comps
{
    public class SacrificeSpotComp : CompUsable
    {
        public static int neededCount = 10;
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
        {
            if (!pawn.CanReserve(parent))
            {
                yield break;
            }
            else if (pawn.Map != null && pawn.Map == Find.CurrentMap)
            {
                if (pawn.IsRevia() || pawn.IsSkarnite())
                {
                    // Pick a prisoner to sacrifice.
                    var sacrificeTargets = pawn.Map.mapPawns.AllPawns.Where(p => IsValidSacrificeOption(pawn, p)).ToList();
                    if (sacrificeTargets.Count == 0)
                    {
                        var caption = Strings.SacrificePrisonerNone.Translate();
                        yield return new FloatMenuOption(caption, null, MenuOptionPriority.DisabledOption);
                    }
                    else
                    {
                        foreach (var target in sacrificeTargets)
                        {
                            yield return CreateSacrificeOption(pawn, target);
                        }
                    }
                    var convertTargets = pawn.Map.mapPawns.AllPawns.Where(p => IsValidConvertOption(p)).ToList();
                    List<Thing> bloodstonesStacks = new List<Thing>();
                    int needed = neededCount;

                    var bloodstonesOnSpot = parent.Map.thingGrid.ThingsListAt(parent.InteractionCell).FirstOrDefault(x => x.def == Defs.Bloodstone);
                    Log.Message("thing on grid " + bloodstonesOnSpot);

                    if (bloodstonesOnSpot != null && bloodstonesOnSpot.def == Defs.Bloodstone)
                    {
                        //bloodstonesStacks.Add(bloodstonesOnSpot);
                        needed -= bloodstonesOnSpot.stackCount;
                        Log.Message("Found on spot. Needed: " + needed);
                    }
                    /*pawn.Position.GetThingList(pawn.Map).Where(x=>x.)*/
                    if (needed > 0)
                        bloodstonesStacks.AddRange(parent.Map.listerThings.ThingsOfDef(Defs.Bloodstone).Except(bloodstonesOnSpot));
                    var thingCountList = new List<ThingCount>();
                    if (TryGetClosestBloodstones(pawn.Position, bloodstonesStacks, thingCountList,needed))
                        foreach (var convertable in convertTargets)
                        {
                            Log.Message("Select stacks:\n" + string.Join("\n", thingCountList.Select(x => x.Thing.ToString() + ": " + x.Count)));
                            yield return CreateConvertOption(pawn, convertable, thingCountList,bloodstonesOnSpot);
                        }
                }
                else
                {
                    var caption = Strings.NonSkarniteOrReviaSacrificing.Translate();
                    yield return new FloatMenuOption(caption, null, MenuOptionPriority.DisabledOption);
                }
            }
        }
        private static bool TryGetClosestBloodstones(IntVec3 rootCell, List<Thing> availableThings, List<ThingCount> chosen,int needed)
        {
            if (needed == 0)
                return true;
            Comparison<Thing> comparison = delegate (Thing t1, Thing t2)
            {
                float num5 = (float)(t1.PositionHeld - rootCell).LengthHorizontalSquared;
                float value = (float)(t2.PositionHeld - rootCell).LengthHorizontalSquared;
                return num5.CompareTo(value);
            };
            availableThings.Sort(comparison);
            while (availableThings.Count != 0)
            {
                //Log.Message($"Next iteration. Needed {needed}");
                chosen.Add(new ThingCount(availableThings[0], Math.Min(availableThings[0].stackCount, needed)));
                needed -= chosen.Last().Count;
                availableThings.RemoveAt(0);
                if (needed <= 0)
                    return true;
            }
            Log.Message("False");
            return false;

        }
        private bool IsValidSacrificeOption(Pawn sacrificer, Pawn victim)
        {

            return victim.IsHumanlike() &&
                (victim.IsPrisonerOfColony ||
                 victim.Downed && !victim.Faction.AllyOrNeutralTo(sacrificer.Faction));
        }
        private bool IsValidConvertOption(Pawn converting)
        {

            return converting.IsHumanlike() && converting.IsColonist
                && converting.genes.GenesListForReading.Any(x => x.def.defName.Contains("Revia"))
                && converting.genes.Xenotype != Defs.XenotypeDef;

        }
        protected FloatMenuOption CreateConvertOption(Pawn sacrificer, Pawn converting, List<ThingCount> bloodstones,Thing bloodstonesOnSpot =null)
        {
            var caption = Strings.ConvertXenotypeName.Translate(converting);
            return new FloatMenuOption(caption, () =>
            {
                //var haulOffJob = WorkGiverUtility.HaulStuffOffBillGiverJob(sacrificer, parent as IBillGiver, null);
                //if(haulOffJob!=null)
                //    sacrificer.jobs.TryTakeOrderedJob(haulOffJob);
                var job = JobMaker.MakeJob(Defs.ConvertXenotype, parent, bloodstonesOnSpot, converting);
                job.targetQueueB = new List<LocalTargetInfo>(bloodstones.Count);
                job.countQueue = new List<int>(bloodstones.Count);
                for (int i = 0; i < bloodstones.Count; i++)
                {
                    job.targetQueueB.Add(bloodstones[i].Thing);
                    job.countQueue.Add(bloodstones[i].Count);
                }
                job.count = 1;
                job.haulMode = HaulMode.ToCellNonStorage;
                sacrificer.jobs.debugLog = true;
                sacrificer.jobs.TryTakeOrderedJob(job);
            });
        }
        protected FloatMenuOption CreateSacrificeOption(Pawn sacrificer, Pawn prisoner)
        {
            var caption = Strings.SacrificePrisonerName.Translate(prisoner);
            return new FloatMenuOption(caption, () =>
            {
                var job = JobMaker.MakeJob(Defs.SacrificePrisoner, sacrificer, parent, prisoner);
                job.count = 1;
                sacrificer.jobs.TryTakeOrderedJob(job);
            });
        }
    }
}
