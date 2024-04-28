using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.Workers
{
    public class SacrificeWorker : RecipeWorker
    {
        public static bool EnableCorpseStripOnSacrifice { get; set; }

        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
        {
            if (EnableCorpseStripOnSacrifice)
            {
                var corpse = ingredient as Corpse;
                corpse.Strip();
            }
            Utils.PostSacrifide(map, true);
            base.ConsumeIngredient(ingredient, recipe, map);
        }
    }
}
