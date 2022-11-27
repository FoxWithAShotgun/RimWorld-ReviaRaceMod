using HarmonyLib;
using ReviaRace.Helpers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.HarmonyPatches
{
    [StaticConstructorOnStartup]
    internal static class Entry
    {
        private static readonly Type patchType = typeof(Entry);
        static Entry()
        {
            var harmony = new Harmony("ReviaRace");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            harmony.Patch(AccessTools.Method(typeof(WorkGiver_Researcher), nameof(WorkGiver_Researcher.ShouldSkip)),
                postfix: new HarmonyMethod(patchType, nameof(ShouldSkipResearchPostfix)));
            harmony.Patch(AccessTools.Method(typeof(Bill), nameof(Bill.PawnAllowedToStartAnew)),
                postfix: new HarmonyMethod(patchType, nameof(PawnAllowedToStartAnewPostfix)));
        }
        private static bool CanDoRecipe(Pawn pawn, RecipeDef recipe)
        {
            if (recipe.defName.StartsWith("Revia"))
                return pawn.IsRevia();
            return true;
        }
        
        public static void PawnAllowedToStartAnewPostfix(Pawn p, Bill __instance, ref bool __result)
        {
            RecipeDef recipe = __instance.recipe;

            if (__result&&recipe!=null)
                __result = CanDoRecipe(p, recipe);
        }

        public static void ShouldSkipResearchPostfix(Pawn pawn, ref bool __result)
        {
            if (__result) return;
            ResearchProjectDef project = Find.ResearchManager?.currentProj;

            if (project?.defName?.StartsWith("Revia")??false) __result = !pawn.IsRevia();
        }
    }
}
