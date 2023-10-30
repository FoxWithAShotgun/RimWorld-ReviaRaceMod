using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ReviaRace.Comps
{
    public class ReviaRaceOnSpawn : ThingComp
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            Log.Message("Something happend " + respawningAfterLoad);
            base.PostSpawnSetup(respawningAfterLoad);

            Pawn pawn = parent as Pawn;
            if (pawn == null)
            {
                // Only applies against pawns.
                return;
            }

            // All Revia must have bloodlust.
            if (!pawn.story.traits.allTraits.Any(trait => trait.def == TraitDefOf.Bloodlust))
            {
                pawn.story.traits.allTraits.Add(new Trait(TraitDefOf.Bloodlust));
            }

            // All Revia must not have Wimp.
            var wimpTrait = pawn.story.traits.allTraits.FirstOrDefault(trait => trait.def == TraitDef.Named("Wimp"));
            if (wimpTrait != null)
            {
                pawn.story.traits.allTraits.Remove(wimpTrait);
            }

            // All Revia must be female.
            if (pawn.gender == Gender.Male)
            {
                pawn.gender = Gender.Female;
                var random = new Random();
            }
        }
        public MyExampleCompProperties Props => (MyExampleCompProperties)this.props;
    }
    public class MyExampleCompProperties : CompProperties
    {
        public bool myExampleBool;
        public float myExampleFloat;

        /// <summary>
        /// These constructors aren't strictly required if the compClass is set in the XML.
        /// </summary>
        public MyExampleCompProperties()
        {
            this.compClass = typeof(ReviaRaceOnSpawn);
        }

        public MyExampleCompProperties(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }
}
