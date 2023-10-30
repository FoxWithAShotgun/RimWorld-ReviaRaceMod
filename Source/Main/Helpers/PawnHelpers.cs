﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Helpers
{
    public static class PawnHelpers
    {
        internal static bool HasTraits(this Pawn pawn) => pawn?.story?.traits != null;
        internal static bool HasTrait(this Pawn pawn, string defName) => pawn.story.traits.HasTrait(TraitDef.Named(defName));
        internal static bool IsMasochist(this Pawn pawn) => pawn.HasTraits() && pawn.HasTrait("Masochist");
        internal static bool IsBloodlust(this Pawn pawn) => pawn.HasTraits() && pawn.HasTrait("Bloodlust");
        internal static bool IsCannibal(this Pawn pawn) => pawn.HasTraits() && pawn.HasTrait("Cannibal");
        internal static bool IsPsychopath(this Pawn pawn) => pawn.HasTraits() && pawn.HasTrait("Psychopath");
        public static bool IsRevia(this Pawn pawn) => pawn?
            //.kindDef.race.defName
            //.ToLower().Contains("revia");
            .genes?.HasGene(Defs.Tail)??false;
        internal static bool IsHumanlike(this Pawn pawn) => pawn.RaceProps.Humanlike;
        internal static bool IsSkarnite(this Pawn pawn) => ModLister.IdeologyInstalled && (pawn?.ideo?.Ideo?.HasMeme(Defs.Skarnite) ?? false);
        internal static bool IsHediffActive(this Pawn pawn) => pawn.health.hediffSet.hediffs.Any(x => x.def.label.StartsWith("ReviaRaceSoulreap"));
        internal static Hediff SoulReapHediff(this Pawn pawn)=>pawn?.health.hediffSet.hediffs
                                         .FirstOrDefault(hediff => hediff.def.defName.StartsWith("ReviaRaceSoulreapTier"));
    }
}
