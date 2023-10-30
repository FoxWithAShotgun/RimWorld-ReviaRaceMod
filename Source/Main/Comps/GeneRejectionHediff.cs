using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Comps
{
    public class GeneRejectionHediff : HediffWithComps
    {
        public override void Notify_Resurrected()
        {
            base.Notify_Resurrected();
            this.severityInt = Math.Min(this.severityInt, 0.5f);
        }
    }
}
