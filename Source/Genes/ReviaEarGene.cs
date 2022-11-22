using ReviaRace.Needs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Genes
{
    public class ReviaEarGene:Gene
    {
        public ReviaEarGene() : base() { }
        public override void PostAdd()
        {
            base.PostAdd();
            if (pawn.gender != Gender.Female)
            {
                pawn.genes.RemoveGene(this);
                var btNeed= pawn.needs.TryGetNeed<BloodthirstNeed>();
                if(btNeed!=null)
                pawn.needs.AllNeeds.Remove(btNeed);
                Vector3Utility
                
            }
        }
    }
}
