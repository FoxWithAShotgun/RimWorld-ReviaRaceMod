using ReviaRace.Comps;
using ReviaRace.HarmonyPatches;
using ReviaRace.Helpers;
using ReviaRace.Needs;
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
            EnableCorpseStripOnSacrifice = true;
            EnableBloodthirstNeed = true;
            BloodthirstDaysToEmpty = 7.0f;
            SoulReapSpawnRange = new IntRange(1, 3);
            SoulReapSpawnFixed = 2;
            SoulReapSpawnByAge = true;
        }

        public void ApplySettings()
        {
            InvokeBlessing.CostBase = CostBase;
            InvokeBlessing.CostGrowthFactor = CostGrowthFactor;
            InvokeBlessing.CostGrowthMode = CostGrowthMode;
            InvokeBlessing.CostGrowthStartTier = CostGrowthStartTier;
            SoulReaperWorker.EnableRandomSoulReapTier = EnableRandomSoulReapTier;
            SoulReaperWorker.SoulReapSpawnRange = SoulReapSpawnRange;
            SoulReaperWorker.SoulReapSpawnByAge = SoulReapSpawnByAge;
            SoulReaperWorker.SoulReapSpawnFixed = SoulReapSpawnFixed;
            SacrificeWorker.EnableCorpseStripOnSacrifice = EnableCorpseStripOnSacrifice;
            BloodthirstNeed.Enabled = EnableBloodthirstNeed;
            BloodthirstNeed.DaysToEmpty = BloodthirstDaysToEmpty;
            Genes.ReviaBaseGene.DisableUncompleteDebuff_Claws = DisableUncompleteDebuff_Claws;
            Genes.ReviaBaseGene.DisableUncompleteDebuff_Ears = DisableUncompleteDebuff_Ears;
            Genes.ReviaBaseGene.DisableUncompleteDebuff_Teeth = DisableUncompleteDebuff_Teeth;
            Entry.NoCraftLimitation = NoCraftLimitations;
            Entry.NoProjectLimitations = NoProjectLimitations;
            Log.Message("Settings applied");
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

        public bool EnableBloodthirstNeed
        {
            get => _enableBloodthirstNeed;
            set => _enableBloodthirstNeed = value;
        }
        internal bool _enableBloodthirstNeed;

        public IntRange SoulReapSpawnRange
        {
            get => _soulReapSpawnRange;
            set => _soulReapSpawnRange = value;
        }
        internal IntRange _soulReapSpawnRange;

        public bool SoulReapSpawnByAge
        {
            get => _soulReapSpawnByAge;
            set => _soulReapSpawnByAge = value;
        }
        internal bool _soulReapSpawnByAge;

        public int SoulReapSpawnFixed
        {
            get => _soulReapSpawnFixed;
            set => _soulReapSpawnFixed = 2;
        }
        internal int _soulReapSpawnFixed;

        public float BloodthirstDaysToEmpty
        {
            get => _bloodthirstDaysToEmpty;
            set => _bloodthirstDaysToEmpty = 7.0f;
        }
        internal float _bloodthirstDaysToEmpty;

        internal bool _DisableUncompleteDebuff_Ears, _DisableUncompleteDebuff_Claws, _DisableUncompleteDebuff_Teeth, _NoProjectLimitations,_NoCraftLimitations;
        public bool DisableUncompleteDebuff_Ears { get=> _DisableUncompleteDebuff_Ears; set=>_DisableUncompleteDebuff_Ears=value; }
        public bool DisableUncompleteDebuff_Claws { get=>_DisableUncompleteDebuff_Claws; set=>_DisableUncompleteDebuff_Claws=value; }
        public bool DisableUncompleteDebuff_Teeth { get=>_DisableUncompleteDebuff_Teeth; set=>_DisableUncompleteDebuff_Teeth=value; }
        public bool NoProjectLimitations { get => _NoProjectLimitations; set => _NoProjectLimitations = value; }
        public bool NoCraftLimitations { get => _NoCraftLimitations; set => _NoCraftLimitations = value; }
        public override void ExposeData()
        {
            Scribe_Values.Look(ref _costBase, GetLabel(nameof(CostBase)), 1);
            Scribe_Values.Look(ref _costGrowthFactor, GetLabel(nameof(CostGrowthFactor)), 2);
            Scribe_Values.Look(ref _costGrowthStartTier, GetLabel(nameof(CostGrowthStartTier)), 1);
            Scribe_Values.Look(ref _costGrowthMode, GetLabel(nameof(CostGrowthMode)), SacrificeCostGrowth.Exponential);
            Scribe_Values.Look(ref _enableRandomSoulReapTier, GetLabel(nameof(EnableRandomSoulReapTier)), false);
            Scribe_Values.Look(ref _enableCorpseStripOnSacrifice, GetLabel(nameof(EnableCorpseStripOnSacrifice)), true);
            Scribe_Values.Look(ref _enableBloodthirstNeed, GetLabel(nameof(EnableBloodthirstNeed)), true);
            Scribe_Values.Look(ref _bloodthirstDaysToEmpty, GetLabel(nameof(BloodthirstDaysToEmpty)), 7.0f);
            Scribe_Values.Look(ref _soulReapSpawnRange, GetLabel(nameof(SoulReapSpawnRange)), new IntRange(1,3));
            Scribe_Values.Look(ref _soulReapSpawnByAge, GetLabel(nameof(SoulReapSpawnByAge)), true);
            Scribe_Values.Look(ref _soulReapSpawnFixed, GetLabel(nameof(SoulReapSpawnFixed)), 2);
            Scribe_Values.Look(ref _DisableUncompleteDebuff_Ears, GetLabel(nameof(DisableUncompleteDebuff_Ears)),false);
            Scribe_Values.Look(ref _DisableUncompleteDebuff_Claws, GetLabel(nameof(DisableUncompleteDebuff_Claws)), false);
            Scribe_Values.Look(ref _DisableUncompleteDebuff_Teeth, GetLabel(nameof(DisableUncompleteDebuff_Teeth)), false);
            Scribe_Values.Look(ref _NoProjectLimitations, GetLabel(nameof(NoProjectLimitations)), false);
            Scribe_Values.Look(ref _NoCraftLimitations, GetLabel(nameof(NoCraftLimitations)), false);
            ApplySettings();
        }

        private string GetLabel(string nameOfVar) => $"ReviaRace_{nameOfVar}";
    }
}
