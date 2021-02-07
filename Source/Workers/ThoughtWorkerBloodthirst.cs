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
            if (!BloodthirstNeed.Enabled)
            {
                return ThoughtState.Inactive;
            }

            var bloodthirstNeed = p.needs.TryGetNeed<BloodthirstNeed>();
            if (bloodthirstNeed == null)
            {
                return ThoughtState.Inactive;
            }

            var level = bloodthirstNeed.CurLevel;
            var stage = BloodthirstNeed.Thresholds.FindLastIndex(t => t <= level);

            return ThoughtState.ActiveAtStage(stage);
        }
    }
}
