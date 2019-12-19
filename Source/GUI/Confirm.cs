using System;
using System.IO;
using UnityEngine;

namespace SafeBrakes
{
    class Confirm : MonoBehaviour
    {
        private bool gamepaused;
        public static Rect windowRect;

        private readonly Vector2 KSPskinSize = new Vector2(360, 72);
        private readonly Vector2 DefaultskinSize = new Vector2(360, 64);
        private Vector2 windowSize;

        public void Awake()
        {
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);

            windowSize = DefaultskinSize;
            if (Settings.Fetch.KSPSkin) windowSize = KSPskinSize;
            windowRect.position = new Vector2((Screen.width - windowSize.x) / 2, (Screen.height - windowSize.y) / 2);
        }

        public void CloseGUI()
        {
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            PresetsHandler.mainGUI.confirmGUI = null;
            Destroy(this);
        }

        public void OnGUI()
        {
            if (gamepaused) return;
            if (Settings.Fetch.KSPSkin) GUI.skin = HighLogic.Skin;

            windowRect = GUILayout.Window(this.GetInstanceID(), windowRect, this.Window, "Confirm delete", new GUIStyle(GUI.skin.window) { padding = new RectOffset(5, 5, GUI.skin.window.padding.top, 0) }, GUILayout.Width(1), GUILayout.Height(1));
            GUI.BringWindowToFront(this.GetInstanceID());
        }
        
        private void Window(int id)
        {
            GUILayout.BeginVertical(GUILayout.Width(350f));
            GUILayout.Label($"Are you sure you want to delete {PresetsHandler.current.name}?", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, stretchWidth = true });
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", new GUIStyle(GUI.skin.button) { padding = new RectOffset(10, 10, GUI.skin.button.padding.top, GUI.skin.button.padding.bottom) }))
            {
                CloseGUI();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Yes", new GUIStyle(GUI.skin.button) { padding = new RectOffset(10, 10, GUI.skin.button.padding.top, GUI.skin.button.padding.bottom) }))
            {
                try
                {
                    int i = PresetsHandler.allConfigs.IndexOf(PresetsHandler.current);

                    File.Delete(PresetsHandler.presets_dir + PresetsHandler.current.FileName);
                    PresetsHandler.allConfigs.Remove(PresetsHandler.current);

                    PresetsHandler.current = i < PresetsHandler.allConfigs.Count ? PresetsHandler.allConfigs[i] : PresetsHandler.allConfigs[i - 1];
                    Settings.Fetch.Save();

                    PresetsHandler.mainGUI.Update_PresetPage();
                    ScreenMessages.PostScreenMessage($"[{Logger.modName}]: Config deleted.", 5, ScreenMessageStyle.UPPER_CENTER);
                }
                catch (Exception e)
                {
                    Logger.Error("An error has occured while deleting the preset.", e);
                    ScreenMessages.PostScreenMessage($"[{Logger.modName}]: An error has occured while deleting the preset.", 5, ScreenMessageStyle.UPPER_CENTER);
                }
                CloseGUI();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
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
