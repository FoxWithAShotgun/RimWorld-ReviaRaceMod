using ReviaRace.Helpers;
using ReviaRace.Needs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Workers
{
    public class ThoughtWorkerBloodthirst : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            var bloodthirstNeed = p.needs.TryGetNeed<BloodthirstNeed>();

            if (bloodthirstNeed == null)
            {
                return ThoughtState.Inactive;
            }

            var level = bloodthirstNeed.CurLevel;
            var stage = BloodthirstNeed.Thresholds.FindIndex(t => t >= level);
            return ThoughtState.ActiveAtStage(stage);
        }
    }
}
