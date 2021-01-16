using ReviaRace.Comps;
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
        public ReviaRaceMod(ModContentPack content) : base(content)
        {
            Settings = base.GetSettings<ReviaSettings>();
            Settings.ApplySettings();
        }
        public ReviaSettings Settings { get; set; }
        private string _baseCostBuf;
        private string _growthFactorBuf;
        private string _growthStartTierBuf;

        public override string SettingsCategory()
        {
            return Translator.Translate(Strings.ReviaRaceModName);
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            FactionDef

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
                + (bodyStyle.lineHeight + 8) * 10
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
            DrawSacrificeDropDown(sacrificeList.GetRect(lineHeight));
            DrawTextFieldWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSacrificeCostBase, ref Settings._costBase, ref _baseCostBuf, 1, 10);
            DrawTextFieldWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSacrificeCostGrowthFactor, ref Settings._costGrowthFactor, ref _growthFactorBuf, 0, 10);
            DrawTextFieldWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSacrificeCostGrowthStartTier, ref Settings._costGrowthStartTier, ref _growthStartTierBuf, 1, 8);
            DrawCheckBoxWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSacrificeEnableRandomSoulReapTier, ref Settings._enableRandomSoulReapTier);
            DrawCheckBoxWithLabel(sacrificeList.GetRect(lineHeight), Strings.SettingsSacrificeEnableStripOnSacrifice, ref Settings._enableCorpseStripOnSacrifice);

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

        private void DrawCheckBoxWithLabel(Rect elemRect, string taggedLabelID, ref bool setting)
        {
            var leftRect = elemRect.LeftPart(0.20f);
            var rightRect = elemRect.RightPart(0.80f);
            var x = (float)elemRect.x;
            var y = (float)(elemRect.y + 0.5 * (elemRect.height - 24));
            Widgets.Checkbox(x, y, ref setting);
            Widgets.Label(rightRect, Translator.Translate(taggedLabelID));
        }

        private void DrawTextFieldWithLabel(Rect elemRect, string taggedLabelID, ref float setting, ref string buffer, int min = 0, int max = 100000)
        {
            var leftRect = elemRect.LeftPart(0.33f);
            var rightRect = elemRect.RightPart(0.66f);
            Widgets.Label(leftRect, Translator.Translate(taggedLabelID));
            Widgets.TextFieldNumeric(rightRect, ref setting, ref buffer, min, max);
        }

        private void DrawTextFieldWithLabel(Rect elemRect, string taggedLabelID, ref int setting, ref string buffer, int min = 0, int max = 100000)
        {
            var leftRect = elemRect.LeftPart(0.33f);
            var rightRect = elemRect.RightPart(0.66f);
            Widgets.Label(leftRect, Translator.Translate(taggedLabelID));
            Widgets.TextFieldNumeric(rightRect, ref setting, ref buffer, min, max);
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
    }
}
