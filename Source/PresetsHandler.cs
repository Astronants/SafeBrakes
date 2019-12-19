using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SafeBrakes
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class PresetsHandler : MonoBehaviour
    {
        private static bool firstRun = true;

        public static readonly string assembly_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", "/");
        public static readonly string presets_dir = assembly_dir.Replace("Plugins", "Settings/");
        public const string defaultPreset = "Default.cfg";

        public static Preset current;
        public static List<Preset> allConfigs;
        
        public static Main mainGUI;

        public void Awake()
        {
            if (firstRun)
            {
                Load_Presets();
                Settings.Instantiate();
                firstRun = false;
            }
            if (mainGUI == null)
            {
                mainGUI = this.gameObject.AddComponent<Main>();
            }
        }

        public void OnDestroy()
        {
            Destroy(mainGUI);
            mainGUI = null;
        }
        
        public static void Load_Presets()
        {
            allConfigs = new List<Preset>();
            foreach (var file in Directory.GetFiles(presets_dir, "*.cfg"))
            {
                try
                {
                    Preset cfg = Preset.Load(file);
                    if (cfg.FileName == defaultPreset)
                    {
                        allConfigs.Insert(0, cfg);
                    }
                    else
                    {
                        allConfigs.Add(cfg);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"{Path.GetFileName(file)}: an error has occured while loading the preset.", e);
                }
            }
            if (!allConfigs.Where(preset => preset.FileName == defaultPreset).Any())
            {
                Preset cfg = new Preset("Default");
                cfg.Save(presets_dir);
                allConfigs.Insert(0, cfg);
            }
        }
        
    }
}
