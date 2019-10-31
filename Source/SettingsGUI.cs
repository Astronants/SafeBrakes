using UnityEngine;

namespace SafeBrakes
{
    class SettingsGUI : MonoBehaviour
    {
        private static bool isRunning;
        private bool isCentered;
        public static Rect window_pos;

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
            PresetsGUI.settingsGUI = null;
        }

        public void OnGUI()
        {
            if (Configs.KSPSkin) { GUI.skin = HighLogic.Skin; }
            window_pos = GUILayout.Window(this.GetInstanceID(), window_pos, this.Window, "SafeBrakes - Settings", GUILayout.Width(1f), GUILayout.Height(1f));

            if (!isCentered && window_pos.width > 0f && window_pos.height > 0f)
            {
                window_pos.center = new Vector2(PresetsGUI.window_pos.center.x, PresetsGUI.window_pos.center.y);
                isCentered = true;
            }
            GUI.BringWindowToFront(this.GetInstanceID());
        }

        private void Window(int id)
        {
            bool show_KSPskin = Configs.KSPSkin;
            GUILayout.BeginVertical(GUILayout.Width(200f));
            show_KSPskin = GUILayout.Toggle(show_KSPskin, "use KSP Skin");
            if (show_KSPskin != Configs.KSPSkin)
            {
                Configs.KSPSkin = show_KSPskin;
                Configs.Update_Plugin(Configs.current);
            }
            if (GUILayout.Button("Reload Presets"))
            {
                Configs.Load_Presets();
                Configs.Update_PresetsGUI();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close", new GUIStyle(GUI.skin.button) { padding = new RectOffset(10, 10, GUI.skin.button.padding.top, GUI.skin.button.padding.bottom) }, GUILayout.ExpandWidth(false)))
            {
                CloseGUI();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}
