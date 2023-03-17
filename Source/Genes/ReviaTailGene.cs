using ReviaRace.Helpers;
using ReviaRace.Needs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using ReviaRace.HarmonyPatches;
namespace ReviaRace.Genes
{
    public class ReviaTailGene : ReviaBaseGene
    {

        public ReviaTailGene() : base() { }
        internal static bool flag;
        public override void PostAdd()
        {
            base.PostAdd();

            pawn.SetSoulReaperLevel();
            flag = true;
            if (Entry.FAPatchActive) ((Action)(() => HarmonyPatch_FacialAnimation.ResetFaceType(pawn)))();

        }
        public override void PostRemove()
        {
            base.PostRemove();
            var btNeed = pawn.needs.TryGetNeed<BloodthirstNeed>();
            if (btNeed != null)
                pawn.needs.AllNeeds.Remove(btNeed);
            pawn.RemoveSoulReapHediffs();
            if (Entry.FAPatchActive)((Action)(()=> HarmonyPatch_FacialAnimation.ResetFaceType(pawn)))();
        }
    }
}
