using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using ReviaRace.Helpers;
using ReviaRace.Needs;

namespace ReviaRace.Comps
{
    public class SoulReaper : ThingComp
    {
        private int _lastAttackedTick = -1;
        private int _btTick = 0;
        private BloodthirstNeed _btNeed = null; // Caching to avoid frequent casting and searching of the need list. Iterators are bad for performance, mmkay?
        private bool _btDisabled = false;
        public override void CompTick()
        {
            if (_btDisabled)
            {
                return;
            }

            if (_btTick >= 10)
            {
                var pawn = parent as Pawn;
                _btNeed = _btNeed == null ? pawn.needs.TryGetNeed<BloodthirstNeed>() : _btNeed;

                if (_btNeed == null)
                {
                    _btDisabled = true;
                    return;
                }

                if (pawn.LastAttackTargetTick != _lastAttackedTick &&
                    pawn.LastAttackedTarget.Pawn is Pawn victim)
                {
                    _lastAttackedTick = pawn.LastAttackTargetTick;

                    if (victim.Dead)
                    {
                        // Skulls for the skull throne!
                        _btNeed.CurLevel += victim.BodySize * 0.80f;
                    }
                    else
                    {
                        // Blood for the blood god! 
                        var amount = 0.001f * victim.health.hediffSet.BleedRateTotal * victim.BodySize;
                        _btNeed.CurLevel += amount;
                    }
                }
            }
            else
            {
                _btTick++;
            }
            
            base.CompTick();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            var pawn = parent as Pawn;
            
            if (pawn != null && GetSoulReapTier() == -1)
            {
                if (pawn.kindDef == Defs.MarauderSkullshatterer ||
                    pawn.kindDef == Defs.TemplarHighTemplar)
                {
                    AddSoulReapTier(9);
                    pawn.skills.GetSkill(SkillDefOf.Melee).Level = 20;
                    pawn.skills.GetSkill(SkillDefOf.Shooting).Level = 20;
                    pawn.story.traits.allTraits.AddDistinct(new Trait(TraitDefOf.Tough));
                }
                else
                {
                    if (EnableRandomSoulReapTier)
                    {
                        // This should have at least tier 1 of soul reap.
                        // Add it now.
                        var rng = new Random();
                        AddSoulReapTier(rng.Next(SoulReapSpawnRange.min, SoulReapSpawnRange.max));
                    }
                    else
                    {
                        AddSoulReapTier(SoulReapSpawnFixed);
                    }
                }
            }
        }

        internal Hediff SoulReapHediff => (parent as Pawn)?.health.hediffSet.hediffs
                                         .FirstOrDefault(hediff => hediff.def.defName.Contains("ReviaRaceSoulreapTier"));
        internal static bool EnableRandomSoulReapTier { get; set; }
        internal static IntRange SoulReapSpawnRange { get; set; }
        internal static int SoulReapSpawnFixed { get; set; }

        internal int GetSoulReapTier()
        {
            if (SoulReapHediff != null)
            {
                string tierAsString = SoulReapHediff.def.defName.Replace("ReviaRaceSoulreapTier", "");
                return int.TryParse(tierAsString, out int tier) ? tier : -1;
            }
            else
            {
                return -1;
            }
        }

        internal void AddSoulReapTier(int tier)
        {
            var pawn = parent as Pawn;
            if (tier < 0 || tier > 9 || 
                pawn == null)
            {
                return;
            }
           
            HediffDef toAdd = HediffDef.Named($"ReviaRaceSoulreapTier{tier}");
            toAdd.initialSeverity = float.Epsilon;
            pawn.health.AddHediff(toAdd);
        }

        internal void RemoveSoulReapHediffs()
        {
            var pawn = parent as Pawn;
            if (pawn == null)
            {
                return;
            }

            var hediffsToRemove = pawn.health.hediffSet.hediffs.Where(hediff => hediff.def.defName.StartsWith("ReviaRaceSoulreapTier")).ToList();
            foreach (var hediff in hediffsToRemove)
            {
                pawn.health.RemoveHediff(hediff);
            }
        }
    }
}
