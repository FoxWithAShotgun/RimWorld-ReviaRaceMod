using ReviaRace.Needs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Genes
{
    public class ReviaBaseGene : Gene
    {
        protected bool Added { get; private set; }
        public override void PostAdd()
        {
            base.PostAdd();
            if (pawn.gender != Gender.Female)
            {
                OnWrongAdd();
            }
            else
                Added = true;
        }
        bool shouldInstantlyRemove = true, shouldInstantlyKill = true;
        private void OnWrongAdd()
        {
            if (shouldInstantlyRemove)
                pawn.genes.RemoveGene(this);
            if (shouldInstantlyKill || PawnGenerator.IsBeingGenerated(pawn))
                if (!pawn.Dead) pawn.Kill(null);
        }
        public override void PostRemove()
        {
            base.PostRemove();
        }
    }
}
