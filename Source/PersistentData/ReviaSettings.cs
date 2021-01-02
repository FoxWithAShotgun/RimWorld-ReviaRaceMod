using ReviaRace.Comps;
using ReviaRace.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ReviaRace.PersistentData
{
    public class ReviaSettings : Verse.ModSettings
    {
        public ReviaSettings()
        {
            CostBase = 1;
            CostGrowthFactor = 2;
            CostGrowthStartTier = 1;
            CostGrowthMode = SacrificeCostGrowth.Exponential;
            EnableRandomSoulReapTier = true;
        }

        public void ApplySettings()
        {
            InvokeBlessing.CostBase = CostBase;
            InvokeBlessing.CostGrowthFactor = CostGrowthFactor;
            InvokeBlessing.CostGrowthMode = CostGrowthMode;
            InvokeBlessing.CostGrowthStartTier = CostGrowthStartTier;
            SoulReaper.EnableRandomSoulReapTier = EnableRandomSoulReapTier;
            SacrificeWorker.EnableCorpseStripOnSacrifice = EnableCorpseStripOnSacrifice;
        }

        public float CostBase
        {
            get => _costBase;
            set => _costBase = value;
        }
        internal float _costBase;
        public float CostGrowthFactor 
        {
            get => _costGrowthFactor;
            set => _costGrowthFactor = value;
        }
        internal float _costGrowthFactor;

        public int CostGrowthStartTier
        {
            get => _costGrowthStartTier;
            set => _costGrowthStartTier = value;
        }
        internal int _costGrowthStartTier;
        public SacrificeCostGrowth CostGrowthMode 
        {
            get => _costGrowthMode;
            set => _costGrowthMode = value;
        }
        internal SacrificeCostGrowth _costGrowthMode;

        public bool EnableRandomSoulReapTier
        {
            get => _enableRandomSoulReapTier;
            set => _enableRandomSoulReapTier = value;
        }
        internal bool _enableRandomSoulReapTier;

        public bool EnableCorpseStripOnSacrifice
        {
            get => _enableCorpseStripOnSacrifice;
            set => _enableCorpseStripOnSacrifice = value;
        }
        internal bool _enableCorpseStripOnSacrifice;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref _costBase, GetLabel(nameof(CostBase)), 1);
            Scribe_Values.Look(ref _costGrowthFactor, GetLabel(nameof(CostGrowthFactor)), 2);
            Scribe_Values.Look(ref _costGrowthStartTier, GetLabel(nameof(CostGrowthStartTier)), 1);
            Scribe_Values.Look(ref _costGrowthMode, GetLabel(nameof(CostGrowthMode)), SacrificeCostGrowth.Exponential);
            Scribe_Values.Look(ref _enableRandomSoulReapTier, GetLabel(nameof(EnableRandomSoulReapTier)), false);
            Scribe_Values.Look(ref _enableCorpseStripOnSacrifice, GetLabel(nameof(EnableCorpseStripOnSacrifice)), true);

            ApplySettings();
        }

        private string GetLabel(string nameOfVar) => $"ReviaRace_{nameOfVar}";
    }
}
