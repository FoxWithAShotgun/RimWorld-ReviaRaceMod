using HarmonyLib;
using ReviaRace.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class MapEventPatcher
    {
        static Harmony harmony = new Harmony("ReviaRace");
        static MapEventPatcher()
        {
            harmony.Patch(AccessTools.Method(typeof(MapPawns), "DoListChangedNotifications"),
                postfix: new HarmonyMethod(typeof(MapEventPatcher), nameof(RefreshListForSoulReaperComponent)));
        }

        public static void RefreshListForSoulReaperComponent(MapPawns __instance)
        {
            var map = __instance.AllPawns.FirstOrDefault()?.Map;
            if (map != null)
            {
                //Log.Message(nameof(RefreshListForSoulReaperComponent) + " called");
                map.GetComponent<MapSoulReaperComp>().pawnsChanged = true;
            }
        }
    }
}
