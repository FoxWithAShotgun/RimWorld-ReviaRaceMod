using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ReviaRace.Needs;
using RimWorld;
using Verse;

namespace ReviaRace.HarmonyPatches
{
    [HarmonyPatch(typeof(DamageWorker_AddInjury))]
    [HarmonyPatch(nameof(DamageWorker_AddInjury.Apply))]
    [HarmonyPatch(new Type[] { typeof(DamageInfo), typeof(Thing)})]
    class AddInjuryPatch
    {
        [HarmonyPostfix]
        static void Postfix(DamageInfo dinfo, Thing thing)
        {
            if (!BloodthirstNeed.Enabled)
            {
                return;
            }

            var attacker = dinfo.Instigator as Pawn;
            var victim = thing as Pawn;

            if (attacker != null && victim != null)
            {
                if (dinfo.Def == DamageDefOf.SurgicalCut)
                {
                    return;
                }

                var btNeed = attacker?.needs?.TryGetNeed<BloodthirstNeed>();
                if (btNeed == null)
                {
                    return;
                }

                if (victim.Dead)
                {
                    var amount = 0.50f * victim.BodySize;
                    btNeed.CurLevel += amount;
                }
                else
                {
                    var bleedRate = (victim.health?.hediffSet?.BleedRateTotal ?? 0.0f);
                    var amount = 0.0025f * bleedRate * victim.BodySize;
                    btNeed.CurLevel += amount;
                }
            }
        }
    }
}
