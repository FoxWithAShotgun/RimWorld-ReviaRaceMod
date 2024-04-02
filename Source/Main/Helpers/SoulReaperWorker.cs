using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Helpers
{
    internal static class SoulReaperWorker
    {
        #region variables
        internal static bool EnableRandomSoulReapTier { get; set; }
        internal static IntRange SoulReapSpawnRange { get; set; }
        internal static bool SoulReapSpawnByAge { get; set; }
        internal static int SoulReapSpawnFixed { get; set; }
        private static SimpleCurve TailAgeCurve = new SimpleCurve(new CurvePoint[]
        {
            new CurvePoint(0, 1),
            new CurvePoint(12, 2),
            new CurvePoint(25, 3),
            new CurvePoint(30, 4),
            new CurvePoint(40, 5),
            new CurvePoint(45, 6),
            new CurvePoint(50, 7),
            new CurvePoint(70, 8),
            new CurvePoint(110, 9),
        });
        #endregion
        #region 
        internal static void SetSoulReaperLevel(this Pawn pawn)
        {
            if (pawn != null && !PawnGenerator.IsBeingGenerated(pawn))
            {
                pawn.AddSoulReapTier(1);
                return;
            }
            if (pawn != null && GetSoulReapTier(pawn) == -1)
            {
                if (pawn.kindDef == Defs.MarauderSkullshatterer ||
                    pawn.kindDef == Defs.TemplarHighTemplar)
                {
                    pawn.AddSoulReapTier(9);
                    pawn.skills.GetSkill(SkillDefOf.Melee).Level = 20;
                    pawn.skills.GetSkill(SkillDefOf.Shooting).Level = 20;

                    if (!pawn.story.traits.HasTrait(ReviaDefOf.Tough))
                    {
                        pawn.story.traits.allTraits.AddDistinct(new Trait(ReviaDefOf.Tough));
                    }
                }
                else
                {
                    if (EnableRandomSoulReapTier)
                    {
                        // This should have at least tier 1 of soul reap.
                        // Add it now.
                        var rng = new Random();
                        if (SoulReapSpawnByAge)
                        {
                            float ageTails = TailAgeCurve.Evaluate(pawn.ageTracker.AgeBiologicalYears);
                            float deviation = (float)(rng.NextDouble());

                            var tails = (int)Math.Max(1, Math.Min(9, ageTails + deviation - 0.5));
                            pawn.AddSoulReapTier(tails);
                        }
                        else
                        {
                            pawn.AddSoulReapTier(rng.Next(SoulReapSpawnRange.min, SoulReapSpawnRange.max));
                        }
                    }
                    else
                    {
                        pawn.AddSoulReapTier(SoulReapSpawnFixed);
                    }
                }
            }
        }
        internal static int GetSoulReapTier(Pawn pawn)
        {
            if (pawn.SoulReapHediff() != null)
            {
                string tierAsString = pawn.SoulReapHediff().def.defName.Replace("ReviaRaceSoulreapTier", "");
                return int.TryParse(tierAsString, out int tier) ? tier : -1;
            }
            else
            {
                return -1;
            }
        }
        internal static void AddSoulReapTier(this Pawn pawn, int tier)
        {
            if (tier < 0 || tier > 9 ||
                pawn == null)
            {
                return;
            }

            HediffDef toAdd = HediffDef.Named($"ReviaRaceSoulreapTier{tier}");
            toAdd.initialSeverity = float.Epsilon;
            pawn.health.AddHediff(toAdd);
        }
        internal static void RemoveSoulReapHediffs(this Pawn pawn)
        {
            if (pawn == null)
            {
                return;
            }

            var hediffsToRemove = pawn.health.hediffSet.hediffs.Where(hediff => hediff.def.defName.StartsWith("ReviaRaceSoulreapTier")).ToList();
            foreach (var hediff in hediffsToRemove)
            {
                pawn.health.RemoveHediff(hediff);
            }
        }
        #endregion
    }
}
