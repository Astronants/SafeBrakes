using System;
using UnityEngine;

namespace SafeBrakes
{
    class PresetsGUI : MonoBehaviour
    {
        private static bool isRunning;
        private bool isCentered;
        public static Rect window_pos;
        private Vector2 scroll_pos;
        public static ConfirmGUI confirmGUI;
        public static SettingsGUI settingsGUI;

        public string show_name, show_absMin, show_sabHigh, show_sabLow;
        public bool show_sabAllow;

        public void Awake()
        {
            if (isRunning)
            {
                Destroy(this);
            }
            isRunning = true;
        }

        public void CloseGUI()
        {
            isRunning = false;
            Destroy(this);
        }

        public void OnDestroy()
        {
            if (confirmGUI != null) { confirmGUI.CloseGUI(); }
            if (settingsGUI != null) { settingsGUI.CloseGUI(); }
            Configs.presetsGUI = null;
        }

        public void OnGUI()
        {
            if (Configs.KSPSkin) { GUI.skin = HighLogic.Skin; }
            try
            {
                window_pos = GUILayout.Window(this.GetInstanceID(), window_pos, this.Window, "SafeBrakes", GUILayout.Width(1f), GUILayout.Height(1f));

                if (!isCentered && window_pos.width > 0f && window_pos.height > 0f)
                {
                    window_pos.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                    isCentered = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error("an error has occured while opening the PresetsGUI.", e);
                CloseGUI();
            }
        }

        private void Window(int id)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(370f));
            #region Left Panel
            GUILayout.BeginVertical(GUILayout.Width(220f));
            #region Show Config
            GUILayout.BeginVertical(GUI.skin.box);
            show_name = GUI_Name_Input(show_name);
            show_absMin = GUI_Float_Input("ABS min speed", show_absMin, "m/s");
            show_sabAllow = GUILayout.Toggle(show_sabAllow, "Use SafeAirBrakes");
            show_sabHigh = GUI_Float_Input("SAB high trigger", show_sabHigh, "%");
            show_sabLow = GUI_Float_Input("SAB low trigger", show_sabLow, "%");
            GUILayout.EndVertical();
            #endregion
            GUILayout.BeginHorizontal();
            #region 'Save' Button
            if (GUILayout.Button("Save"))
            {
                if (Configs.current.fileName == Configs.defaultPreset)
                {
                    ScreenMessages.PostScreenMessage("[SafeBrakes]: You cannot change default settings", 5, ScreenMessageStyle.UPPER_CENTER);
                }
                else
                {
                    Configs.current.name = show_name;
                    Configs.current.abs_minSpd = float.Parse(show_absMin);
                    Configs.current.sab_allow = show_sabAllow;
                    Configs.current.sab_highT = float.Parse(show_sabHigh);
                    Configs.current.sab_lowT = float.Parse(show_sabLow);
                    if (Configs.current.Save(Configs.presets_dir))
                    {
                        Logger.Log("Config saved.");
                        ScreenMessages.PostScreenMessage("[SafeBrakes]: Config saved.", 5, ScreenMessageStyle.UPPER_CENTER);
                    }

                }
                Configs.Update_PresetsGUI();
            }
            #endregion
            #region 'Delete' button
            if (Configs.allConfigs.Count > 1 && Configs.current.fileName != Configs.defaultPreset)
            {
                if (GUILayout.Button("Delete", new GUIStyle(GUI.skin.button) { padding = new RectOffset(10, 10, GUI.skin.button.padding.top, GUI.skin.button.padding.bottom) }, GUILayout.ExpandWidth(false)))
                {
                    confirmGUI = this.gameObject.AddComponent<ConfirmGUI>();
                    Configs.Update_PresetsGUI();
                }
            }
            #endregion
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            #region 'Close' button
            if (GUILayout.Button("Close", new GUIStyle(GUI.skin.button) { padding = new RectOffset(10, 10, GUI.skin.button.padding.top, GUI.skin.button.padding.bottom) }, GUILayout.ExpandWidth(false)))
            {
                CloseGUI();
            }
            #endregion
            #region 'Settings' button
            if (GUILayout.Button("Settings"))
            {
                if (settingsGUI == null)
                {
                    settingsGUI = this.gameObject.AddComponent<SettingsGUI>();
                }
                else
                {
                    settingsGUI.CloseGUI();
                }
            }
            #endregion
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion
            #region Right Panel
            GUILayout.BeginVertical();
            #region 'Add' Button
            if (GUILayout.Button("Add config"))
            {
                string newName = "New config";
                Preset newcfg = new Preset(newName);
                newcfg.Save(Configs.presets_dir);
                Configs.allConfigs.Add(newcfg);
                Configs.Update_Plugin(Configs.allConfigs[Configs.allConfigs.Count - 1]);
                Configs.Update_PresetsGUI();
            }
            #endregion
            #region Configs list
            scroll_pos = GUILayout.BeginScrollView(scroll_pos);
            foreach (var config in Configs.allConfigs)
            {
                if (GUILayout.Button(config.name))
                {
                    Configs.Update_Plugin(config);
                    Configs.Update_PresetsGUI();
                }
            }
            GUILayout.EndScrollView();
            #endregion
            GUILayout.EndVertical();
            #endregion
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        private string GUI_Name_Input(string _var)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:");
            string value = GUILayout.TextField(_var, GUILayout.Width(160f));
            GUILayout.EndHorizontal();
            return value;
        }
        private string GUI_Float_Input(string _name, string _var, string _unit)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_name + ":");
            string value = GUILayout.TextField(_var, GUILayout.Width(45f));
            GUILayout.Label(_unit);
            GUILayout.EndHorizontal();
            return value;
        }
    }
}
