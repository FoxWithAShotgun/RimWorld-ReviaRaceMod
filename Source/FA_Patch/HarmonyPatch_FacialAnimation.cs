using FacialAnimation;
using HarmonyLib;
using ReviaRace.Helpers;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;


namespace ReviaRace.FA_Compat
{

    [StaticConstructorOnStartup]
    public static class HarmonyPatch_FacialAnimation
    {
        static Harmony harmony = new Harmony("ReviaRace_FA");

        static HarmonyPatch_FacialAnimation()
        {
            LongEventHandler.ExecuteWhenFinished(() => harmony.PatchAll());
        }
        public static IEnumerable<T> GetFaceTypeDefsForPawn<T>(Pawn pawn, bool includeDefault = true) where T : FaceTypeDef, new()
        {
            IEnumerable<T> result = new List<T>();
            if (pawn.IsRevia())
            {
                try
                {
                    result = result.Union(FaceTypeGenerator<T>.GetFaceTypeDefsForRace("ReviaRaceAlien", pawn.gender));
                }
                catch
                { 
                }
            }
            if (!result.Any() || includeDefault)
            {
                result = result.Union(FaceTypeGenerator<T>.GetFaceTypeDefsForRace(pawn.def.defName, pawn.gender));
            }
            return result;
        }
        public static T GetRandomDef<T>(Pawn pawn, bool includeDefault = false) where T : FaceTypeDef, new()
        {
            IEnumerable<T> faceTypeDefsForRace = GetFaceTypeDefsForPawn<T>(pawn, includeDefault);
            float num = faceTypeDefsForRace.Sum((T x) => x.probability);
            float num2 = Rand.Range(0f, 1f) * num;
            foreach (T t in faceTypeDefsForRace)
            {
                if (num2 < t.probability)
                {
                    return t;
                }
                num2 -= t.probability;
            }
            return faceTypeDefsForRace.First<T>();
        }
        private static MethodInfo _GetRandomDef_MB = null;
        internal static MethodInfo GetRandomDef_MB => _GetRandomDef_MB ??= typeof(HarmonyPatch_FacialAnimation).GetMethod(nameof(GetRandomDef));
        public static FaceTypeDef GetRandomDefByComp(ThingComp comp, Pawn pawn, bool includeDefault = false)
        {
            return GetRandomDef_MB.MakeGenericMethod(comp.GetType().BaseType.GetGenericArguments()[0]).Invoke(null, new object[] { pawn, false }) as FaceTypeDef;
        }
    }

    [HarmonyPatch(typeof(ControllerBaseComp<FaceTypeDef, BrowShapeDef>), "InitIfNeed")]
    internal static class ControllerBaseComp_Patch
    {
        public static bool Prefix(ThingComp __instance, ref Pawn ___pawn, ref Gender ___prevGender, ref Color ___color, ref FaceTypeDef ___faceType, Thing ___parent)
        {
            if (___pawn != null)
            {
                return false;
            }
            ___pawn = (___parent as Pawn);
            ___prevGender = ___pawn.gender;
            if (___faceType == null)
            {
                ___faceType = HarmonyPatch_FacialAnimation.GetRandomDefByComp(__instance, ___pawn);
            }
            if (__instance.GetCurrentColor() == Color.clear)
            {
                ___color = __instance.ResetColor();
            }
            return false;
        }
        internal static Color GetCurrentColor(this ThingComp instance)
        {
            return (Color)(new Traverse(instance).Method("GetCurrentColor").GetValue());
        }
        internal static Color ResetColor(this ThingComp instance)
        {
            return (Color)(new Traverse(instance).Method("ResetColor").GetValue());
        }
    }
    [HarmonyPatch(typeof(NL_SelectPartWindow), nameof(NL_SelectPartWindow.DrawAnimationPawnParamFacial))]
    internal static class NL_SelectPartWindow_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var toReturn = instructions.ToList();
            for (int i = 0; i < toReturn.Count; i++)
            {
                if (i + 2 < toReturn.Count && toReturn[i].opcode == OpCodes.Ldarg_1 && toReturn[i + 1].LoadsField(AccessTools.Field(typeof(Thing), nameof(Thing.def))) && toReturn[i + 2].LoadsField(AccessTools.Field("Verse.Def:defName")))
                {
                    var generic = (toReturn[i + 5].operand as MethodBase).DeclaringType.GetGenericArguments()[0];
                    toReturn[i + 1] = new CodeInstruction(OpCodes.Ldc_I4_1);
                    toReturn[i + 2] = CodeInstruction.Call(typeof(HarmonyPatch_FacialAnimation), nameof(HarmonyPatch_FacialAnimation.GetFaceTypeDefsForPawn), generics: new Type[] { generic });
                    for (int y = 0; y < 3; y++)
                    {
                        toReturn.RemoveAt(i + 3);
                    }
                }
            }
            return toReturn;
        }
    }
}
