using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Comps
{
    public abstract class InvokeBlessing : CompUseEffect
    {
        public override void DoEffect(Pawn pawn)
        {
            base.DoEffect(pawn);

            if (!ViabilityCheck(pawn))
            {
                return;
            }

            var srComp = pawn.GetComp<SoulReaper>();
            var srTier = srComp.GetSoulReapTier();
            var cost = CalculateAdvanceCost(srTier);

            if (cost > parent.stackCount)
            {
                
                if (cost / 2.0f > parent.stackCount)
                {
                    Messages.Message($"The blood god was gravely insulted by {pawn.NameShortColored}'s paltry offering!", pawn, MessageTypeDefOf.NegativeEvent, false);
                    new WeatherEvent_LightningFlash(pawn.Map).FireEvent();
                    new WeatherEvent_LightningStrike(pawn.Map, pawn.Position).FireEvent();
                }
                else
                {
                    Messages.Message($"{pawn.NameShortColored}'s offering was rejected. The blood god demands more blood.", pawn, MessageTypeDefOf.NeutralEvent, false);
                }
                Messages.Message($"{cost} {parent.LabelNoCount}s were demanded.", pawn, MessageTypeDefOf.NeutralEvent, false);
                return;
            } 
            else
            {
                DecrementOnUse(pawn, cost);
                IncreaseSoulReapTier(pawn);
            }
        }

        /// <summary>
        /// Increase the soul reap tier of this pawn.
        /// </summary>
        /// <param name="pawn">The pawn to increase the Soul Reap tier of.</param>
        protected void IncreaseSoulReapTier(Pawn pawn)
        {
            var srComp = pawn.GetComp<SoulReaper>();
            var soulReapTier = srComp.GetSoulReapTier();

            // Blood for the blood god! Skulls for the skull throne!
            srComp.RemoveSoulReapHediffs();
            srComp.AddSoulReapTier(soulReapTier + 1);
            
            Messages.Message($"The blessing has succeeded, and {pawn.NameShortColored} has grown stronger.", pawn, MessageTypeDefOf.PositiveEvent);
        }

        /// <summary>
        /// Decrement the number of consumed items. If the count would be zero or less, destroy the stack.
        /// </summary>
        /// <param name="pawn">The pawn using the stack.</param>
        /// <param name="cost">The cost of the invocation.</param>
        protected void DecrementOnUse(Pawn pawn, int cost)
        {
            Messages.Message($"{cost} {parent.LabelNoCount}(s) were consumed in {pawn.NameShortColored}'s offering.", pawn, MessageTypeDefOf.NeutralEvent);
            if (parent.stackCount == cost)
            {
                parent.Destroy();
            }
            else if (parent.stackCount > cost)
            {
                parent.stackCount -= cost;
            }
            else
            {
                throw new ArgumentException("Attempting to use more items than is available!");
            }
        }

        protected abstract int CalculateAdvanceCost(int tier);

        /// <summary>
        /// Checks if the pawn is eligible to advance soul reap tiers.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        protected bool ViabilityCheck(Pawn pawn)
        {
            var srComp = pawn.GetComp<SoulReaper>();
            Messages.Message($"{pawn.NameShortColored} offers some {parent.LabelNoCount}s to the bloody god.", pawn, MessageTypeDefOf.NeutralEvent, false);

            var soulReapTier = srComp.GetSoulReapTier();
            if (soulReapTier == -1)
            {
                Messages.Message($"The {parent.LabelNoCount} does nothing for the uninitiated...", pawn, MessageTypeDefOf.NegativeEvent, false);
                return false;
            }
            else if (soulReapTier == 9)
            {
                Messages.Message($"{pawn.NameShortColored} is already at the height of {pawn.ProSubj()} power!", pawn, MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }
    }
}
