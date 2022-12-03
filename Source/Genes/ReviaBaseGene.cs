using ReviaRace.Comps;
using ReviaRace.HarmonyPatches;
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
        public static bool DisableUncompleteDebuff_Claws { get; internal set; }
        public static bool DisableUncompleteDebuff_Ears { get; internal set; }
        public static bool DisableUncompleteDebuff_Teeth { get; internal set; }

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
            RefreshAttackHediffs(pawn);
        }
        //bool shouldInstantlyRemove = true, shouldInstantlyKill = true;
        internal static RejectionType RejectionType { get; set; }
        protected virtual void OnWrongAdd()
        {
            switch (RejectionType)
            {
                case RejectionType.InstaKill:
                    pawn.genes.RemoveGene(this);
                    if (!pawn.Dead&&!PawnGenerator.IsBeingGenerated(pawn))
                        pawn.Kill(null, pawn.health.AddHediff(Defs.GeneRejection));
                    break;
                case RejectionType.Disease:
                    if(!pawn.health.hediffSet.HasHediff(Defs.GeneRejection))
                    pawn.health.AddHediff(Defs.GeneRejection);
                    break;
                case RejectionType.NoRejection:
                    break;
                default:
                    break;
            }
            
            
        }
        protected void TryRemoveRejection()
        {
            if (!pawn.Dead)
            {
                if (pawn.health.hediffSet.HasHediff(Defs.GeneRejection))
                {
                    if(Entry.GeneCanBeAdded(this.pawn,this.def))
                    if (RejectionType==RejectionType.NoRejection||((DisableUncompleteDebuff_Teeth || !pawn.genes.HasGene(Defs.Teeth))
                    && (DisableUncompleteDebuff_Claws || !pawn.genes.HasGene(Defs.Claws))
                    && (DisableUncompleteDebuff_Ears || !pawn.genes.HasGene(Defs.Ears))
                    && !pawn.genes.HasGene(Defs.Tail)))
                        TryRemoveDebuff(pawn, Defs.GeneRejection);
                    
                    
                }
            }
        }
        public override void PostRemove()
        {
            base.PostRemove();
            TryRemoveRejection();
            CheckGenesForIncompleteness();
            RefreshAttackHediffs(pawn);
        }

        

        void CheckGenesForIncompleteness()
        {
            if (pawn.Dead) return;
            if (pawn.genes.HasGene(Defs.Tail))
            {
                TryRemoveDebuff(pawn,Defs.IncompleteHediff);
            }
            else
            {
                if ((!DisableUncompleteDebuff_Teeth&& pawn.genes.HasGene(Defs.Teeth)) 
                    || (!DisableUncompleteDebuff_Claws&& pawn.genes.HasGene(Defs.Claws))
                    || (!DisableUncompleteDebuff_Ears&& pawn.genes.HasGene(Defs.Ears)))
                {
                    TryAddDebuff(pawn, Defs.IncompleteHediff);
                }
                else TryRemoveDebuff(pawn, Defs.IncompleteHediff);
            }
        }

        private static void TryAddDebuff(Pawn pawn,HediffDef hediffDef, BodyPartRecord bodyPart = null)
        {
            if (!pawn.health.hediffSet.HasHediff(hediffDef, bodyPart))
                pawn.health.AddHediff(hediffDef, bodyPart);
        }

        private static void TryRemoveDebuff(Pawn pawn, HediffDef hediffDef, BodyPartRecord bodyPart = null)
        {
            var hediff = pawn.health.hediffSet.hediffs.FirstOrDefault(x => x.def == hediffDef && x.Part == bodyPart);
            if (hediff != null)
                pawn.health.RemoveHediff(hediff);
        }
        private static BodyPartRecord jawRecord,rightFist,leftFist;

        public static void RefreshAttackHediffs(Pawn pawn)
        {
            if (pawn.genes == null) return;

            if (jawRecord == null) jawRecord = pawn.def.race.body.AllParts.First(x => x.def.defName == "Jaw");
            if (leftFist == null) leftFist = pawn.def.race.body.AllParts.First(x => x.untranslatedCustomLabel == "left hand");
            if (rightFist == null) rightFist = pawn.def.race.body.AllParts.First(x => x.untranslatedCustomLabel == "right hand");


            if (pawn.genes.HasGene(Defs.Teeth) && !pawn.health.hediffSet.PartIsMissing(jawRecord))
                TryAddDebuff(pawn, Defs.TeethHediff, jawRecord);
            else
                TryRemoveDebuff(pawn, Defs.TeethHediff, jawRecord);

            if (pawn.genes.HasGene(Defs.Claws))
            {
                if (!IsMissingFistAndFingers(pawn, leftFist))
                    TryAddDebuff(pawn, Defs.ClawsHediff, leftFist);
                else
                    TryRemoveDebuff(pawn, Defs.ClawsHediff, leftFist);

                if (!IsMissingFistAndFingers(pawn, rightFist))
                    TryAddDebuff(pawn, Defs.ClawsHediff, rightFist);
                else
                    TryRemoveDebuff(pawn, Defs.ClawsHediff, rightFist);
            }
            else
            {
                TryRemoveDebuff(pawn, Defs.ClawsHediff, rightFist);
                TryRemoveDebuff(pawn, Defs.ClawsHediff, leftFist);
            }
        }
        static bool IsMissingFistAndFingers(Pawn pawn,BodyPartRecord fistRecord)
        {
            if (pawn.health.hediffSet.PartIsMissing(fistRecord))
                return true;
            if (fistRecord.GetDirectChildParts().All(x => pawn.health.hediffSet.PartIsMissing(x)))
                return true;
            return false;
        }
    }
}
