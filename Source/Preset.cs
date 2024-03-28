using System;
using System.IO;

namespace SafeBrakes
{
    /// <summary>
    /// SafeBrakes settings preset.
    /// </summary>
    class Preset
    {
        public string name;
        public string FileName { get; private set; }
        public string OldFileName { get; private set; }
        public float abs_minSpd = 2.0f;
        public bool allow_sab = true;
        public float sab_highT = 80.0f;
        public float sab_lowT = 50.0f;

        public Preset(string name)
        {
            this.name = name;
            this.FileName = this.name + ".cfg";
            this.OldFileName = this.name + ".cfg";
        }

        public static Preset Load(string path)
        {
            ConfigNode config = ConfigNode.Load(path);
            Preset preset = new Preset(config.GetValue("Name"))
            {
                FileName = Path.GetFileName(path),
                OldFileName = Path.GetFileName(path)
            };
            try { preset.abs_minSpd = float.Parse(config.GetValue("ABS_MinSpd")); } catch { }
            try { preset.allow_sab = bool.Parse(config.GetValue("SAB_Allow")); } catch { }
            try { preset.sab_highT = float.Parse(config.GetValue("SAB_HighTrigger")); } catch { }
            try { preset.sab_lowT = float.Parse(config.GetValue("SAB_LowTrigger")); } catch { }
            return preset;
        }

        public bool Save(string directory)
        {
            try
            {
                // prevent other configs with same name from being overwritten
                string newName = this.name;
                int n = 2;
                while (Settings.Instance.Presets.All.Exists(cfg => cfg != this && cfg.name == newName))
                {
                    newName = $"{this.name} {n++}";
                }
                this.name = newName;

                // if the fileName has been changed, delete the old file
                this.FileName = this.name + ".cfg";
                if (this.FileName != this.OldFileName)
                {
                    File.Delete(Path.Combine(directory, this.OldFileName));
                    this.OldFileName = this.FileName;
                }

                // saving the preset
                ConfigNode config = new ConfigNode(this.name);
                config.AddValue("Name", this.name);
                config.AddValue("ABS_MinSpd", this.abs_minSpd);
                config.AddValue("SAB_Allow", this.allow_sab);
                config.AddValue("SAB_HighTrigger", this.sab_highT);
                config.AddValue("SAB_LowTrigger", this.sab_lowT);
                return config.Save(Path.Combine(directory, this.FileName));
            }
            catch (Exception e)
            {
                Logger.Error("An error has occured while saving the config.", e);
                return false;
            }
        }
    }
}
