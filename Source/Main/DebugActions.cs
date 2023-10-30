using ReviaRace.Helpers;
using RimWorld;
using RimWorld.QuestGen;
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
                if (pawn.IsHumanlike() && pawn.IsRevia())
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

        [DebugAction(category = "Revia debug", name = "Test2", actionType = DebugActionType.Action,
   allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void Test2()
        {
            Slate slate = new Slate();
            var script = DefDatabase<QuestScriptDef>.GetNamed("ThreatReward_Raid_Joiner");
            if (script.IsRootDecree)
            {
                slate.Set<Pawn>("asker", PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists.RandomElement<Pawn>(), false);
            }

            Pawn pawn = slate.Get<Pawn>("asker", null, false);
            slate.Set<float>("points", 5000, false);
            if (!script.CanRun(slate))
            {
                Log.Error("Script cant be runned");
                return;
            }
            //if (pawn.royalty.AllTitlesForReading.NullOrEmpty<RoyalTitle>() && Faction.OfEmpire != null)
            //{
            //    pawn.royalty.SetTitle(Faction.OfEmpire, RoyalTitleDefOf.Knight, false, false, true);
            //    Messages.Message("DEV: Gave " + RoyalTitleDefOf.Knight.label + " title to " + pawn.LabelCap, pawn, MessageTypeDefOf.NeutralEvent, false);
            //}
            Find.CurrentMap.StoryState.RecordDecreeFired(script);
            for (int i = 0; i < 1600; i++)
            {
                var quest = QuestGen.Generate(script, slate);
                var qPartChoices = quest.PartsListForReading.Where(x => x is QuestPart_Choice).Cast<QuestPart_Choice>();
                if (qPartChoices.Any())
                {
                    //Log.Message()
                    if (qPartChoices.SelectMany(x => x.choices).SelectMany(x => x.rewards).Any(x => x is Reward_Pawn &&((x as Reward_Pawn).pawn.IsRevia()||(x as Reward_Pawn).pawn.health.hediffSet.hediffs.Any(x=>x.def.defName.Contains("Soul")))&& (x as Reward_Pawn).pawn.gender ==Gender.Male))
                    {
                        Find.QuestManager.Add(quest);
                        QuestUtility.SendLetterQuestAvailable(quest);
                    }
                }
                else
                    Log.Error("qPartChoices is empty!");
            }
        }
    }
}
