using HarmonyLib;
using ReviaRace.Enums;
using ReviaRace.Helpers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class HeredityPatcher
    {
        static Harmony harmony = new Harmony("ReviaRace");
        static HeredityPatcher()
        {
            harmony.Patch(AccessTools.Method(typeof(PregnancyUtility), nameof(PregnancyUtility.ApplyBirthOutcome)),
               transpiler: new HarmonyMethod(typeof(HeredityPatcher), nameof(ApplyBirthOutcomeTranspiler)));
            harmony.Patch(AccessTools.Method(typeof(PregnancyUtility), nameof(PregnancyUtility.GetInheritedGeneSet),parameters:new Type[] {typeof(Pawn),typeof(Pawn),typeof(bool).MakeByRefType() }),
                 postfix: new HarmonyMethod(typeof(HeredityPatcher), nameof(GetInheritedGeneSetPostfix)));
            harmony.Patch(AccessTools.Method(typeof(PregnancyUtility), "ShouldByHybrid"),
                 postfix: new HarmonyMethod(typeof(HeredityPatcher), nameof(ShouldByHybridPostfix)));
            harmony.Patch(AccessTools.Method(typeof(PregnancyUtility), "TryGetInheritedXenotype"),
                 postfix: new HarmonyMethod(typeof(HeredityPatcher), nameof(TryGetInheritedXenotypePostfix)));

        }
        public static IEnumerable<CodeInstruction> ApplyBirthOutcomeTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            Log.Message(nameof(ApplyBirthOutcomeTranspiler) + " called");
            foreach (var ci in instructions)
            {
                yield return ci;
                if (ci.opcode == OpCodes.Initobj && ci.operand.ToString().Contains("Gender"))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 4);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 15);
                    yield return CodeInstruction.Call(typeof(HeredityPatcher), nameof(SelectGender));
                    yield return new CodeInstruction(OpCodes.Stloc_S, 15);
                }
            }
        }
        public static Gender? SelectGender(Pawn mother, Gender? gender)
        {
            if (mother.IsRevia() && gender == null&&StaticModVariables.BornSettings!=BornSettingsEnum.Default)
                gender = Gender.Female;
            return gender;
        }

        public static void GetInheritedGeneSetPostfix(Pawn father, Pawn mother, ref bool success, ref GeneSet __result)
        {
            var bornSettings = StaticModVariables.BornSettings;
            if (success && mother.IsRevia() && bornSettings == BornSettingsEnum.ForceBornRevia)
                    ApplyReviaXenotype(mother, __result);

        }
        private static void ApplyReviaXenotype(Pawn mother, GeneSet genes)
        {
            foreach (var geneDef in Defs.XenotypeDef.AllGenes)
                genes.AddGene(geneDef);
            
            genes.SetNameDirect(mother.genes.xenotypeName);
        }
        public static void ShouldByHybridPostfix(Pawn mother,Pawn father,ref bool __result)
        {
            if (__result && mother.IsRevia() && StaticModVariables.BornSettings == BornSettingsEnum.ForceBornRevia&&(father==null|| father.genes.Xenotype==DefDatabase<XenotypeDef>.GetNamed("Baseliner")))
                __result = false;
        }
        public static void TryGetInheritedXenotypePostfix(Pawn mother, Pawn father, ref XenotypeDef xenotype,ref bool __result)
        {
            if (mother?.IsRevia() ?? false)
            {
                xenotype = Defs.XenotypeDef;
                __result = true;
            }
        }
    }
}
