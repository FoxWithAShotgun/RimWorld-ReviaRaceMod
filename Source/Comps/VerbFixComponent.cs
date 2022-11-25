using ReviaRace.Helpers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Comps
{
    public class VerbFixComponent : GameComponent
    {
        public VerbFixComponent(Game game) : base()
        {
        }
        bool ShouldBeCached(Pawn pawn) => pawn?.genes != null && (pawn.genes.HasGene(Defs.Claws) || pawn.genes.HasGene(Defs.Teeth));
        /// <summary>
        /// Pawns with custom attack
        /// </summary>
        List<Pawn> pawns = new List<Pawn>();
        public override void FinalizeInit()
        {
            base.FinalizeInit();
            RefreshList();
            RefreshAllPawnsVerbs();
        }
        public override void LoadedGame()
        {
            base.LoadedGame();

        }
        public void AddPawn(Pawn pawn)
        {
            if (ShouldBeCached(pawn))
            {
                if (!pawns.Contains(pawn))
                    pawns.Add(pawn);
                RefreshPawnVerbs(pawn);
            }
        }
        public void RemovePawn(Pawn pawn)
        {
            var flag = ShouldBeCached(pawn);
            if (flag)
                pawns.Remove(pawn);
            RefreshPawnVerbs(pawn, true, flag);

        }
        void RefreshList()
        {
            pawns = PawnsFinder.AllMapsAndWorld_Alive.Where(x => ShouldBeCached(x)).ToList();
        }
        void RefreshAllPawnsVerbs()
        {
            foreach (var pawn in pawns)
                RefreshPawnVerbs(pawn);
        }

        public void RefreshPawnVerbs(Pawn pawn, bool shouldRefreshBefore = true, bool applyCustom = true)
        {
            if (shouldRefreshBefore)
                pawn.VerbTracker.InitVerbsFromZero();
            if (applyCustom)
            {
                if (pawn.genes.HasGene(Defs.Teeth))
                {
                    ReInitVerb(pawn, "teeth", "RevianTeeth");
                }
                if (pawn.genes.HasGene(Defs.Claws))
                {
                    ReInitVerb(pawn, "right fist", "RevianClaws");
                    ReInitVerb(pawn, "left fist", "RevianClaws");
                }
            }
            
        }
        void ReInitVerb(Pawn pawn, string originalVerbName, string newToolName)
        {
            var verb = pawn?.VerbTracker?.AllVerbs.FirstOrDefault(x => x.tool.untranslatedLabel.Equals(originalVerbName, StringComparison.OrdinalIgnoreCase));
            if (verb != null)
            {
                var tool = DefDatabase<CustomDefs.CustomTool>.GetNamed(newToolName).ToTool(verb?.tool);
                ReInitVerb(pawn,tool);
                pawn.verbTracker.AllVerbs.Remove(verb);
            }
        }
        void ReInitVerb(Pawn pawn,Tool newTool)
        {
            Func<Type, string, Verb> creator = delegate (Type type, string id)
            {
                Verb verb = (Verb)Activator.CreateInstance(type);
                pawn.verbTracker.AllVerbs.Add(verb);
                return verb;
            };
            foreach (ManeuverDef maneuverDef in newTool.Maneuvers)
            {
                try
                {
                    VerbProperties verb = maneuverDef.verb;
                    string text2 = Verb.CalculateUniqueLoadID(pawn, newTool, maneuverDef);
                    InitVerb(creator(verb.verbClass, text2), verb, newTool, maneuverDef, text2);
                }
                catch (Exception ex2)
                {
                    Log.Error(string.Concat(new object[]
                    {
                                "Could not instantiate Verb (directOwner=",
                                pawn.ToStringSafe<IVerbOwner>(),
                                "): ",
                                ex2
                    }));
                }
            }
            void InitVerb(Verb verb, VerbProperties properties, Tool tool, ManeuverDef maneuver, string id)
            {
                verb.loadID = id;
                verb.verbProps = properties;
                verb.verbTracker = pawn.verbTracker;
                verb.tool = tool;
                verb.maneuver = maneuver;
                verb.caster = (pawn as IVerbOwner).ConstantCaster;
            }
        }
    }
}
