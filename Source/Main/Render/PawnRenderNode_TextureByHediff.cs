using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace
{
    public class PawnRenderNode_TextureByHediff : PawnRenderNode
    {
        public PawnRenderNode_TextureByHediff(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }
        PawnRendererNodeProperties_TextureByHediff Props => props as PawnRendererNodeProperties_TextureByHediff;
        protected override string TexPathFor(Pawn pawn)
        {
            foreach (var item in Props.hediffTex)
            {
                if (pawn.health.hediffSet.HasHediff(item.hediff)) return item.path;
            }
            Log.ErrorOnce($"Unable to find texture for {pawn}", 2902395);
            return base.TexPathFor(pawn);
        }
    }
}
