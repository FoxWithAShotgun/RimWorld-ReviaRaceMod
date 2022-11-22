using ReviaRace.Needs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Helpers
{
    internal class ReviaComp
    {
        internal ReviaComp(Pawn pawn)
        {
            this.pawn = pawn;
        }
        public Pawn pawn { private set; get; }
        private int _lastAttackedTick = -1;
        private int _btTick = 0;
        private BloodthirstNeed _btNeed = null; // Caching to avoid frequent casting and searching of the need list. Iterators are bad for performance, mmkay?
        private bool _btDisabled = false;
        public void CompTick()
        {
            if (_btDisabled)
            {
                return;
            }

            if (_btTick >= 10)
            {
                
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

        }
    }
}
