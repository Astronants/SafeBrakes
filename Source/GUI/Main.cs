using System.Text.RegularExpressions;
using UnityEngine;

namespace SafeBrakes
{
    class Main : MonoBehaviour
    {
        public enum SB_Pages
        {
            Preset,
            Settings
        }
        public SB_Pages Page = SB_Pages.Preset;
        public Confirm confirmGUI;

        public static Rect windowRect;
        private Vector2 presetsListScroll, pageScroll;

        public string preset_name, preset_absMin, preset_sabHigh, preset_sabLow;
        public bool preset_sabAllow;
        bool settings_KSPskin;

        private GUIStyle selected_button, orange_button, inactive_button;

        private bool gamepaused;
        public bool window_enabled = false;
        
        public static readonly Vector2 KSPskinSize = new Vector2(384, 254);
        public static readonly Vector2 DefaultskinSize = new Vector2(390, 238);

        public void Awake()
        {
            GameEvents.onGamePause.Add(this.OnGamePause);
            GameEvents.onGameUnpause.Add(this.OnGameUnpause);
            
            this.Update_PresetPage();
        }

        public void OnDestroy()
        {
            GameEvents.onGamePause.Remove(this.OnGamePause);
            GameEvents.onGameUnpause.Remove(this.OnGameUnpause);
        }

        public void OnGUI()
        {
            if (!window_enabled || gamepaused) return;
            if (Settings.Fetch.KSPSkin) GUI.skin = HighLogic.Skin;

            selected_button = new GUIStyle(GUI.skin.button) { normal = { textColor = GUI.skin.button.active.textColor, background = GUI.skin.button.active.background } };
            orange_button = new GUIStyle(GUI.skin.button) { normal = { textColor = new Color(1f, 0.5f, 0f) }, padding = new RectOffset(10, 10, GUI.skin.button.padding.top, GUI.skin.button.padding.bottom), hover = { textColor = new Color(1f, 0.5f, 0f) }, active = { textColor = new Color(1f, 0.5f, 0f) } };
            inactive_button = new GUIStyle(GUI.skin.button) { normal = { textColor = Color.gray, background = GUI.skin.button.active.background } };

            windowRect = GUILayout.Window(this.GetInstanceID(), windowRect, this.Window, "SafeBrakes", GUILayout.Width(1), GUILayout.Height(1));
        }

        private void Window(int id)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(370));
            #region 'X' Button
            if (GUI.Button(new Rect(windowRect.width - 23f, 3f, 20f, 20f), " X"))
            {
                AppLauncherButton.appButton.SetFalse(true);
            }
            #endregion
            GUILayout.BeginVertical(GUILayout.Width(220));
            #region Settings Panel
            pageScroll = GUILayout.BeginScrollView(pageScroll, GUILayout.Height(150));
            switch (this.Page)
            {
                case SB_Pages.Preset:
                    this.PresetPage();
                    break;
                case SB_Pages.Settings:
                    this.SettingsPage();
                    break;
            }
            GUILayout.EndScrollView();
            #endregion
            #region Bottom-Left Panel
            GUILayout.BeginHorizontal();
            #region 'Save' Button
            if (this.Page == SB_Pages.Preset && PresetsHandler.current.FileName == PresetsHandler.defaultPreset)
            {
                GUILayout.Label("Save", inactive_button);
            }
            else
            {
                if (GUILayout.Button("Save"))
                {
                    switch (this.Page)
                    {
                        case SB_Pages.Preset:
                            SavePreset();
                            break;
                        case SB_Pages.Settings:
                            SaveSettings();
                            break;
                    }
                }
            }
            #endregion
            #region 'Delete' Button
            if (this.Page == SB_Pages.Preset && PresetsHandler.allConfigs.Count > 1 && PresetsHandler.current.FileName != PresetsHandler.defaultPreset)
            {
                if (GUILayout.Button("Delete", orange_button, GUILayout.ExpandWidth(false)))
                {
                    confirmGUI = this.gameObject.AddComponent<Confirm>();
                    Update_PresetPage();
                }
            }
            #endregion
            GUILayout.EndHorizontal();
            #region 'Settings' Button
            if (GUILayout.Button("Settings"))
            {
                switch (this.Page)
                {
                    case SB_Pages.Preset:
                        SetPage(SB_Pages.Settings);
                        break;
                    case SB_Pages.Settings:
                        SetPage(SB_Pages.Preset);
                        break;
                }
            }
            #endregion
            #endregion
            GUILayout.EndVertical();
            #region Right Panel
            GUILayout.BeginVertical();
            #region 'Add' Button
            if (GUILayout.Button("Add config"))
            {
                OnAddButtonClicked();
            }
            #endregion
            #region Configs list
            presetsListScroll = GUILayout.BeginScrollView(presetsListScroll);
            foreach (var config in PresetsHandler.allConfigs)
            {
                if (PresetsHandler.current == config)
                {
                    GUILayout.Label(config.name, selected_button);
                }
                else
                {
                    if (GUILayout.Button(config.name))
                    {
                        PresetsHandler.current = config;
                        Settings.Fetch.Save();
                        SetPage(SB_Pages.Preset);
                    }
                }
            }
            GUILayout.EndScrollView();
            #endregion
            GUILayout.EndVertical();
            #endregion
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        private void PresetPage()
        {
            if (PresetsHandler.current.FileName != PresetsHandler.defaultPreset)
            {
                preset_name = GUI_Name_Input(preset_name);
                preset_absMin = GUI_Float_Input(preset_absMin, "ABS min speed", "m/s");
                preset_sabAllow = GUILayout.Toggle(preset_sabAllow, "Enable SafeAirBrakes");
                preset_sabHigh = GUI_Float_Input(preset_sabHigh, "SAB high threshold", "%");
                preset_sabLow = GUI_Float_Input(preset_sabLow, "SAB low threshold", "%");
            }
            else
            {
                GUI_Name_Input(preset_name);
                GUI_Float_Input(preset_absMin, "ABS min speed", "m/s");
                GUILayout.Toggle(preset_sabAllow, "Enable SafeAirBrakes");
                GUI_Float_Input(preset_sabHigh, "SAB high threshold", "%");
                GUI_Float_Input(preset_sabLow, "SAB low threshold", "%");
            }
        }

