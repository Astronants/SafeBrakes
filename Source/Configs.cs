using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using KSP.UI.Screens;

namespace SafeBrakes
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    class Configs : MonoBehaviour
    {
        private static bool firstRun = true;
        private static readonly string assembly_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", "/");
        public static string presets_dir;
        public readonly static string defaultPreset = "Default.cfg";
        public static Preset current;
        public static List<Preset> allConfigs = new List<Preset>();

        private static ApplicationLauncherButton appButton;
        private static Texture2D appTex_normal, appTex_active, appTex_ABS, appTex_SAB;
        public static bool ABS_active, SAB_active;

        public static bool KSPSkin = true;
        public static PresetsGUI presetsGUI;

        public void Start()
        {
            presets_dir = assembly_dir.Replace("Plugins", "Settings/");
            if (firstRun)
            {
                Load_Presets();
                Load_PluginCfg();
                firstRun = false;
            }
            if (presetsGUI != null)
            {
                Update_PresetsGUI();
            }

            GameEvents.onGUIApplicationLauncherReady.Add(CreateAppButton);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(DestroyAppButton);
        }

        public void CreateAppButton()
        {
            if (ApplicationLauncher.Ready && appButton == null)
            {
                Logger.Log("creating app button.");
                //loading textures
                appTex_normal = new Texture2D(38, 38);
                appTex_normal.LoadImage(File.ReadAllBytes(assembly_dir.Replace("Plugins", "Textures/") + "appIcon_N.png"));
                appTex_active = new Texture2D(38, 38);
                appTex_active.LoadImage(File.ReadAllBytes(assembly_dir.Replace("Plugins", "Textures/") + "appIcon_A.png"));
                appTex_ABS = new Texture2D(38, 38);
                appTex_ABS.LoadImage(File.ReadAllBytes(assembly_dir.Replace("Plugins", "Textures/") + "appIcon_ABS.png"));
                appTex_SAB = new Texture2D(38, 38);
                appTex_SAB.LoadImage(File.ReadAllBytes(assembly_dir.Replace("Plugins", "Textures/") + "appIcon_SAB.png"));
                //creating button
                appButton = ApplicationLauncher.Instance.AddModApplication(
                    Toggle_PresetsGUI, //onTrue
                    Toggle_PresetsGUI, //onFalse
                    null, //onHover
                    null, //onHoverFalse
                    null, //onEnable
                    null, //onDisable
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW,
                    appTex_normal);
            }
        }
        public void DestroyAppButton(GameScenes data)
        {
            DestroyAppButton();
        }

        public void DestroyAppButton()
        {
            if (appButton != null)
            {
                Logger.Log("destroying app button.");
                GameEvents.onGUIApplicationLauncherReady.Remove(CreateAppButton);
                ApplicationLauncher.Instance.RemoveModApplication(appButton);
                appButton = null;
            }
        }

        public void OnDestroy()
        {
            DestroyAppButton();
            if (presetsGUI != null) { presetsGUI.CloseGUI(); }
        }

        public void Update()
        {
            if (appButton == null) { return; }
            if (!ABS_active && !SAB_active)
            {
                appButton.SetTexture(appTex_normal);
            }
            if (ABS_active && SAB_active)
            {
                appButton.SetTexture(appTex_active);
            }
            if (ABS_active && !SAB_active)
            {
                appButton.SetTexture(appTex_ABS);
            }
            if (!ABS_active && SAB_active)
            {
                appButton.SetTexture(appTex_SAB);
            }
        }

        private void Load_PluginCfg()
        {
            ConfigNode pluginCfg = ConfigNode.Load(assembly_dir.Replace("Plugins", "SafeBrakes.cfg"));
            try
            {
                current = allConfigs.Where(cfg => cfg.fileName == pluginCfg.GetValue("SettingsFile")).ToArray()[0];
            }
            catch
            {
                current = allConfigs.Where(cfg => cfg.fileName == defaultPreset).ToArray()[0];
            }
            try { KSPSkin = bool.Parse(pluginCfg.GetValue("KSPskin")); } catch { }
        }

        public static void Load_Presets()
        {
            allConfigs = new List<Preset>();
            foreach (var file in Directory.GetFiles(presets_dir, "*.cfg"))
            {
                try
                {
                    Preset cfg = Preset.Load(file);
                    allConfigs.Add(cfg);
                }
                catch (Exception e)
                {
                    Logger.Error($"{Path.GetFileName(file)}: an error has occured while loading the preset.", e);
                }
            }
            if (allConfigs.Count == 0)
            {
                Preset cfg = new Preset("Default");
                cfg.Save(presets_dir);
                allConfigs.Add(cfg);
            }
        }

        private void Toggle_PresetsGUI()
        {
            if (presetsGUI == null)
            {
                presetsGUI = this.gameObject.AddComponent<PresetsGUI>();
                Update_PresetsGUI();
            }
            else
            {
                presetsGUI.CloseGUI();
            }
        }

        public static void Update_PresetsGUI()
        {
            presetsGUI.show_name = current.name;
            presetsGUI.show_absMin = current.abs_minSpd.ToString();
            presetsGUI.show_sabAllow = current.sab_allow;
            presetsGUI.show_sabHigh = current.sab_highT.ToString();
            presetsGUI.show_sabLow = current.sab_lowT.ToString();
        }

        public static void Update_Plugin(Preset config)
        {
            current = config;
            Update_Plugin();
        }
        public static void Update_Plugin()
        {
            try
            {
                ConfigNode cfg = new ConfigNode();
                cfg.AddValue("SettingsFile", current.fileName);
                cfg.AddValue("KSPskin", KSPSkin);
                cfg.Save(assembly_dir.Replace("Plugins", "SafeBrakes.cfg"));
            }
            catch (Exception e)
            {
                Logger.Error("an error has occured while saving the settings.", e);
            }
        }
    }
}
