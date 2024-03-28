using UnityEngine;

namespace SafeBrakes.UI
{
    internal class SettingsPage : IPage
    {
        private bool useKSPskin;
        private Vector2 scroll;

        public void Show()
        {
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(150));
            {
                useKSPskin = GUILayout.Toggle(useKSPskin, "use KSP Skin");
                if (GUILayout.Button("Reload Presets"))
                {
                    Settings.Instance.Presets.LoadPresets();
                }
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save")) Save();
            GUILayout.EndHorizontal();
        }

        public void Update()
        {
            this.useKSPskin = Settings.Instance.useKSPskin;
        }

        public void Save()
        {
            Settings.Instance.useKSPskin = this.useKSPskin;
            Settings.Instance.Save();
        }
    }
}
