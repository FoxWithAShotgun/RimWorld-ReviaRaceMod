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
        [DebugAction(category ="Revia debug", name ="Log verbs", actionType = DebugActionType.ToolMapForPawns,
            allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void ShowVerbs(Pawn p)
        {
            string verbs = string.Join(", ", p.VerbTracker.AllVerbs.Select(x => $"({x.GetType().Name}, {x.tool.LabelCap}, {x.tool.chanceFactor}, {x.GetDamageDef().LabelCap}, {x.HediffSource?.LabelCap}"));
            Log.Message(verbs);
        }

        

    }
}
