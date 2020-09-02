using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Helpers
{
    internal static class PawnHelpers
    {
        internal static bool HasTraits(this Pawn pawn) => pawn?.story?.traits != null;
        internal static bool HasTrait(this Pawn pawn, string defName) => pawn.story.traits.HasTrait(TraitDef.Named(defName));
        internal static bool IsMasochist(this Pawn pawn) => pawn.HasTraits() && pawn.HasTrait("Masochist");
        internal static bool IsBloodlust(this Pawn pawn) => pawn.HasTraits() && pawn.HasTrait("Bloodlust");
        internal static bool IsCannibal(this Pawn pawn) => pawn.HasTraits() && pawn.HasTrait("Cannibal");
        internal static bool IsPsychopath(this Pawn pawn) => pawn.HasTraits() && pawn.HasTrait("Psychopath");
        internal static bool IsRevia(this Pawn pawn) => pawn.kindDef.race.defName.ToLower().Contains("revia");
        internal static bool IsHumanlike(this Pawn pawn) => pawn.RaceProps.Humanlike;
    }
}
