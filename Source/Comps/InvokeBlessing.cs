using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Comps
{
    public class InvokeBlessing : CompUseEffect
    {
        public override void DoEffect(Pawn pawn)
        {
            base.DoEffect(pawn);
            var srComp = pawn.GetComp<SoulReaper>();
            Messages.Message($"{pawn.Name} offers some bloodstones to the bloody god.", pawn, MessageTypeDefOf.NeutralEvent, false);

            if (pawn.MapHeld == null || srComp == null)
            {
                Messages.Message($"Invalid target. srcomp null = {srComp == null}", pawn, MessageTypeDefOf.CautionInput, false);
                return;
            }

            var soulReapTier = srComp.GetSoulReapTier();
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

            // Check that we have enough bloodstones in the stack to proceed.
            int cost = CalculateAdvanceCost(soulReapTier);
            if (cost > parent.stackCount)
            {
                Messages.Message($"{pawn.Name}'s offering was rejected. The blood god demands more blood.", pawn, MessageTypeDefOf.NeutralEvent, false);
                return;
            }

            // Blood for the blood god! Skulls for the skull throne!
            srComp.RemoveSoulReapHediffs();
            srComp.AddSoulReapTier(soulReapTier + 1);
            Messages.Message($"The blessing has succeeded, and {pawn.Name} has grown stronger.", pawn, MessageTypeDefOf.PositiveEvent);

            // Destroy the bloodstones that were used up. If there were exactly enough, destroy the stack.
            if (parent.stackCount == cost)
            {
                parent.Destroy();
            }
            else
            {
                parent.stackCount -= cost;
            }
        }

        /// <summary>
        /// The cost of advancing is 2 ^ (n-2) bloodstones, 
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>True if the random number generated passes the RNG check.
        /// False if the hediff wasn't a soulreap that can be levelled up, 
        /// or if the RNG check failed.</returns>
        private int CalculateAdvanceCost(int tier)
        {
            if (tier == -1 || tier == 9)
            {
                return -1;
            }

            double cost = Math.Max(1.0, Math.Pow(2, tier - 2));

            return (int)cost;
        }
    }
}
