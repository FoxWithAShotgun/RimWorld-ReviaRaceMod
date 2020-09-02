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
        internal static TaleDef TaleSacrificed => DefDatabase<TaleDef>.GetNamed("ReviaRaceTaleSacrificed");

        // Thoughts
        internal static ThoughtDef SacrificedFear => DefDatabase<ThoughtDef>.GetNamed("ReviaRaceThoughtSacrificedFear");
        internal static ThoughtDef SacrificedNegative => DefDatabase<ThoughtDef>.GetNamed("ReviaRaceThoughtSacrificedNegative");
        internal static ThoughtDef SacrificedPositive => DefDatabase<ThoughtDef>.GetNamed("ReviaRaceThoughtSacrificedPositive");


        // Things
        internal static ThingDef BloodMote => DefDatabase<ThingDef>.GetNamed("Mote_FoodBitMeat");

        internal static ThingDef Bloodstone => DefDatabase<ThingDef>.GetNamed("ReviaRaceBloodstone");
        internal static ThingDef Palestone => DefDatabase<ThingDef>.GetNamed("ReviaRacePalestone");

        // Game Conditions
        internal static GameConditionDef Eclipse => DefDatabase<GameConditionDef>.GetNamed("Eclipse");
        internal static GameConditionDef SolarFlare => DefDatabase<GameConditionDef>.GetNamed("SolarFlare");

        // DamageDefs
        internal static DamageDef HeartExtraction => DefDatabase<DamageDef>.GetNamed("ReviaRaceSacrificeDamage");

        // Jobs
        internal static JobDef SacrificePrisoner => DefDatabase<JobDef>.GetNamed("ReviaRaceSacrificingPrisoner");
        internal static JobDef PrisonerWait => DefDatabase<JobDef>.GetNamed("ReviaRaceSacrificedPrisonerWait");
    }
}
