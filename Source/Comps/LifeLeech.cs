using ReviaRace.Helpers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Comps
{
    public class LifeLeech_CompProperties : CompProperties
    {
        public LifeLeech_CompProperties() : 
            base(typeof(LifeLeech))
        {
            
        }

        public float LeechStrength { get; set; }
    }

    public class LifeLeech : ThingComp
    {
        private const string LifeLeechLabel = "LifeLeech_LeechStrength";

        public override string GetDescriptionPart()
        {
            var props = this.props as LifeLeech_CompProperties;

            return props.LeechStrength > 0
                ? Strings.SanctifyDescPart.Translate()
                : null;
        }

        public override void Notify_UsedWeapon(Pawn pawn)
        {
            if (pawn == null)
            {
                return;
            }

            base.Notify_UsedWeapon(pawn);
            var victim = pawn.LastAttackedTarget;

            if (!pawn.IsRevia() && !pawn.IsSkarnite())
            {
                return;
            }
            if (victim == null || victim.Pawn == null)
            {
                return;
            }

            var bleedRate = victim.Pawn.health?.hediffSet?.BleedRateTotal ?? 0.0f;
            if (bleedRate == 0.0f)
            {
                return;
            }

            var props = this.props as LifeLeech_CompProperties;
            var healAmount = bleedRate * 2.0f * props.LeechStrength;
            var bloodRegenAmount = bleedRate * 0.05f * props.LeechStrength;

            ApplyHealing(pawn, healAmount, bloodRegenAmount);
        }

        public override void Notify_KilledPawn(Pawn pawn)
        {
            base.Notify_KilledPawn(pawn);

            var victim = pawn.LastAttackedTarget;

            if (!pawn.IsRevia()) return;
            if (victim == null || victim.Pawn == null) return;
            
            ApplyHealing(pawn, 15.0f, 0.08f);
        }

        private void ApplyHealing(Pawn pawn, float physical, float bleed)
        {
            var bloodLossHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            if (bloodLossHediff != null)
            {
                bloodLossHediff.Severity -= bleed;
            }

            var injuries = pawn.health.hediffSet.hediffs.Select(hd => hd as Hediff_Injury).Where(injury => injury != null && !injury.IsPermanent());
            foreach (var injury in injuries)
            {
                var amount = Math.Min(physical, injury.Severity);

                injury.Severity -= amount;
                physical -= amount;

                if (physical <= 0)
                {
                    break;
                }
            }
        }
    }
}
