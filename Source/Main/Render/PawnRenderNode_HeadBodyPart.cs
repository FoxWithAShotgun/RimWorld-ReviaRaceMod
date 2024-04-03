using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace
{
    public class PawnRenderNode_HeadBodyPart : PawnRenderNode_AttachmentHead
    {
        public PawnRenderNode_HeadBodyPart(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }
        PawnRenderNodeProperties_BodyPart Props => props as PawnRenderNodeProperties_BodyPart;
        public override Graphic GraphicFor(Pawn pawn)
        {
            var bodyPartRecord = pawn.RaceProps.body.AllParts.FirstOrDefault(x => x.untranslatedCustomLabel == Props.bodyPartLabel);
            if (bodyPartRecord == null || pawn.health.hediffSet.PartIsMissing(bodyPartRecord))
            {
                return null;
            }
            return base.GraphicFor(pawn);
        }
    }
}
