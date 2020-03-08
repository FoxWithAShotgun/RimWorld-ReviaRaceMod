using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace
{
    public class InvokeBlessing : CompUseEffect
    {
        public override float OrderPriority => 0;

        private int GetSoulReapTier(Pawn pawn)
        {
            if (pawn == null)
            {
                return -1;
            }
            
            var hediff = pawn.health.hediffSet.hediffs.First(h => h.Label.Contains("Soul Reap"));
            if (hediff == null)
            {
                return -1;
            }

            var tier = -1;
            switch (hediff.def.label)
            {
                case "Soul Reap (1)": tier = 1; break;
                case "Soul Reap (2)": tier = 2; break;
                case "Soul Reap (3)": tier = 3; break;
                case "Soul Reap (4)": tier = 4; break;
                case "Soul Reap (5)": tier = 5; break;
                case "Soul Reap (6)": tier = 6; break;
                case "Soul Reap (7)": tier = 7; break;
                case "Soul Reap (8)": tier = 8; break;
                case "Soul Reap (9)": tier = 9; break;
                default: break;
            }

            return tier;
        }

        public override void DoEffect(Pawn pawn)
        {
            base.DoEffect(pawn);

            if (pawn.MapHeld == null)
            {
                return;
            }

            var soulReapTier = GetSoulReapTier(pawn);
            if (soulReapTier == -1)
            {
                Messages.Message("The bloodstone does nothing for the uninitiated...", pawn, MessageTypeDefOf.NegativeEvent, false);
                return;
            }
            else if (soulReapTier == 9)
            {
                Messages.Message($"{pawn.Name} is already at the height of {pawn.ProSubj()} power!", pawn, MessageTypeDefOf.NeutralEvent, false);
                return;
            }

            // Roll the dice!
            if (CalculateAdvanceSuccess(pawn))
            {
                AdvanceSoulreapTier(pawn);
            }

            // Destroy one bloodstone on use, regardless of success.
            parent.stackCount--;
        }

        /// <summary>
        /// Pray to RNGesus for success.
        /// Probability of success is 1 / (2^(n-2)),
        /// guaranteed when n < 3.
        /// Soul Reap tier 9 cannot ever advance, as it is the max level.
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>True if the random number generated passes the RNG check.
        /// False if the hediff wasn't a soulreap that can be levelled up, 
        /// or if the RNG check failed.</returns>
        private bool CalculateAdvanceSuccess(Pawn pawn)
        {
            var tier = GetSoulReapTier(pawn);
            if (tier == -1 || tier == 9)
            {
                return false;
            }

            Random rng = new Random();

            double roll = rng.NextDouble();
            double threshold = 1 / (Math.Pow(2, tier - 2));

            return roll <= threshold;
        }

        /// <summary>
        /// Advances the soul reap tier of a pawn. No effect on Tier 9 soul reaps,
        /// or if the pawn doesn't have it to begin with.
        /// </summary>
        /// <param name="pawn">The pawn to advance.</param>
        private void AdvanceSoulreapTier(Pawn pawn)
        {
            var soulReapTier = GetSoulReapTier(pawn);
            if (soulReapTier == -1 || soulReapTier == 9)
            {
                return;
            }

            var toAdd = HediffDef.Named($"ReviaRaceSoulreapTier{soulReapTier + 1}");
            var toRemove = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named($"ReviaRaceSoulreapTier{soulReapTier}"));
            
            if (toAdd != null)
            {
                pawn.health.AddHediff(toAdd);
                pawn.health.RemoveHediff(toRemove);
                Messages.Message($"The blessing has succeeded, and {pawn.Name} has grown stronger.", pawn, MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}
