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

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            var weapon = p.equipment?.Primary;
#if DEBUG
            Log.Message($"Weapon: {weapon.ToString()}");
#endif

            if (weapon == null || weapon.def.Verbs.Any(v => !v.IsMeleeAttack))
            {
                Messages.Message(Strings.SanctifyNonMeleeWeapon.Translate(), p, MessageTypeDefOf.RejectInput, false);
                return AcceptanceReport.WasRejected;
            }

            var llcp = weapon.GetComp<CompLifeLeech>();

            if (llcp != null &&
                llcp.LeechStrength > 0)
            {
                Messages.Message(Strings.SanctifyAlreadySanctified.Translate(), p, MessageTypeDefOf.RejectInput, false);
                return AcceptanceReport.WasRejected;
            }

            else if (!p.IsRevia() && !p.IsSkarnite())
            {
                Messages.Message(Strings.SanctifyNonReviaNonSkarnite.Translate(), p, MessageTypeDefOf.RejectInput, false); 
                return AcceptanceReport.WasRejected;
            }

            return AcceptanceReport.WasAccepted;
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
