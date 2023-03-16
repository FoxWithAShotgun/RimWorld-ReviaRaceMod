using FacialAnimation;
using HarmonyLib;
using ReviaRace.Helpers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HeadTypeDef = FacialAnimation.HeadTypeDef;

namespace ReviaRace.HarmonyPatches
{

    [StaticConstructorOnStartup]
    public static class HarmonyPatch_FacialAnimation
    {
        static Harmony harmony = new Harmony("ReviaRace");
        internal static bool PatchActive { get; private set; } = false;
        static HarmonyPatch_FacialAnimation()
        {
            if(!FacialAnimationActive&&ReviaFacialAnimationActive)
            {
                Log.Error("Loaded Revia Facial Animation without [NL] Facial Animation - WIP!");
                return;
            }
            if (FacialAnimationActive && !ReviaFacialAnimationActive)
            {
                Log.Warning("Facial animation active, but there is no revia facial animation. There will be default");
                return;
            }
            if (FacialAnimationActive && ReviaFacialAnimationActive)
            {
                try
                {
                    var methodToPatch = AccessTools.Method("FacialAnimation.DrawFaceGraphicsComp:DrawBodyPart").MakeGenericMethod(typeof(IFacialAnimationController));//Unfortunatly, i cant directly patch ControllerBaseComp.InitIfNeeded
                    harmony.Patch(methodToPatch,
                                     prefix: new HarmonyMethod(typeof(HarmonyPatch_FacialAnimation), nameof(Prefix)));
                    PatchActive = true;
                    Log.Message("Succesfully patched [NL] Facial Animation - WIP for Revia biotech");
                }
                catch (Exception ex)
                {
                    Log.Warning("Something went wrong while patching [NL] Facial Animation - WIP\n"+ex.ToString());
                }
            }
        }
        internal static bool FacialAnimationActive =>
            ModLister.HasActiveModWithName("Combat Extended");
        internal static bool ReviaFacialAnimationActive =>
        ModLister.HasActiveModWithName("Combat Extended");
        public static void Prefix(ref int drawCount, ref object controller, bool isBottomLayer, ref Vector3 headOffset, Quaternion quaternion, Rot4 facing, bool portrait, bool headStump, RotDrawMode mode)
        {
            if (controller == null) return;
            var pawn = (controller as ThingComp).parent as Pawn;
            var faceTypeField = new Traverse(controller).Field("faceType");
            if (faceTypeField.GetValue() == null && pawn.IsRevia())
            {
                var typeOfFaceTypeDef = controller.GetType().BaseType.GetGenericArguments().First();
                var type1 = AccessTools.TypeByName("FacialAnimation.FaceTypeGenerator`1");
                var type2 = type1.MakeGenericType(typeOfFaceTypeDef);

                var method = AccessTools.Method(type2, "GetRandomDef");
                object result;
                try
                {
                    result = method.Invoke(null, new object[] { "ReviaRaceAlien", pawn.gender });
                }
                catch (Exception)
                {
                    result = method.Invoke(null, new object[] { "Human", pawn.gender });
                }
                faceTypeField.SetValue(result);

            }

        }

        internal static void ResetFaceType(Pawn pawn)
        {
            foreach (var item in pawn.AllComps.Where(x =>x is IFacialAnimationController))
            {
                new Traverse(item).Field("faceType").SetValue(null);
                new Traverse(item).Field("pawn").SetValue(null);
                new Traverse(item).Method("SetDirty").GetValue();
            }
            PortraitsCache.SetDirty(pawn);
            
        }
    }
}
