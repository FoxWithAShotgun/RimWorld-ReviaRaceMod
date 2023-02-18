﻿using ReviaRace.Comps;
using ReviaRace.Enums;
using ReviaRace.Helpers;
using ReviaRace.PersistentData;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static Verse.Widgets;

namespace ReviaRace
{
    public class ReviaRaceMod : Verse.Mod
    {
        public override void WriteSettings()
        {
            base.WriteSettings();
            Settings.ApplySettings();
        }
        public ReviaRaceMod(ModContentPack content) : base(content)
        {
            Settings = base.GetSettings<ReviaSettings>();
            Settings.ApplySettings();

            LongEventHandler.ExecuteWhenFinished(AddLifeLeechComp);
        }

        public ReviaSettings Settings { get; set; }
        private string _baseCostBuf;
        private string _growthFactorBuf;
        private string _growthStartTierBuf;
        private string _fixedTierBuf;
        private string _bloodthirstDaysToEmptyBuf;

        public override string SettingsCategory()
        {
            return Translator.Translate(Strings.ReviaRaceModName);
        }

        private static void AddLifeLeechComp()
        {
            var meleeWeaponDefs = DefDatabase<ThingDef>.AllDefs
                .Where(def => def.IsMeleeWeapon );

            foreach (var thing in meleeWeaponDefs)
            {
                if (!thing.HasComp(typeof(CompLifeLeech)))
                {
#if DEBUG
                    Log.Message($"{thing} has no LifeLeech. Adding...");
#endif
                    var llStrength = GetDefaultLifeLeech(thing.defName);
                    if (llStrength == 0)
                    {
                        thing.comps.Add(new CompProperties_LifeLeech() 
                        {
                            LeechStrength = llStrength 
                        });
                    }
#if DEBUG
                    Log.Message($"Adding LifeLeech to {thing} with strength {llStrength}");
#endif
                }
                else
                {
                    var llcp = thing.comps.First(comp => comp is CompProperties_LifeLeech);
#if DEBUG
                    Log.Message($"LifeLeech already exists on {thing} with strength {(llcp as CompProperties_LifeLeech).LeechStrength}");
#endif
                }
            }
        }

        internal static float GetDefaultLifeLeech(string thingDefName)
        {
            switch (thingDefName)
            {
                case "ReviaBlessedSerratedDagger":
                    return 0.75f;
                case "ReviaBlessedSerratedScythe":
                    return 2.50f;
                case "ReviaBlessedSerratedSword":
                    return 1.50f;
                default: return 0.0f;
            }
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            const int numRows = 14;

            var headerState = new GUIStyleState
            {
                textColor = new Color(1.0f, 0.9f, 0.25f, 1.0f),
            };
            var headerStyle = new GUIStyle(Text.CurFontStyle)
            {
                fontStyle = FontStyle.Bold,
                normal = headerState,
            };
            var bodyStyle = new GUIStyle(Text.CurFontStyle)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(4, 4, 4, 4),
            };

            var sacrificeList = new Listing_Standard();
            var height
                = 3 * 10
                + (bodyStyle.lineHeight + 8) * numRows
                + (headerStyle.lineHeight);
                
            var sacrificeGroupRect = new Rect(inRect.x + 10, inRect.y + 10, inRect.width - 20, height);
            GUI.Box(sacrificeGroupRect, Translator.Translate(Strings.SettingsSacrificeCostHeader), headerStyle);

            var sacrificeListRect = new Rect(
                sacrificeGroupRect.x + 10,
                sacrificeGroupRect.y + 10 + headerStyle.lineHeight,
                sacrificeGroupRect.width - 20,
                sacrificeGroupRect.height - 20);
            sacrificeList.Begin(sacrificeListRect);
            sacrificeList.Gap(10);

