using ReviaRace.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace
{
    public static class Utils
    {
        public static void PostSacrifide(Map map, bool corpse = false)
        {
            foreach (var mapPawn in map.mapPawns.FreeColonistsAndPrisoners)
            {
                if (mapPawn.IsPrisoner)
                {
                    mapPawn.needs.mood.thoughts.memories.TryGainMemory(corpse ? ReviaDefOf.ReviaRaceThoughtSacrificedNegativePrisoner : Defs.SacrificedFear);
                }
                else if (mapPawn.IsColonist && (mapPawn.IsRevia() || mapPawn.IsSkarnite()))
                {
                    if (!corpse)
                    {
                        mapPawn.needs.mood.thoughts.memories.TryGainMemory(Defs.SacrificedPositive);
                    }
                }
                else if (mapPawn.IsColonist &&
                         !(mapPawn.IsCannibal() || mapPawn.IsPsychopath() || mapPawn.IsBloodlust()))
                {
                    mapPawn.needs.mood.thoughts.memories.TryGainMemory(Defs.SacrificedNegative);
                }
            }
        }
    }
}
