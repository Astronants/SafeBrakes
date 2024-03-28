using System;
using UnityEngine;

namespace SafeBrakes
{
    [KSPAddon(KSPAddon.Startup.Flight, true)]
    class Settings : MonoBehaviour
    {
        public static Settings Instance { get; private set; }

        private readonly string settingsFile = UrlDir.PathCombine(DirUtils.ModDir, "SafeBrakes.cfg");
        private ConfigNode node;
        public bool useKSPskin = true;

        public readonly PresetsHandler Presets = new PresetsHandler();

        public void Start()
        {
            Presets.LoadPresets();
            node = ConfigNode.Load(settingsFile);
            if (node.HasValues("SettingsFile", "KSPskin"))
            {
                Presets.current = Presets.FirstOrDefault(node.GetValue("SettingsFile"));
                useKSPskin = bool.Parse(node.GetValue("KSPskin"));
            }
            Instance = this;
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
                cfg.AddValue("SettingsFile", Settings.Instance.Presets.current.FileName);
                cfg.AddValue("KSPskin", useKSPskin);
                cfg.AddValue("X", UI.MainWindow.windowRect.x);
                cfg.AddValue("Y", UI.MainWindow.windowRect.y);
                cfg.Save(settingsFile);
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
