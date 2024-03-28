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
        public IPage Page = new PresetPage();

        public static Rect windowRect;
        private Vector2 presetsListScroll;

        private bool gamepaused;

        public void Awake()
        {
            enabled = false;
            GameEvents.onGamePause.Add(this.OnGamePause);
            GameEvents.onGameUnpause.Add(this.OnGameUnpause);
            Page.Update();
            float x = Mathf.Clamp(Mouse.screenPos.x - 400, 0, Screen.width - UI.MainWindow.windowRect.width);
            float y = Mathf.Clamp(Mouse.screenPos.y - 50, 0, Screen.height - UI.MainWindow.windowRect.height);
            windowRect = new Rect(x, y, 0, 0);
            Settings.Instance.SetWindowPosition(ref windowRect);
        }

        public void OnDestroy()
        {
            GameEvents.onGamePause.Remove(this.OnGamePause);
            GameEvents.onGameUnpause.Remove(this.OnGameUnpause);
        }

        public void OnGUI()
        {
            if (!enabled || gamepaused) return;
            if (Settings.Instance.useKSPskin) GUI.skin = HighLogic.Skin;

            Styles.LoadStyles();

            windowRect = GUILayout.Window(this.GetInstanceID(), windowRect, this.Window, "SafeBrakes", GUILayout.Width(1), GUILayout.Height(1));
        }

        private void Window(int id)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(370));
            {
                if (GUI.Button(new Rect(windowRect.width - 23f, 3f, 20f, 20f), " X"))
                {
                    AppLauncherButton.Instance.appButton.SetFalse(true);
                }
                GUILayout.BeginVertical(GUILayout.Width(220)); // Left panel
                {
                    Page.Show();
                    if (GUILayout.Button("Settings"))
                    {
                        if (Page is PresetPage) SetPage(new SettingsPage());
                        else SetPage(new PresetPage());
                    }
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical(); // Right panel
                {
                    if (GUILayout.Button("Add config"))
                    {
                        string newName = "New config";
                        Preset newcfg = new Preset(newName);
                        newcfg.Save(DirUtils.PresetsDir);
                        Settings.Instance.Presets.All.Add(newcfg);
                        Settings.Instance.Presets.current = Settings.Instance.Presets.All[Settings.Instance.Presets.All.Count - 1];
                        Settings.Instance.Save();
                        SetPage(new PresetPage());
                    }
                    presetsListScroll = GUILayout.BeginScrollView(presetsListScroll);
                    foreach (var config in Settings.Instance.Presets.All)
                    {
                        if (Settings.Instance.Presets.current == config)
                        {
                            GUILayout.Label(config.name, Styles.selected_button);
                        }
                        else
                        {
                            if (GUILayout.Button(config.name))
                            {
                                Settings.Instance.Presets.current = config;
                                Settings.Instance.Save();
                                SetPage(new PresetPage());
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

        public void SetPage(IPage newPage)
        {
            Page = newPage;
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
