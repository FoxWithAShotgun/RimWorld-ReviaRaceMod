using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Comps
{
    public class InvokeGreaterBlessing : InvokeBlessing
    {
        /// <summary>
        /// The cost of advancing is 2 ^ (n-2) bloodstones, 
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>The cost (in bloodstones) required to advance the Soul Reap tier.</returns>
        protected override int CalculateAdvanceCost(int tier)
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
