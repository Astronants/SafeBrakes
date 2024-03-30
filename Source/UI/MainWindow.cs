using System;
using System.Linq;
using UnityEngine;

namespace SafeBrakes.UI
{
    internal struct Styles
    {
        internal static GUIStyle selected_button, orange_button, inactive_button, inactive_field;
        internal static void LoadStyles()
        {
            selected_button = new GUIStyle(GUI.skin.button) { normal = { textColor = GUI.skin.button.active.textColor, background = GUI.skin.button.active.background } };
            orange_button = new GUIStyle(GUI.skin.button) { normal = { textColor = new Color(1f, 0.5f, 0f) }, padding = new RectOffset(10, 10, GUI.skin.button.padding.top, GUI.skin.button.padding.bottom), hover = { textColor = new Color(1f, 0.5f, 0f) }, active = { textColor = new Color(1f, 0.5f, 0f) } };
            inactive_button = new GUIStyle(GUI.skin.button) { normal = { textColor = Color.gray, background = GUI.skin.button.active.background } };
            inactive_field = new GUIStyle(GUI.skin.textField) { normal = { textColor = Color.gray } };
        }
    }

    class MainWindow : MonoBehaviour
    {
        public App app;
        public IPage Page;

        public static Rect windowRect = new Rect(0, 0, 0, 0);
        private Vector2 presetsListScroll;

        private bool gamepaused;

        public void Awake()
        {
            enabled = false;
            GameEvents.onGamePause.Add(this.OnGamePause);
            GameEvents.onGameUnpause.Add(this.OnGameUnpause);
            float x = Mathf.Clamp(Mouse.screenPos.x - 400, 0, Screen.width - windowRect.width);
            float y = Mathf.Clamp(Mouse.screenPos.y - 50, 0, Screen.height - windowRect.height);
            windowRect.position = new Vector2(x, y);
            app.settings.SetWindowPosition(ref windowRect);
        }

        public void OnDestroy()
        {
            GameEvents.onGamePause.Remove(this.OnGamePause);
            GameEvents.onGameUnpause.Remove(this.OnGameUnpause);
        }

        public void OnGUI()
        {
            if (!enabled || gamepaused) return;
            if (app.settings.useKSPskin) GUI.skin = HighLogic.Skin;

            Styles.LoadStyles();

            windowRect = GUILayout.Window(this.GetInstanceID(), windowRect, this.Window, "SafeBrakes", GUILayout.Width(1), GUILayout.Height(1));
        }

        private void Window(int id)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(370));
            {
                if (GUI.Button(new Rect(windowRect.width - 23f, 3f, 20f, 20f), " X"))
                {
                    app.button.SetFalse(true);
                }
                GUILayout.BeginVertical(GUILayout.Width(220)); // Left panel
                {
                    Page.Show();
                    if (GUILayout.Button("Settings"))
                    {
                        if (Page is PresetPage) SetPage(typeof(SettingsPage));
                        else SetPage(typeof(PresetPage));
                    }
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical(); // Right panel
                {
                    if (GUILayout.Button("Add config"))
                    {
                        string newName = "New config";
                        int n = 2;
                        while (app.presets.Exists(cfg => cfg.Name == newName))
                        {
                            newName = $"New config {n++}";
                        }
                        Preset newcfg = new Preset(newName);
                        newcfg.Save();
                        app.presets.Add(newcfg);
                        app.presets.Selected = app.presets.Last();
                        app.settings.Save();
                        SetPage(typeof(PresetPage));
                    }
                    presetsListScroll = GUILayout.BeginScrollView(presetsListScroll);
                    foreach (var config in app.presets)
                    {
                        if (app.presets.Selected == config)
                        {
                            GUILayout.Label(config.Name, Styles.selected_button);
                        }
                        else
                        {
                            if (GUILayout.Button(config.Name))
                            {
                                app.presets.Selected = config;
                                app.settings.Save();
                                SetPage(typeof(PresetPage));
                            }
                        }
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        public void SetPage(Type t)
        {
            if (t == typeof(PresetPage) && !(Page is PresetPage)) Page = new PresetPage(this);
            else if (t == typeof(SettingsPage) && !(Page is SettingsPage)) Page = new SettingsPage(this);
            Page.Update();
        }

        public void OnGamePause()
        {
            gamepaused = true;
        }

        public void OnGameUnpause()
        {
            gamepaused = false;
        }
    }
}
