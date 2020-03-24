using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ReviaRace.RoomRoleWorkers
{
    public class SkarneChapel : RoomRoleWorker
    {
        public override float GetScore(Room room)
        {
            float score = room.ContainsThing(ThingDef.Named("ReviaBloodSigil")) ? 1.0f : 0.0f;
            return score;
        }
    }
}
