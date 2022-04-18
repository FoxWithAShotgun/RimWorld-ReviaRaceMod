using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ReviaRace.Needs
{
    public class BloodthirstNeed : Need
    {
        public BloodthirstNeed(Pawn pawn) 
            : base(pawn)
        {
            threshPercents = Thresholds;
        }
        public static bool Enabled { get; set; }

        public static float DaysToEmpty 
        {
            get => _daysToEmpty;
            set 
            {
                _daysToEmpty = value;
                DecayPerDay = 1.0f / DaysToEmpty;
            }
        }
        private static float _daysToEmpty;

        public static readonly List<float> Thresholds = new List<float>()
        {
            ThreshDesperate,
            ThreshAgitated,
            ThreshFrustrated,
            ThreshItching,
            ThreshSatisfied,
        };

        public override float CurLevel 
        {
            get => base.CurLevel;
            set => base.CurLevel = Math.Min(value, 1.0f); 
        }

        private static float DecayPerDay { get; set; }
        public static int TickMultTimer => 10;

        public static float ThreshDesperate => 0.00f;
        public static float ThreshAgitated => 0.10f;
        public static float ThreshFrustrated => 0.30f;
        public static float ThreshItching => 0.50f;
        public static float ThreshSatisfied => 0.70f;

        public bool PawnAffected => !pawn.WorkTagIsDisabled(WorkTags.Violent) && pawn.ageTracker.CurLifeStageIndex > 1;
        public float PawnAffectedMult => PawnAffected ? 1.0f : 0.0f;
        public float PawnRedHazeMult => pawn.health.hediffSet.HasHediff(HediffDef.Named("ReviaRaceRedHazeAddiction")) ? 1.5f : 1.0f;

        private float GetFallPerTick()
        {
            return (DecayPerDay / 60000.0f) * PawnAffectedMult * PawnRedHazeMult;
        }
        private int curTick = 0;
        
        public override void NeedInterval()
        {
            if (!Enabled || pawn.Map == null)
            {
                return;
            }

            curTick++;

            if (curTick >= 10)
            {
                curTick = 0;

                if (!PawnAffected)
                {
                    CurLevel = ThreshItching;
                    return;
                }

                if (!def.freezeWhileSleeping || pawn.Awake())
                {
                    // 150 ticks per NeedInterval call. 
                    CurLevel -= 150 * TickMultTimer * GetFallPerTick();
                }
            }
        }
    }
}
