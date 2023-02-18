using ReviaRace.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviaRace.Helpers
{
    internal static class StaticModVariables
    {
        public static BornSettingsEnum BornSettings { get; internal set; }
        public static int BloodstonesCountForConversation { get; internal set; } = 10;
    }
}
