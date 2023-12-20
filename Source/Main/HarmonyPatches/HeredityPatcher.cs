using HarmonyLib;
using ReviaRace.Enums;
using ReviaRace.Helpers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                transpiler: new HarmonyMethod(typeof(HeredityPatcher), nameof(ApplyBirthOutcomeTranspiler)),
                prefix: new HarmonyMethod(typeof(HeredityPatcher), nameof(ApplyBirthOutcomePrefix)),
                postfix: new HarmonyMethod(typeof(HeredityPatcher), nameof(ApplyBirthOutcomePostfix))
                );
            harmony.Patch(AccessTools.Method(typeof(PregnancyUtility), nameof(PregnancyUtility.GetInheritedGeneSet), parameters: new Type[] { typeof(Pawn), typeof(Pawn), typeof(bool).MakeByRefType() }),
                 postfix: new HarmonyMethod(typeof(HeredityPatcher), nameof(GetInheritedGeneSetPostfix)));
            harmony.Patch(AccessTools.Method(typeof(PregnancyUtility), "ShouldByHybrid"),
                 postfix: new HarmonyMethod(typeof(HeredityPatcher), nameof(ShouldByHybridPostfix)));
            harmony.Patch(AccessTools.Method(typeof(PregnancyUtility), "TryGetInheritedXenotype"),
                 postfix: new HarmonyMethod(typeof(HeredityPatcher), nameof(TryGetInheritedXenotypePostfix)));
            harmony.Patch(AccessTools.Method(typeof(PregnancyUtility), nameof(PregnancyUtility.GetInheritedGenes), parameters: new Type[] { typeof(Pawn), typeof(Pawn), typeof(bool).MakeByRefType() }),
                transpiler: new HarmonyMethod(typeof(HeredityPatcher), nameof(GetInheritedGenesTranspiler)));

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
        public static void ApplyBirthOutcomePrefix()
        {
            StaticModVariables.BirthOutcome = true;
        }
        public static void ApplyBirthOutcomePostfix()
        {
            StaticModVariables.BirthOutcome = false;
        }
        public static Gender? SelectGender(Pawn mother, Gender? gender)
        {
            if (mother.IsRevia() && gender == null && StaticModVariables.BornSettings != BornSettingsEnum.Default)
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
        public static void ShouldByHybridPostfix(Pawn mother, Pawn father, ref bool __result)
        {
            if (__result && mother.IsRevia() && StaticModVariables.BornSettings == BornSettingsEnum.ForceBornRevia && (StaticModVariables.NoHybrid || father == null || father.genes.Xenotype == DefDatabase<XenotypeDef>.GetNamed("Baseliner")))
                __result = false;
        }
        public static void TryGetInheritedXenotypePostfix(Pawn mother, Pawn father, ref XenotypeDef xenotype, ref bool __result)
        {
            if (StaticModVariables.BornSettings == BornSettingsEnum.ForceBornRevia && (mother?.IsRevia() ?? false))
            {
                xenotype = Defs.XenotypeDef;
                __result = true;
            }
        }
        public static IEnumerable<CodeInstruction> GetInheritedGenesTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo field = AccessTools.Field("Verse.GeneDef:biostatArc");
            List<CodeInstruction> _instructions = instructions.ToList();
            for (int i = 0; i < _instructions.Count; i++)
            {
                var current = _instructions[i];
                if (current.LoadsField(field))
                {
                    Label label = (Label)_instructions[i + 2].operand;
                    _instructions.Insert(i + 3, new CodeInstruction(OpCodes.Ldarg_1));
                    _instructions.Insert(i + 4, CodeInstruction.Call(typeof(HeredityPatcher), nameof(InheritFatherGenes)));
                    _instructions.Insert(i + 5, new CodeInstruction(OpCodes.Brfalse_S, label));
                    break;
                }
            }
            return _instructions;
        }
        public static bool InheritFatherGenes(Pawn mother)
        {
            if((mother?.IsRevia() ?? false) && StaticModVariables.BornSettings == BornSettingsEnum.ForceBornRevia)
            {
                return !StaticModVariables.NoHybrid;
            }
            return true;
        }
    }
}
