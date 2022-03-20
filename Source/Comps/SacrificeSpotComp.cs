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
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
        {
            if (!pawn.CanReserve(parent))
            {
                yield break;
            }
            else if (pawn.Map != null && pawn.Map == Find.CurrentMap)
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
            }
        }

        private bool IsValidSacrificeOption(Pawn sacrificer, Pawn victim)
        {
            return victim.IsHumanlike() &&
                (victim.IsPrisonerOfColony ||
                 victim.Downed && !victim.Faction.AllyOrNeutralTo(sacrificer.Faction));
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