        public void Update_PresetPage()
        {
            Preset current = PresetsHandler.current;
            this.preset_name = current.name;
            this.preset_absMin = current.abs_minSpd.ToString();
            this.preset_sabAllow = current.allow_sab;
            this.preset_sabHigh = current.sab_highT.ToString();
            this.preset_sabLow = current.sab_lowT.ToString();
        }

        private void SettingsPage()
        {
            settings_KSPskin = GUILayout.Toggle(settings_KSPskin, "use KSP Skin");
            if (GUILayout.Button("Reload Presets"))
            {
                PresetsHandler.Load_Presets();
                Update_PresetPage();
            }
        }

        public void Update_SettingsPage()
        {
            this.settings_KSPskin = Settings.Fetch.KSPSkin;
        }

        private string GUI_Name_Input(string var)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:");
            string value = GUILayout.TextField(var, GUILayout.Width(140f));
            GUILayout.EndHorizontal();
            return value;
        }
        private string GUI_Float_Input(string var, string name, string unit)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":");
            string value = GUILayout.TextField(var, GUILayout.Width(42f));
            GUILayout.Label(unit);
            GUILayout.EndHorizontal();
            return value;
        }

        private void SetPage(SB_Pages newPage)
        {
            switch (newPage)
            {
                case SB_Pages.Preset:
                    Update_PresetPage();
                    break;
                case SB_Pages.Settings:
                    Update_SettingsPage();
                    break;
            }
            this.Page = newPage;
        }
        
        public void OnGamePause()
        {
            gamepaused = true;
        }

        public void OnGameUnpause()
        {
            gamepaused = false;
        }

        private void OnAddButtonClicked()
        {
            string newName = "New config";
            Preset newcfg = new Preset(newName);
            newcfg.Save(PresetsHandler.presets_dir);
            PresetsHandler.allConfigs.Add(newcfg);
            PresetsHandler.current = PresetsHandler.allConfigs[PresetsHandler.allConfigs.Count - 1];
            Settings.Fetch.Save();
            SetPage(SB_Pages.Preset);
        }

        private void SavePreset()
        {
            Regex floatRegex = new Regex("[0-9]+$");
            if (string.IsNullOrWhiteSpace(preset_name) || preset_name.Contains("/") || preset_name.Contains("\\"))
            {
                ScreenMessages.PostScreenMessage($"[{Logger.modName}]: Incorrect preset name. Please enter a correct name.");
            }
            else if (!floatRegex.IsMatch(preset_absMin) || !floatRegex.IsMatch(preset_sabHigh) || !floatRegex.IsMatch(preset_sabLow))
            {
                ScreenMessages.PostScreenMessage($"[{Logger.modName}]: One or multiple values are incorrect. Please enter a correct value.");
            }
            else
            {
                PresetsHandler.current.name = preset_name;
                try { PresetsHandler.current.abs_minSpd = float.Parse(preset_absMin); } catch { }
                PresetsHandler.current.allow_sab = preset_sabAllow;
                try { PresetsHandler.current.sab_highT = float.Parse(preset_sabHigh); } catch { }
                try { PresetsHandler.current.sab_lowT = float.Parse(preset_sabLow); } catch { }

                if (PresetsHandler.current.Save(PresetsHandler.presets_dir))
                {
                    ScreenMessages.PostScreenMessage($"[{Logger.modName}]: Config saved.", 5, ScreenMessageStyle.UPPER_CENTER);
                }
                else
                {
                    ScreenMessages.PostScreenMessage($"[{Logger.modName}]: An error has occured while saving the preset.", 5, ScreenMessageStyle.UPPER_CENTER, Color.yellow);
                }
            }
            Update_PresetPage();
        }

        private void SaveSettings()
        {
            Settings.Fetch.KSPSkin = settings_KSPskin;

            if (Settings.Fetch.Save())
            {
                ScreenMessages.PostScreenMessage($"[{Logger.modName}]: Settings saved.", 5, ScreenMessageStyle.UPPER_CENTER);
            }
            else
            {
                ScreenMessages.PostScreenMessage($"[{Logger.modName}]: An error has occured while saving the settings.", 5, ScreenMessageStyle.UPPER_CENTER, Color.yellow);
            }
        }
    }
}
