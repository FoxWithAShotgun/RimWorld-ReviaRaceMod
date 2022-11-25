using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.CustomDefs
{
    public class CustomTool : Def
    {
        public string toolLabelToReplace;
        public bool? ensureLinkedBodyPartsGroupAlwaysUsable;
        public BodyPartGroupDef linkedBodyPartsGroup;
        public SoundDef soundMeleeMiss;
        public List<ExtraDamage> extraMeleeDamages;
        public bool? alwaysTreatAsWeapon;
        public float? chanceFactor;
        public HediffDef hediff;
        public SoundDef soundMeleeHit;
        public float? cooldownTime;
        public float? armorPenetration;
        public float? power;
        public List<ToolCapacityDef> capacities;

        internal Tool ToTool(Tool originalTool=null)
        {
            var res = originalTool==null? new Tool(): Gen.MemberwiseClone<Tool>(originalTool);
            if (chanceFactor.HasValue)res.chanceFactor=chanceFactor.Value;
            if (power.HasValue) res.power = power.Value;
            if (cooldownTime.HasValue) res.cooldownTime = cooldownTime.Value;
            if (hediff != null) res.hediff = hediff;
            res.untranslatedLabel = untranslatedLabel;
            if(!string.IsNullOrWhiteSpace(label))res.label = label;
            if (capacities != null)res.capacities = capacities;
                
            

            if (armorPenetration.HasValue) res.armorPenetration = armorPenetration.Value;
            if (extraMeleeDamages != null) res.extraMeleeDamages = extraMeleeDamages;
            if (surpriseAttack != null) res.surpriseAttack = surpriseAttack;
            if (alwaysTreatAsWeapon.HasValue) res.alwaysTreatAsWeapon = alwaysTreatAsWeapon.Value;
            return res;
        }

        public bool labelUsedInLogging;
        public string untranslatedLabel;
        public SurpriseAttackProps surpriseAttack;

        public CustomTool() { }
        
        //Tool tool2 = Gen.MemberwiseClone<Tool>(tool);
    }
}
