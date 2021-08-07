using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace ReviaRace.Helpers
{
    internal static class Defs
    {
        // Tales
        internal static TaleDef TaleSacrificed => _taleSacrificed ??= DefDatabase<TaleDef>.GetNamed("ReviaRaceTaleSacrificed");
        private static TaleDef _taleSacrificed;

        // Thoughts
        internal static ThoughtDef SacrificedFear => _sacrificedFear ??= DefDatabase<ThoughtDef>.GetNamed("ReviaRaceThoughtSacrificedFear");
        private static ThoughtDef _sacrificedFear;
        internal static ThoughtDef SacrificedNegative => _sacrificedNegative ??= DefDatabase<ThoughtDef>.GetNamed("ReviaRaceThoughtSacrificedNegative");
        private static ThoughtDef _sacrificedNegative;
        internal static ThoughtDef SacrificedPositive => _sacrificedPositive ??= DefDatabase<ThoughtDef>.GetNamed("ReviaRaceThoughtSacrificedPositive");
        private static ThoughtDef _sacrificedPositive;

        // Pawn kinds
        internal static PawnKindDef MarauderSkullshatterer => _marauderSkullshatterer ??= DefDatabase<PawnKindDef>.GetNamed("ReviaRaceSkullshatterer");
        private static PawnKindDef _marauderSkullshatterer;
        internal static PawnKindDef TemplarHighTemplar => _templarHighTemplar ??= DefDatabase<PawnKindDef>.GetNamed("ReviaRaceHighTemplar");
        private static PawnKindDef _templarHighTemplar;
        // Things
        internal static ThingDef BloodMote => _bloodMote ??= DefDatabase<ThingDef>.GetNamed("Mote_FoodBitMeat");
        private static ThingDef _bloodMote;

        internal static ThingDef Bloodstone => _bloodstone ??= DefDatabase<ThingDef>.GetNamed("ReviaRaceBloodstone");
        private static ThingDef _bloodstone;
        internal static ThingDef Palestone => _palestone ??= DefDatabase<ThingDef>.GetNamed("ReviaRacePalestone");
        private static ThingDef _palestone;

        // Game Conditions
        internal static GameConditionDef Eclipse => _eclipse ??= DefDatabase<GameConditionDef>.GetNamed("Eclipse");
        private static GameConditionDef _eclipse;
        internal static GameConditionDef SolarFlare => _solarFlare ??= DefDatabase<GameConditionDef>.GetNamed("SolarFlare");
        private static GameConditionDef _solarFlare;

        // DamageDefs
        internal static DamageDef HeartExtraction => _heartExtraction ??= DefDatabase<DamageDef>.GetNamed("ReviaRaceSacrificeDamage");
        private static DamageDef _heartExtraction;

        // Jobs
        internal static JobDef SacrificePrisoner => _sacrificePrisoner ??= DefDatabase<JobDef>.GetNamed("ReviaRaceSacrificingPrisoner");
        private static JobDef _sacrificePrisoner;
        internal static JobDef PrisonerWait => _prisonerWait ??= DefDatabase<JobDef>.GetNamed("ReviaRaceSacrificedPrisonerWait");
        private static JobDef _prisonerWait;

        // Ideology
        internal static MemeDef Skarnite => _skarnite ??= DefDatabase<MemeDef>.GetNamed("ReviaRaceSkarniteMeme");
        private static MemeDef _skarnite;
    }
}
