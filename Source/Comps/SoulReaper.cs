using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace ReviaRace.Comps
{
    public class SoulReaper : ThingComp
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            var pawn = parent as Pawn;
            
            if (pawn != null && GetSoulReapTier() == -1)
            {
                // This should have at least tier 1 of soul reap.
                // Add it now.
                var rng = new Random();
                var tier = rng.Next(1, 3);
                AddSoulReapTier(tier);
            }
        }

        private Hediff SoulReapHediff => (parent as Pawn)?.health.hediffSet.hediffs
                                         .FirstOrDefault(hediff => hediff.def.label.Contains("Soul Reap"));

        internal int GetSoulReapTier()
        {
            var tier = -1;

            if (SoulReapHediff != null)
            {
                switch (SoulReapHediff.def.defName)
                {
                    case "ReviaRaceSoulreapTier1": tier = 1; break;
                    case "ReviaRaceSoulreapTier2": tier = 2; break;
                    case "ReviaRaceSoulreapTier3": tier = 3; break;
                    case "ReviaRaceSoulreapTier4": tier = 4; break;
                    case "ReviaRaceSoulreapTier5": tier = 5; break;
                    case "ReviaRaceSoulreapTier6": tier = 6; break;
                    case "ReviaRaceSoulreapTier7": tier = 7; break;
                    case "ReviaRaceSoulreapTier8": tier = 8; break;
                    case "ReviaRaceSoulreapTier9": tier = 9; break;
                    default: break;
                }
            }

            return tier;
        }

        internal void AddSoulReapTier(int tier)
        {
            var pawn = parent as Pawn;
            if (tier < 0 || tier > 9 || 
                pawn == null)
            {
                return;
            }
           
            HediffDef toAdd = HediffDef.Named($"ReviaRaceSoulreapTier{tier}");
            pawn.health.AddHediff(toAdd);
        }

        internal void RemoveSoulReapHediffs()
        {
            var pawn = parent as Pawn;
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
    }
}
