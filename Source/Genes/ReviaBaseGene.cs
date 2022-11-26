using ReviaRace.Helpers;
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
            CheckGenesForIncompleteness();
        }
        bool shouldInstantlyRemove = true, shouldInstantlyKill = true;
        private void OnWrongAdd()
        {
            if (shouldInstantlyRemove)
                pawn.genes.RemoveGene(this);
            if (shouldInstantlyKill || PawnGenerator.IsBeingGenerated(pawn))
                if (!pawn.Dead)
                    pawn.Kill(null, pawn.health.AddHediff(Defs.GeneRejection));
        }
        public override void PostRemove()
        {
            base.PostRemove();
            CheckGenesForIncompleteness();
        }
        void CheckGenesForIncompleteness()
        {
            if (pawn.Dead) return;
            if (pawn.genes.HasGene(Defs.Tail))
            {
                TryRemoveDebuff();
            }
            else
            {
                if (pawn.genes.HasGene(Defs.Teeth) || pawn.genes.HasGene(Defs.Claws) || pawn.genes.HasGene(Defs.Ears))
                {
                    TryAddDebuff();
                }
                else TryRemoveDebuff();
            }
        }

        private void TryAddDebuff()
        {
            if (!pawn.health.hediffSet.HasHediff(Defs.IncompleteHediff))
                pawn.health.AddHediff(Defs.IncompleteHediff);
        }

        private void TryRemoveDebuff()
        {
            var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(Defs.IncompleteHediff);
            if (hediff != null)
                pawn.health.RemoveHediff(hediff);
        }
    }
}
