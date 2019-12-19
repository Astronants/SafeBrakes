using System;
using System.Linq;
using UnityEngine;

namespace SafeBrakes
{
    class Settings
    {
        private static Settings instance;
        public static Settings Fetch
        {
            get
            {
                if (instance == null)
                {
                    Instantiate();
                }
                return instance;
            }
        }

        public static void Instantiate()
        {
            instance = new Settings();
        }

        public bool KSPSkin = true;

        public Settings()
        {
            ConfigNode pluginCfg = ConfigNode.Load(PresetsHandler.assembly_dir.Replace("Plugins", "SafeBrakes.cfg"));
            try
            {
                PresetsHandler.current = PresetsHandler.allConfigs.Where(cfg => cfg.FileName == pluginCfg.GetValue("SettingsFile")).First();
            }
            catch
            {
                PresetsHandler.current = PresetsHandler.allConfigs.Where(cfg => cfg.FileName == PresetsHandler.defaultPreset).First();
            }

            try { KSPSkin = bool.Parse(pluginCfg.GetValue("KSPskin")); } catch { }

            Vector2 windowSize = Main.DefaultskinSize;
            if (KSPSkin) windowSize = Main.KSPskinSize;
            try
            {
                Main.windowRect.position = new Vector2(Mathf.Clamp(float.Parse(pluginCfg.GetValue("X")), 0, Screen.width - windowSize.x), Mathf.Clamp(float.Parse(pluginCfg.GetValue("Y")), 0, Screen.height - windowSize.y));
            }
            catch
            {
                Main.windowRect.position = new Vector2((Screen.width - windowSize.x) / 2, (Screen.height - windowSize.y) / 2);
            }
        }

        public bool Save()
        {
            try
            {
                ConfigNode cfg = new ConfigNode();
                cfg.AddValue("SettingsFile", PresetsHandler.current.FileName);
                cfg.AddValue("KSPskin", KSPSkin);
                cfg.AddValue("X", Main.windowRect.position.x);
                cfg.AddValue("Y", Main.windowRect.position.y);
                cfg.Save(PresetsHandler.assembly_dir.Replace("Plugins", "SafeBrakes.cfg"));
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
