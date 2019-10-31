using System.IO;
using UnityEngine;

namespace SafeBrakes
{
    class ConfirmGUI : MonoBehaviour
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
            PresetsGUI.confirmGUI = null;
        }

        public void OnGUI()
        {
            if (Configs.KSPSkin) { GUI.skin = HighLogic.Skin; }
            window_pos = GUILayout.Window(this.GetInstanceID(), window_pos, this.Window, "Confirm delete", GUILayout.Width(1f), GUILayout.Height(1f));
            if (!isCentered && window_pos.width > 0f && window_pos.height > 0f)
            {
                window_pos.center = new Vector2(PresetsGUI.window_pos.center.x, PresetsGUI.window_pos.center.y);
                isCentered = true;
            }
            GUI.BringWindowToFront(this.GetInstanceID());
        }

        private void Window(int id)
        {
            GUILayout.BeginVertical(GUILayout.Width(300f));
            GUILayout.Label($"Are you sure you want to delete {Configs.current.name}?");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("No", new GUIStyle(GUI.skin.button) { padding = new RectOffset(10, 10, GUI.skin.button.padding.top, GUI.skin.button.padding.bottom) }))
            {
                CloseGUI();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Yes", new GUIStyle(GUI.skin.button) { padding = new RectOffset(10, 10, GUI.skin.button.padding.top, GUI.skin.button.padding.bottom) }))
            {
                File.Delete(Configs.presets_dir + Configs.current.fileName);
                int i = Configs.allConfigs.IndexOf(Configs.current);
                Configs.allConfigs.Remove(Configs.current);
                Configs.Update_Plugin(i < Configs.allConfigs.Count ? Configs.allConfigs[i] : Configs.allConfigs[i - 1]);
                Configs.Update_PresetsGUI();
                Logger.Log("Config deleted.");
                ScreenMessages.PostScreenMessage("[SafeBrakes]: Config deleted.", 5, ScreenMessageStyle.UPPER_CENTER);
                CloseGUI();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}
