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
    public class ReviaTailGene : Gene
    {

        public ReviaTailGene() : base() { }
        internal static bool flag;
        public override void PostAdd()
        {
            base.PostAdd();
            if (pawn.gender != Gender.Female)
            {
                pawn.genes.RemoveGene(this);
            }
            else
            {
                pawn.SetSoulReaperLevel();
                flag = true;

            }
        }
        public override void PostRemove()
        {
            base.PostRemove();
            var btNeed = pawn.needs.TryGetNeed<BloodthirstNeed>();
            if (btNeed != null)
                pawn.needs.AllNeeds.Remove(btNeed);
            pawn.RemoveSoulReapHediffs();
        }
    }
}
