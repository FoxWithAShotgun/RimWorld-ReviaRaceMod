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
            var weapon = p.equipment?.Primary;
#if DEBUG
            Log.Message($"Weapon: {weapon.ToString()}");
#endif
            if (weapon == null || weapon.def.Verbs.Any(v => !v.IsMeleeAttack))
            {
                failReason = Strings.SanctifyNonMeleeWeapon.Translate();
                return false;
            }

            var llcp = weapon.GetComp<CompLifeLeech>();

            if (llcp != null &&
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

            var weapon = usedBy.equipment.Primary;
            var llcp = weapon.GetComp<CompLifeLeech>();

            if (llcp == null)
            {
                llcp = new CompLifeLeech();
                weapon.AllComps.Add(llcp);
            }

            llcp.LeechStrength = GetSanctifyStrength();

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
