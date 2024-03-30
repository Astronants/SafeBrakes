using System;
using UnityEngine;

namespace SafeBrakes
{
    class Settings : MonoBehaviour
    {
        private readonly string settingsFile = UrlDir.PathCombine(DirUtils.ModDir, "SafeBrakes.cfg");
        private ConfigNode node;
        public bool useKSPskin = true;

        public Settings()
        {
            node = ConfigNode.Load(settingsFile);
            if (!node.HasValue("KSPskin")) return;
            useKSPskin = bool.Parse(node.GetValue("KSPskin"));
        }

        public void SetSelectedPreset(ref PresetCollection collection)
        {
            string file = node.GetValue("SettingsFile");
            collection.Selected = collection.FirstOrDefault(file);
        }

        public void SetWindowPosition(ref Rect rect)
        {
            if (!node.HasValues("X", "Y")) return;
            float x = Mathf.Clamp(int.Parse(node.GetValue("X")), 0, Screen.width - UI.MainWindow.windowRect.width);
            float y = Mathf.Clamp(int.Parse(node.GetValue("Y")), 0, Screen.height - UI.MainWindow.windowRect.height);
            rect.position = new Vector2(x, y);
        }

        public bool Save()
        {
            try
            {
                ConfigNode cfg = new ConfigNode("SafeBrakesSettings");
                cfg.AddValue("SettingsFile", UI.App.Instance.presets.Selected.FileName);
                cfg.AddValue("KSPskin", useKSPskin);
                cfg.AddValue("X", UI.MainWindow.windowRect.x);
                cfg.AddValue("Y", UI.MainWindow.windowRect.y);
                node = cfg;
                node.Save(settingsFile);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error("An error has occured while saving the settings.", e);
                return false;
            }
        }

    }
}
