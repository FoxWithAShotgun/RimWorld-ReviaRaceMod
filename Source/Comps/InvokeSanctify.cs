using ReviaRace.Helpers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Comps
{
    public abstract class InvokeSanctify : CompUseEffect
    {
        public abstract float GetSanctifyStrength();

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            var llComp = p.equipment?.PrimaryEq?.parent?.TryGetComp<LifeLeech>();

            if (llComp == null)
            {
                failReason = Strings.SanctifyNonMeleeWeapon.Translate();
                return false;
            }
            else if (llComp.props is LifeLeech_CompProperties llcp &&
                llcp.LeechStrength > 0)
            {
                failReason = Strings.SanctifyAlreadySanctified.Translate();
                return false;
            }
            else if (!p.IsRevia() && !p.IsSkarnite())
            {
                failReason = Strings.SanctifyNonReviaNonSkarnite.Translate();
                return false;
            }

            failReason = null;
            return true;
        }

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            
            var llComp = usedBy.equipment.PrimaryEq.parent.TryGetComp<LifeLeech>();
            (llComp.props as LifeLeech_CompProperties).LeechStrength = GetSanctifyStrength();

            parent.stackCount--;
            if (parent.stackCount <= 0)
            {
                parent.Destroy();
            }
        }
    }

    public class InvokeLesserSanctify : InvokeSanctify
    {
        public override float GetSanctifyStrength() => 0.50f;
    }

    public class InvokeModerateSanctify : InvokeSanctify
    {
        public override float GetSanctifyStrength() => 1.00f;
    }

    public class InvokeGreaterSanctify : InvokeSanctify
    {
        public override float GetSanctifyStrength() => 1.75f;
    }
}
