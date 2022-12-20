using ReviaRace.Helpers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace
{
    public static class DebugActions
    {
        [DebugAction(category = "Revia debug", name = "Log verbs", actionType = DebugActionType.ToolMapForPawns,
            allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void ShowVerbs(Pawn p)
        {
            string verbs = string.Join(", ", p.VerbTracker.AllVerbs.Select(x => $"({x.GetType().Name}, {x.tool.LabelCap}, {x.tool.chanceFactor}, {x.GetDamageDef().LabelCap}, {x.HediffSource?.LabelCap}"));
            Log.Message(verbs);
        }

        [DebugAction(category = "Revia debug", name = "Destroy not revia", actionType = DebugActionType.ToolMap,
    allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void DestroyNotIntresting()
        {
            foreach (var pawn in Find.CurrentMap.mapPawns.AllPawns.ToList())
            {
                if (!pawn.IsRevia() && (!pawn?.story?.AllBackstories.Any(x => x.defName.StartsWith("Revia")) ?? false))
                {
                    pawn.Destroy();
                }
            }
        }
        [DebugAction(category = "Revia debug", name = "Test1", actionType = DebugActionType.Action,
    allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void Test1()
        {
            var inc = DefDatabase<IncidentDef>.GetNamed("WildManWandersIn");
            for (int i = 0; i < 400; i++)
            {
                Log.Clear();
                try
                {
                    inc.Worker.TryExecute(new IncidentParms() { target = Find.CurrentMap });
                }
                catch (Exception)
                {

                    return;
                }
            }
            Log.Clear();
            foreach (var pawn in Find.CurrentMap.mapPawns.AllPawns.ToList())
            {
                if (pawn.IsHumanlike()&&pawn.IsRevia())
                {
                    if (pawn.relations.DirectRelations.Any(x => x.def.defName == "Parent" && x.otherPawn.gender == Gender.Male && x.otherPawn.genes.Xenotype == Defs.XenotypeDef))
                    {
                        Log.Message($"pawn {pawn.Name.ToStringFull} parent is revia");
                        continue;
                    }
                }
                pawn.Destroy();
            }
        }
    }
}
