using LibNoise.Modifiers;
using System.Linq;
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

        private readonly MainWindow Parent;

        internal PresetPage(MainWindow parent)
        {
            this.Parent = parent;
        }

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
                if ((Parent.app.presets.Selected.FileName != "Default.cfg") && Parent.app.presets.Count > 1 && GUILayout.Button("Delete", Styles.orange_button, GUILayout.ExpandWidth(false)))
                {
                    PopupDialog.SpawnPopupDialog(
                        new MultiOptionDialog("SafeBrakesConfirmDeletion",
                            $"Are you sure you want to delete {Parent.app.presets.Selected.Name}?",
                            "",
                            HighLogic.UISkin,
                            new DialogGUIButton("Cancel", () => { return; }),
                            new DialogGUIButton("Yes", () => { Parent.app.presets.Remove(Parent.app.presets.Selected); Parent.app.presets.Selected = Parent.app.presets.FirstOrDefault(); })),
                        false,
                        HighLogic.UISkin);
                    Update();
                }
            }
            GUILayout.EndHorizontal();
        }

        public void Update()
        {
            this.Name = Parent.app.presets.Selected.Name;
            this.AbsMin = Parent.app.presets.Selected.abs_minSpd.ToString();
            this.SabAllow = Parent.app.presets.Selected.allow_sab;
            this.SabHigh = Parent.app.presets.Selected.sab_highT.ToString();
            this.SabLow = Parent.app.presets.Selected.sab_lowT.ToString();
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
            Parent.app.presets.Selected.Name = Name;
            try { Parent.app.presets.Selected.abs_minSpd = float.Parse(AbsMin); } catch { }
            Parent.app.presets.Selected.allow_sab = SabAllow;
            try { Parent.app.presets.Selected.sab_highT = float.Parse(SabHigh); } catch { }
            try { Parent.app.presets.Selected.sab_lowT = float.Parse(SabLow); } catch { }
            // Save cfg
            if (Parent.app.presets.Selected.Save())
                ScreenMessages.PostScreenMessage($"[{Logger.modName}]: Config saved.", 5, ScreenMessageStyle.UPPER_CENTER);
            else
                ScreenMessages.PostScreenMessage($"[{Logger.modName}]: An error has occured while saving the preset.", 5, ScreenMessageStyle.UPPER_CENTER, Color.yellow);

            Update();
        }
    }
}
