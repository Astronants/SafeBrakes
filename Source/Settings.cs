using System;
using System.Linq;
using UnityEngine;

namespace SafeBrakes
{
    class Settings
    {
        private static Settings instance;
        public static Settings Instance => instance ?? (instance = new Settings());

        private readonly string settingsFile = UrlDir.PathCombine(DirUtils.ModDir, "SafeBrakes.cfg");

        public static void Instantiate()
        {
            instance = new Settings();
        }

        public bool useKSPskin = true;

        public Settings()
        {
            ConfigNode pluginCfg = ConfigNode.Load(settingsFile);
            try
            {
                PresetsHandler.Instance.current = PresetsHandler.Instance.allConfigs.Where(cfg => cfg.FileName == pluginCfg.GetValue("SettingsFile")).First();
            }
            catch
            {
                PresetsHandler.Instance.current = PresetsHandler.Instance.allConfigs.Where(cfg => cfg.FileName == PresetsHandler.defaultPreset).First();
            }

            try { useKSPskin = bool.Parse(pluginCfg.GetValue("KSPskin")); } catch { }

            Vector2 windowSize = UI.Main.DefaultskinSize;
            if (useKSPskin) windowSize = UI.Main.KSPskinSize;
            try
            {
                UI.Main.windowRect.position = new Vector2(Mathf.Clamp(float.Parse(pluginCfg.GetValue("X")), 0, Screen.width - windowSize.x), Mathf.Clamp(float.Parse(pluginCfg.GetValue("Y")), 0, Screen.height - windowSize.y));
            }
            catch
            {
                UI.Main.windowRect.position = new Vector2((Screen.width - windowSize.x) / 2, (Screen.height - windowSize.y) / 2);
            }
        }

        public bool Save()
        {
            try
            {
                ConfigNode cfg = new ConfigNode();
                cfg.AddValue("SettingsFile", PresetsHandler.Instance.current.FileName);
                cfg.AddValue("KSPskin", useKSPskin);
                cfg.AddValue("X", UI.Main.windowRect.position.x);
                cfg.AddValue("Y", UI.Main.windowRect.position.y);
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
