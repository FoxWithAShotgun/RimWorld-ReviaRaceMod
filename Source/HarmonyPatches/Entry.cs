using HarmonyLib;
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
        static Entry()
        {
            var harmony = new Harmony("ReviaRace");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
