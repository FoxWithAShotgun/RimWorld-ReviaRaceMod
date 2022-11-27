using ReviaRace.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Genes
{
    public class ReviaClawsGene : ReviaBaseGene
    {
        public override void PostAdd()
        {
            base.PostAdd();
            //if (Added)
                //Current.Game.GetComponent<VerbFixComponent>().AddPawn(pawn);
        }
        public override void PostRemove()
        {
            base.PostRemove();
            //Current.Game.GetComponent<VerbFixComponent>().RemovePawn(pawn);
        }
    }
}
