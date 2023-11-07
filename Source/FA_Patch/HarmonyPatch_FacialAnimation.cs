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
            Log.Warning("Something called this patch");
            LongEventHandler.ExecuteWhenFinished(() => harmony.PatchAll());
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
                var race = ___pawn.IsRevia() ? "ReviaRaceAlien" : ___pawn.def.defName;
                ___faceType = __instance.GetRandomDef(race, ___pawn.def.defName, ___pawn.gender);
            }
            if (__instance.GetCurrentColor() == Color.clear)
            {
                ___color = __instance.ResetColor();
            }
            return false;
        }
        internal static FaceTypeDef GetRandomDef(this ThingComp instance, string race, string defaultRace, Gender gender)
        {
            var faceTypeGenerator = typeof(FaceTypeGenerator<>).MakeGenericType(instance.GetType().BaseType.GetGenericArguments()[0]);
            try
            {
                return faceTypeGenerator.GetMethod("GetRandomDef").Invoke(null, new object[] { race, gender }) as FaceTypeDef;
            }
            catch
            {
                return faceTypeGenerator.GetMethod("GetRandomDef").Invoke(null, new object[] { defaultRace, gender }) as FaceTypeDef;
            }
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
}
