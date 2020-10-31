using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Comps
{
    public class InvokeLesserBlessing : InvokeBlessing
    {
        /// <summary>
        /// The cost of advancing is 2 ^ (n + 2) palestones 
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>The number of palestones to successfully advance.</returns>
        protected override int CalculateAdvanceCost(int tier)
        {
            return GetAdvanceCost(CostGrowthMode, tier, CostBase, CostGrowthFactor, CostGrowthStartTier);
        }

        public static int GetAdvanceCost(SacrificeCostGrowth growthMode, int tier, float costBase, float growthFactor, int startTier)
        {
            if (tier == -1 || tier == 9)
            {
                return -1;
            }

            return CalculateBaseCost(growthMode, tier, costBase, growthFactor, startTier) * 16;
        }
    }
}
