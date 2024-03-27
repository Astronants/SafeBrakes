using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SafeBrakes
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class PresetsHandler : MonoBehaviour
    {
        private static bool FirstRun = true;
        public static PresetsHandler Instance { get; private set; }

        public const string defaultPreset = "Default.cfg";

        public Preset current;
        public List<Preset> allConfigs;
        public UI.Main mainGUI;

        public void Start()
        {
            Instance = this;
            LoadPresets();
            if (FirstRun)
            {
                Settings.Instantiate();
                mainGUI = this.gameObject.AddComponent<UI.Main>();
                FirstRun = false;
            }
        }

        public void OnDestroy()
        {
            Destroy(mainGUI);
            mainGUI = null;
            Instance = null;
        }

        public void LoadPresets()
        {
            allConfigs = new List<Preset>();
            foreach (var file in Directory.GetFiles(DirUtils.PresetsDir, "*.cfg"))
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
                    Logger.Error($"{Path.GetFileName(file)}: an error has occured while loading the config.", e);
                }
            }
            if (!allConfigs.Where(preset => preset.FileName == defaultPreset).Any())
            {
                Preset cfg = new Preset("Default");
                cfg.Save(DirUtils.PresetsDir);
                allConfigs.Insert(0, cfg);
            }
        }

        public bool SavePreset()
        {
            return current.Save(DirUtils.PresetsDir);
        }

        internal void DeletePreset()
        {
            try
            {
                int i = allConfigs.IndexOf(current);

                File.Delete(Path.Combine(DirUtils.PresetsDir, current.FileName));
                allConfigs.Remove(current);

                current = i < allConfigs.Count ? allConfigs[i] : allConfigs[i - 1];
                Settings.Instance.Save();

                UI.Main.Instance.Page.Update();
                ScreenMessages.PostScreenMessage($"[{Logger.modName}]: Config deleted.", 5, ScreenMessageStyle.UPPER_CENTER);
            }
            catch (Exception ex)
            {
                Logger.Error("An error has occured while deleting the config.", ex);
                ScreenMessages.PostScreenMessage($"[{Logger.modName}]: An error has occured while deleting the config.", 5, ScreenMessageStyle.UPPER_CENTER);
            }
        }
    }
}