            var lineHeight = bodyStyle.lineHeight + 8;
            DrawRejectionDropDown(sacrificeList.GetRect(lineHeight));
            DrawSacrificeDropDown(sacrificeList.GetRect(lineHeight));
            DrawBornOptionDropDown(sacrificeList.GetRect(lineHeight));
            DrawTextFieldWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSacrificeCostBase, ref Settings._costBase, ref _baseCostBuf, 1, 10);
            DrawTextFieldWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSacrificeCostGrowthFactor, ref Settings._costGrowthFactor, ref _growthFactorBuf, 0, 10);
            DrawTextFieldWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSacrificeCostGrowthStartTier, ref Settings._costGrowthStartTier, ref _growthStartTierBuf, 1, 8);
            DrawCheckBoxWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSacrificeEnableRandomSoulReapTier, ref Settings._enableRandomSoulReapTier);
            DrawCheckBoxWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSacrificeEnableStripOnSacrifice, ref Settings._enableCorpseStripOnSacrifice);
            DrawCheckBoxWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsEnableBloodthirstNeed, ref Settings._enableBloodthirstNeed);
            DrawCheckBoxWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsDisableUncompleteDebuff_Ears, ref Settings._DisableUncompleteDebuff_Ears);
            DrawCheckBoxWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsDisableUncompleteDebuff_Teeth, ref Settings._DisableUncompleteDebuff_Teeth);
            DrawCheckBoxWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsDisableUncompleteDebuff_Claws, ref Settings._DisableUncompleteDebuff_Claws);
            DrawCheckBoxWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsReviaNoProjectLimitaions, ref Settings._NoProjectLimitations);
            DrawCheckBoxWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsReviaNoCraftLimitaions, ref Settings._NoCraftLimitations);
            if (Settings.EnableBloodthirstNeed)
            {
                DrawTextFieldWithLabel<float>(sacrificeList.GetRect(lineHeight), Strings.SettingsBloodthirstDaysToEmpty, ref Settings._bloodthirstDaysToEmpty, ref _bloodthirstDaysToEmptyBuf, 1, 60);
            }

            if (Settings.EnableRandomSoulReapTier)
            {
                DrawCheckBoxWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSoulReapSpawnAgeCurve, ref Settings._soulReapSpawnByAge);
                if (!Settings.SoulReapSpawnByAge)
                {
                    DrawRangeWidgetWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSoulReapSpawnRange, ref Settings._soulReapSpawnRange, 1, 9);
                }
            }
            else
            {
                DrawTextFieldWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSoulReapSpawnFixed, ref Settings._soulReapSpawnFixed, ref _fixedTierBuf, 1, 9);
            }

            sacrificeList.Gap(10);
            DrawCostCalculationLabel(sacrificeList.GetRect(lineHeight), typeof(InvokeGreaterBlessing));
            DrawCostCalculationLabel(sacrificeList.GetRect(lineHeight), typeof(InvokeLesserBlessing));

            sacrificeList.End();
        }
        private void DrawSacrificeDropDown(Rect elemRect)
        {
            var rectLabel = elemRect.LeftPart(0.33f);
            var rectComboBox = elemRect.RightPart(0.66f);

            Widgets.Label(rectLabel, Translator.Translate(Strings.SettingsSacrificeCostGrowthMode));
            Widgets.Dropdown<SacrificeCostGrowth, SacrificeCostGrowth>(
                rectComboBox,
                SacrificeCostGrowth.Exponential,
                mode => mode,
                (s) =>
                {
                    var optionsList = new List<DropdownMenuElement<SacrificeCostGrowth>>();

                    foreach (var mode in Enum.GetValues(typeof(SacrificeCostGrowth)).OfType<SacrificeCostGrowth>())
                    {
                        optionsList.Add(new DropdownMenuElement<SacrificeCostGrowth>
                        {
                            option = new FloatMenuOption(
                                Translator.Translate(mode.ToString()),
                                () => Settings.CostGrowthMode = mode
                                ),
                            payload = (SacrificeCostGrowth)mode
                        });
                    }

                    return optionsList;
                },
                Translator.Translate(Settings.CostGrowthMode.ToString()));
        }
        private void DrawRejectionDropDown(Rect elemRect)
        {
            var rectLabel = elemRect.LeftPart(0.33f);
            var rectComboBox = elemRect.RightPart(0.66f);

            Widgets.Label(rectLabel, Translator.Translate(Strings.SettingsRejectionType));
            Widgets.Dropdown<RejectionType, RejectionType>(
                rectComboBox,
                RejectionType.Disease,
                mode => mode,
                (s) =>
                {
                    var optionsList = new List<DropdownMenuElement<RejectionType>>();

                    foreach (var mode in Enum.GetValues(typeof(RejectionType)).OfType<RejectionType>())
                    {
                        optionsList.Add(new DropdownMenuElement<RejectionType>
                        {
                            option = new FloatMenuOption(
                                Translator.Translate(mode.ToString()),
                                () => Settings.RejectionType = mode
                                ),
                            payload = (RejectionType)mode
                        });
                    }

                    return optionsList;
                },
                Translator.Translate(Settings.RejectionType.ToString()));
        }

        private void DrawBornOptionDropDown(Rect elemRect)
        {
            var rectLabel = elemRect.LeftPart(0.33f);
            var rectComboBox = elemRect.RightPart(0.66f);

            Widgets.Label(rectLabel, Translator.Translate(Strings.SettingsBornType));
            Widgets.Dropdown<BornSettingsEnum, BornSettingsEnum>(
                rectComboBox,
                BornSettingsEnum.NoMaleBorn,
                mode => mode,
                (s) =>
                {
                    var optionsList = new List<DropdownMenuElement<BornSettingsEnum>>();

                    foreach (var mode in Enum.GetValues(typeof(BornSettingsEnum)).OfType<BornSettingsEnum>())
                    {
                        optionsList.Add(new DropdownMenuElement<BornSettingsEnum>
                        {
                            option = new FloatMenuOption(
                                Translator.Translate(mode.ToString()),
                                () => Settings.BornSettings = mode
                                ),
                            payload = (BornSettingsEnum)mode
                        });
                    }

                    return optionsList;
                },
                Translator.Translate(Settings.BornSettings.ToString()));
        }

        private void DrawCheckBoxWithLabel(Rect elemRect, string taggedLabelID, ref bool setting)
        {
            var leftRect = elemRect.LeftPart(0.20f);
            var rightRect = elemRect.RightPart(0.80f);
            var x = (float)elemRect.x;
            var y = (float)(elemRect.y + 0.5 * (elemRect.height - 24));
            Widgets.Checkbox(x, y, ref setting);
            Widgets.Label(rightRect, Translator.Translate(taggedLabelID));
        }

        private void DrawTextFieldWithLabel<T>(Rect elemRect, string taggedLabelID, ref T setting, ref string buffer, float min = 0, float max = 100000) where T:struct
        {
            var leftRect = elemRect.LeftPart(0.33f);
            var rightRect = elemRect.RightPart(0.66f);
            Widgets.Label(leftRect, Translator.Translate(taggedLabelID));
            Widgets.TextFieldNumeric<T>(rightRect, ref setting, ref buffer, min, max);
        }

        private void DrawCostCalculationLabel(Rect elemRect, Type invokeType)
        {
            var costs = new List<NamedArgument>();
            var advCostFunc = default(Func<SacrificeCostGrowth, int, float, float, int, int>);
            var formatString = default(string);

            if (invokeType == typeof(InvokeGreaterBlessing))
            {
                advCostFunc = InvokeGreaterBlessing.GetAdvanceCost;
                formatString = Strings.SettingsSacrificeCostGreater;
            }
            else if (invokeType == typeof(InvokeLesserBlessing))
            {
                advCostFunc = InvokeLesserBlessing.GetAdvanceCost;
                formatString = Strings.SettingsSacrificeCostLesser;
            }

            for (int i = 0; i < 8; i++)
            {
                var cost = advCostFunc(
                    Settings.CostGrowthMode,
                    i + 1,
                    Settings._costBase,
                    Settings._costGrowthFactor,
                    Settings._costGrowthStartTier);
                costs.Add(new NamedArgument(cost, i.ToString()));
            }

            Widgets.Label(elemRect, TranslatorFormattedStringExtensions.Translate(formatString, costs.ToArray()));
        }

        private void DrawRangeWidgetWithLabel(Rect elemRect, string taggedLabelID, ref IntRange setting, int min, int max)
        {
            var leftRect = elemRect.LeftPart(0.33f);
            var rightRect = elemRect.RightPart(0.66f);
            Widgets.Label(leftRect, Translator.Translate(taggedLabelID));
            Widgets.IntRange(rightRect, 1, ref setting, min, max);
        }
    }
}
