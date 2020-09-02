using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using ReviaRace.Helpers;

namespace ReviaRace.WorkGivers
{
    public class SacrificePrisonerWorkGiver : WorkGiver_Scanner
    {
        public override int MaxRegionsToScanBeforeGlobalSearch => 4;
        public override PathEndMode PathEndMode => PathEndMode.OnCell;
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!forced)
            {
                return false;
            }

            Pawn target = t as Pawn;
            if (target == null || 
                target.Map == null ||
                !target.IsHumanlike())
            {
                return false;
            }

            if (!pawn.CanReach(t, PathEndMode, Danger.Some) ||
                t.IsForbidden(pawn))
            {
                return false;
            }

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            // TODO: Not implemented yet.
            return null;   
        }
    }
}
