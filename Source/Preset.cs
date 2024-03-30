using System;
using System.IO;

namespace SafeBrakes
{
    /// <summary>
    /// SafeBrakes settings preset.
    /// </summary>
    public class Preset
    {
        public string Name = "Default";
        public string FileName { get; private set; }
        public string OldFileName { get; private set; }
        public float abs_minSpd = 2.0f;
        public bool allow_sab = true;
        public float sab_highT = 80.0f;
        public float sab_lowT = 50.0f;

        public Preset(string name)
        {
            this.Name = name;
            this.FileName = this.Name + ".cfg";
            this.OldFileName = this.Name + ".cfg";
        }

        public static Preset Load(string file)
        {
            ConfigNode config = ConfigNode.Load(file);
            Preset preset = new Preset(Path.GetFileNameWithoutExtension(file));
            try { preset.abs_minSpd = float.Parse(config.GetValue("ABS_MinSpd")); } catch { }
            try { preset.allow_sab = bool.Parse(config.GetValue("SAB_Allow")); } catch { }
            try { preset.sab_highT = float.Parse(config.GetValue("SAB_HighTrigger")); } catch { }
            try { preset.sab_lowT = float.Parse(config.GetValue("SAB_LowTrigger")); } catch { }
            return preset;
        }

        public bool Save()
        {
            try
            {
                string newName = this.Name;
                int n = 2;
                while (UI.App.Instance.presets.Exists(cfg => cfg != this && cfg.Name == newName))
                {
                    newName = $"{this.Name} {n++}";
                }
                this.Name = newName;

                // if the fileName has been changed, delete the old file
                this.FileName = this.Name + ".cfg";
                if (this.FileName != this.OldFileName)
                {
                    File.Delete(Path.Combine(DirUtils.PresetsDir, this.OldFileName));
                    this.OldFileName = this.FileName;
                }

                // saving the preset
                ConfigNode config = new ConfigNode(this.Name);
                config.AddValue("ABS_MinSpd", this.abs_minSpd);
                config.AddValue("SAB_Allow", this.allow_sab);
                config.AddValue("SAB_HighTrigger", this.sab_highT);
                config.AddValue("SAB_LowTrigger", this.sab_lowT);
                return config.Save(Path.Combine(DirUtils.PresetsDir, this.FileName));
            }
            catch (Exception e)
            {
                Logger.Error("An error has occured while saving the config.", e);
                return false;
            }
        }
    }
}
