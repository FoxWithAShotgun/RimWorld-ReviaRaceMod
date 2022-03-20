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
    public class CompProperties_LifeLeech : CompProperties
    {
        public float LeechStrength; // This was meant to be a property, but Tynan said fuck you, it needs to be a field.
        
        public CompProperties_LifeLeech()
        {
            this.compClass = typeof(CompLifeLeech);
        }
    }

    public class CompLifeLeech : ThingComp
    {
        private const string LifeLeechLabel = "LifeLeech_LeechStrength";
        public CompProperties_LifeLeech Props => this.props as CompProperties_LifeLeech;
        public float LeechStrength => Props.LeechStrength;

        public override string GetDescriptionPart()
        {
            return LeechStrength > 0
                ? Strings.SanctifyDescPart.Translate()
                : null;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            var defaultLeech = ReviaRaceMod.GetDefaultLifeLeech(parent.def.defName);
            if (defaultLeech > 0 &&
                defaultLeech != Props.LeechStrength)
            {
#if DEBUG
                Log.Message("Leech strength doesn't match what it should be. Resetting.");
#endif
                Props.LeechStrength = defaultLeech;
            }

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
#if DEBUG
                Log.Message("Not Revian or Skarnite");
#endif
                return;
            }
            if (victim == null || victim.Pawn == null)
            {
#if DEBUG
                Log.Message("No victim");
#endif
                return;
            }

            var props = this.props as CompProperties_LifeLeech;
            if (props.LeechStrength <= 0.0f)
            {
#if DEBUG
                Log.Message("Leech strength is zero.");
#endif
                return;
            }

            var bleedRate = victim.Pawn.health?.hediffSet?.BleedRateTotal ?? 0.0f;
            if (bleedRate == 0.0f)
            {
#if DEBUG
                Log.Message("Victim not bleeding");
#endif
                return;
            }

            var healAmount = bleedRate * 2.0f * props.LeechStrength;
            var bloodRegenAmount = bleedRate * 0.05f * props.LeechStrength;

#if DEBUG
            Log.Message($"Bleed Rate = {bleedRate}, Leech Strength = {props.LeechStrength}");
            Log.Message($"Heal = {healAmount}, Blood regen = {bloodRegenAmount}");
#endif
            ApplyHealing(pawn, healAmount, bloodRegenAmount);
        }

        public override void Notify_KilledPawn(Pawn pawn)
        {
            base.Notify_KilledPawn(pawn);

            var victim = pawn.LastAttackedTarget;

            if ((!pawn.IsRevia() && !pawn.IsSkarnite()) ||
                victim == null || victim.Pawn == null) 
            {
                return; 
            }

            var props = this.props as CompProperties_LifeLeech;

            if (props.LeechStrength > 0)
            {
#if DEBUG
                Log.Message("Applying kill healing burst");
#endif
                ApplyHealing(pawn, 15.0f, 0.08f);
            }
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

#if DEBUG
                Log.Message($"Healing {injury.Part} for {amount} points.");
#endif

                injury.Severity -= amount;
                physical -= amount;

                if (physical <= 0)
                {
                    break;
                }
            }
#if DEBUG
            Log.Message("No more injuries to heal.");
#endif
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            var defaultLeech = ReviaRaceMod.GetDefaultLifeLeech(parent.def.defName);
            if (defaultLeech != 0)
            {
                Props.LeechStrength = defaultLeech;
            }
            else
            {
                Scribe_Values.Look<float>(ref Props.LeechStrength, "LeechStrength", defaultLeech);
            }
#if DEBUG
            Log.Message($"{this.parent}: LeechStrength = {Props.LeechStrength}");
#endif
        }
    }
}
