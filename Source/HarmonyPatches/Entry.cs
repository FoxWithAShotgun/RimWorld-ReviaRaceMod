using HarmonyLib;
using ReviaRace.Genes;
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
        internal static bool NoProjectLimitations { get; set; }
        internal static bool NoCraftLimitation { get; set; }
        private static readonly Type patchType = typeof(Entry);
        static Entry()
        {
            var harmony = new Harmony("ReviaRace");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            harmony.Patch(AccessTools.Method(typeof(WorkGiver_Researcher), nameof(WorkGiver_Researcher.ShouldSkip)),
                postfix: new HarmonyMethod(patchType, nameof(ShouldSkipResearchPostfix)));
            harmony.Patch(AccessTools.Method(typeof(Bill), nameof(Bill.PawnAllowedToStartAnew)),
                postfix: new HarmonyMethod(patchType, nameof(PawnAllowedToStartAnewPostfix)));
            harmony.Patch(AccessTools.Method(typeof(Hediff), nameof(Hediff.PostAdd)),
                postfix: new HarmonyMethod(patchType, nameof(HediffPostAdd)));
            harmony.Patch(AccessTools.Method(typeof(Hediff), nameof(Hediff.PostRemoved)),
                postfix: new HarmonyMethod(patchType, nameof(HediffPostRemove)));
        }

        private static void HediffPostRemove(Hediff __instance)
        {
            if (__instance.pawn == null) return;
            ReviaBaseGene.RefreshAttackHediffs(__instance.pawn);
        }
        private static void HediffPostAdd(Hediff __instance)
        {
            if (__instance.pawn == null) return;
            ReviaBaseGene.RefreshAttackHediffs(__instance.pawn);
        }
        private static bool CanDoRecipe(Pawn pawn, RecipeDef recipe)
        {
            if ((recipe.defName.StartsWith("Revia")||(recipe?.ProducedThingDef?.defName?.StartsWith("Revia")??false)))
                return pawn.IsRevia();
            return true;
        }

        public static void PawnAllowedToStartAnewPostfix(Pawn p, Bill __instance, ref bool __result)
        {
            if (NoCraftLimitation) return;
            RecipeDef recipe = __instance.recipe;

            if (__result && recipe != null)
                __result = CanDoRecipe(p, recipe);
        }

        public static void ShouldSkipResearchPostfix(Pawn pawn, ref bool __result)
        {
            if (NoProjectLimitations) return;
            if (__result) return;
            ResearchProjectDef project = Find.ResearchManager?.currentProj;

            if (project?.defName?.StartsWith("Revia") ?? false) __result = !pawn.IsRevia();
        }

    }
}
