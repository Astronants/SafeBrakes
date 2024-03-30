using UnityEngine;

namespace SafeBrakes.UI
{
    internal class SettingsPage : IPage
    {
        private bool useKSPskin;
        private Vector2 scroll;

        private readonly MainWindow Parent;

        internal SettingsPage(MainWindow parent)
        {
            this.Parent = parent;
        }

        public void Show()
        {
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(150));
            {
                useKSPskin = GUILayout.Toggle(useKSPskin, "use KSP Skin");
                if (GUILayout.Button("Reload Presets"))
                {
                    Parent.app.LoadPresets();
                }
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save")) Save();
            GUILayout.EndHorizontal();
        }

        public void Update()
        {
            this.useKSPskin = Parent.app.settings.useKSPskin;
        }

        public void Save()
        {
            Parent.app.settings.useKSPskin = this.useKSPskin;
            Parent.app.settings.Save();
        }
    }
}
