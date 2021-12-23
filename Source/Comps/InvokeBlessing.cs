using ReviaRace.Helpers;
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
        public static SacrificeCostGrowth CostGrowthMode { get; set; }
        public static float CostBase { get; set; }
        public static float CostGrowthFactor { get; set; }
        public static int CostGrowthStartTier { get; set; }

        /// <summary>
        /// Value of a blessing relative to a bloodstone.
        /// </summary>
        protected abstract float BlessingValue { get; }

        public override void DoEffect(Pawn pawn)
        {
            base.DoEffect(pawn);

            if (!ViabilityCheck(pawn))
            {
                return;
            }

            var srComp = pawn.GetComp<SoulReaper>();
            var srTier = srComp.GetSoulReapTier();
            var srHediff = srComp.SoulReapHediff;

            var cost = InvokeGreaterBlessing.GetAdvanceCost(CostGrowthMode, srTier, CostBase, CostGrowthFactor, CostGrowthStartTier);
            var requiredItemCount = Math.Max(1, (int)Math.Ceiling((cost - srHediff.Severity) / BlessingValue));

            if (requiredItemCount <= parent.stackCount)
            {
                DecrementOnUse(pawn, requiredItemCount);
                IncreaseSoulReapTier(pawn);
            } 
            else
            {
                srHediff.Severity += parent.stackCount * BlessingValue;
                DecrementOnUse(pawn, parent.stackCount);

                var msg = Strings.SacrificeRejected.Translate(pawn.NameShortColored);
                Messages.Message(msg, pawn, MessageTypeDefOf.NeutralEvent, false);

                var remainingCost = (int)Math.Ceiling((cost - srHediff.Severity) / BlessingValue);
                var stuffCountMsg = Strings.BloodStuffCountDemand.Translate(remainingCost, parent.LabelNoCount);
                Messages.Message(stuffCountMsg, pawn, MessageTypeDefOf.NeutralEvent, false);
                return;
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

            var msg = Strings.SacrificeSuccess.Translate(pawn.NameShortColored);
            Messages.Message(msg, pawn, MessageTypeDefOf.PositiveEvent);
        }

        /// <summary>
        /// Decrement the number of consumed items. If the count would be zero or less, destroy the stack.
        /// </summary>
        /// <param name="pawn">The pawn using the stack.</param>
        /// <param name="cost">The cost of the invocation.</param>
        protected void DecrementOnUse(Pawn pawn, int cost)
        {
            var msg = Strings.BloodStuffCountConsumed.Translate(cost, parent.LabelNoCount, pawn.NameShortColored);
            Messages.Message(msg, pawn, MessageTypeDefOf.NeutralEvent);
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
                throw new ArgumentException("Attempting to use more items than is available!", nameof(cost));
            }
        }

        
        protected abstract int CalculateAdvanceCost(int tier);
        /// <summary>
        /// Gets the cost of going up tiers according to formula.
        /// Cost is that of bloodstones.
        /// </summary>
        /// <param name="tier">The current tier of soul reap.</param>
        /// <returns>The base cost of leveling up soul reap, in bloodstones.</returns>
        protected static int CalculateBaseCost(SacrificeCostGrowth growthMode, int tier, float costBase, float growthFactor, int startTier)
        {
            var tierDiff = Math.Max(0, tier - startTier);

            switch (growthMode)
            {
                case SacrificeCostGrowth.Linear:
                    return (int)(costBase + tierDiff * growthFactor);
                case SacrificeCostGrowth.Exponential:
                    return (int)(costBase * Math.Max(1, (int)Math.Pow(growthFactor, tierDiff)));
                case SacrificeCostGrowth.Quadratic:
                    return (int)(costBase + costBase * tierDiff * tierDiff);
                default: return 0;
            }
        }

        /// <summary>
        /// Checks if the pawn is eligible to advance soul reap tiers.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        protected bool ViabilityCheck(Pawn pawn)
        {
            var srComp = pawn.GetComp<SoulReaper>();
            if (srComp == null)
            {
                Messages.Message(Strings.OfferingInvalidPawn.Translate(), pawn, MessageTypeDefOf.NegativeEvent, false);
                return false;
            }

            var startMsg = Strings.OfferingStart.Translate(pawn.NameShortColored, parent.LabelNoCount);
            Messages.Message(startMsg, pawn, MessageTypeDefOf.NeutralEvent, false);

            var soulReapTier = srComp.GetSoulReapTier();
            if (soulReapTier == -1)
            {
                Messages.Message(Strings.OfferingInvalidPawn.Translate(), pawn, MessageTypeDefOf.NegativeEvent, false);
                return false;
            }
            else if (soulReapTier == 9)
            {
                var msg = Strings.OfferingMaxLevel.Translate(pawn.NameShortColored, pawn.ProSubj());
                Messages.Message(msg, pawn, MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }
    }

    public enum SacrificeCostGrowth
    {
        Linear,
        Quadratic,
        Exponential,
    }
}
