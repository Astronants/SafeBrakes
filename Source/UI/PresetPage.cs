using KSPAchievements;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SafeBrakes.UI
{
    internal class PresetPage : IPage
    {
        private string Name;
        private string AbsMin;
        private bool   SabAllow;
        private string SabHigh;
        private string SabLow;
        private static Vector2 scroll;

        private string NameInput(string var)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:");
            string value = GUILayout.TextField(var, GUILayout.Width(140f));
            GUILayout.EndHorizontal();
            return value;
        }

        private string FloatInput(string var, string name, string unit)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":");
            string value = GUILayout.TextField(var, GUILayout.Width(42f));
            GUILayout.Label(unit);
            GUILayout.EndHorizontal();
            return value;
        }

        public void Show()
        {

            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(150));
            {
                Name = NameInput(Name);
                AbsMin = FloatInput(AbsMin, "ABS min speed", "m/s");
                SabAllow = GUILayout.Toggle(SabAllow, "Enable SafeAirBrakes");
                SabHigh = FloatInput(SabHigh, "SAB high threshold", "%");
                SabLow = FloatInput(SabLow, "SAB low threshold", "%");
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save")) Save();
                if ((PresetsHandler.Instance.current.FileName != PresetsHandler.defaultPreset) && PresetsHandler.Instance.allConfigs.Count > 1 && GUILayout.Button("Delete", Styles.orange_button, GUILayout.ExpandWidth(false)))
                {
                    PopupDialog.SpawnPopupDialog(
                        new MultiOptionDialog("SafeBrakesConfirmDeletion",
                            $"Are you sure you want to delete {PresetsHandler.Instance.current.name}?",
                            "",
                            HighLogic.UISkin,
                            new DialogGUIButton("Cancel", () => { return; }),
                            new DialogGUIButton("Yes", PresetsHandler.Instance.DeletePreset)),
                        false,
                        HighLogic.UISkin);
                    Update();
                }
            }
            GUILayout.EndHorizontal();
        }

        public void Update()
        {
            Preset current = PresetsHandler.Instance.current;
            this.Name = current.name;
            this.AbsMin = current.abs_minSpd.ToString();
            this.SabAllow = current.allow_sab;
            this.SabHigh = current.sab_highT.ToString();
            this.SabLow = current.sab_lowT.ToString();
        }

        public void Save()
        {
            // Validate inputs
            Regex floatRegex = new Regex("[0-9]+$");
            if (string.IsNullOrWhiteSpace(Name) || Name.Contains("/") || Name.Contains("\\"))
            {
                ScreenMessages.PostScreenMessage($"[{Logger.modName}]: Incorrect preset name. Please enter a correct name.");
                return;
            }
            if (!floatRegex.IsMatch(AbsMin) || !floatRegex.IsMatch(SabHigh) || !floatRegex.IsMatch(SabLow))
            {
                ScreenMessages.PostScreenMessage($"[{Logger.modName}]: One or multiple values are incorrect. Please enter a correct value.");
                return;
            }
            // Apply changes to chosen preset
            PresetsHandler.Instance.current.name = Name;
            try { PresetsHandler.Instance.current.abs_minSpd = float.Parse(AbsMin); } catch { }
            PresetsHandler.Instance.current.allow_sab = SabAllow;
            try { PresetsHandler.Instance.current.sab_highT = float.Parse(SabHigh); } catch { }
            try { PresetsHandler.Instance.current.sab_lowT = float.Parse(SabLow); } catch { }
            // Save cfg
            if (PresetsHandler.Instance.SavePreset())
                ScreenMessages.PostScreenMessage($"[{Logger.modName}]: Config saved.", 5, ScreenMessageStyle.UPPER_CENTER);
            else
                ScreenMessages.PostScreenMessage($"[{Logger.modName}]: An error has occured while saving the preset.", 5, ScreenMessageStyle.UPPER_CENTER, Color.yellow);

            Update();
        }
    }
}
