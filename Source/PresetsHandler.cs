using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SafeBrakes.UI;

namespace SafeBrakes
{
    internal class PresetsHandler
    {
        internal const string defaultPreset = "Default.cfg";

        internal Preset current;
        internal readonly List<Preset> All = new List<Preset>();

        internal void LoadPresets()
        {
            All.Clear();
            foreach (var file in Directory.GetFiles(DirUtils.PresetsDir, "*.cfg"))
            {
                try
                {
                    Preset cfg = Preset.Load(file);
                    if (cfg.FileName == defaultPreset) All.Insert(0, cfg);
                    else All.Add(cfg);
                }
                catch (Exception e)
                {
                    Logger.Error($"{Path.GetFileName(file)}: an error has occured while loading the config.", e);
                }
            }

            if (All.Count == 0) CreateDefault();
        }

        private Preset CreateDefault()
        {
            Preset cfg = new Preset("Default");
            All.Add(cfg);
            cfg.Save(DirUtils.PresetsDir);
            return cfg;
        }

        internal Preset FirstOrDefault(string name)
        {
            Preset cfg = All.FirstOrDefault(e => e.FileName == name);
            if (cfg == default(Preset))
            {
                cfg = All.FirstOrDefault(e => e.FileName == defaultPreset);
                if (cfg == default(Preset)) cfg = CreateDefault();
            }
            return cfg;
        }

        internal bool Save()
        {
            return current.Save(DirUtils.PresetsDir);
        }

        internal void Delete()
        {
            try
            {
                int i = All.IndexOf(current);

                File.Delete(Path.Combine(DirUtils.PresetsDir, current.FileName));
                All.Remove(current);

                current = i < All.Count ? All[i] : All[i - 1];
                Settings.Instance.Save();

                AppLauncherButton.Instance.Window.Page.Update();
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
