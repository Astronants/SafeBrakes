using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SafeBrakes
{
    class Preset
    {
        public string fileName;
        public string oldFileName;
        public string name;
        public float abs_minSpd = 2.0f;
        public bool sab_allow = true;
        public float sab_highT = 80.0f;
        public float sab_lowT = 50.0f;

        public Preset(string name)
        {
            this.name = name;
            this.fileName = this.name + ".cfg";
            this.oldFileName = this.name + ".cfg";
        }

        public static Preset Load(string path)
        {
            ConfigNode config = ConfigNode.Load(path);
            Preset preset = new Preset(config.GetValue("Name"));
            try { preset.fileName = Path.GetFileName(path); } catch { }
            try { preset.oldFileName = Path.GetFileName(path); } catch { }
            try { preset.abs_minSpd = float.Parse(config.GetValue("ABS_MinSpd")); } catch { }
            try { preset.sab_allow = bool.Parse(config.GetValue("SAB_Allow")); } catch { }
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
                while (Configs.allConfigs.Exists(cfg => cfg != this && cfg.name == newName))
                {
                    newName = $"{this.name} {n++}";
                }
                this.name = newName;
                // if the fileName has been changed, delete the old file
                this.fileName = this.name + ".cfg";
                if (this.fileName != this.oldFileName)
                {
                    File.Delete(directory + this.oldFileName);
                    this.oldFileName = this.fileName;
                }
                // saving the config
                ConfigNode config = new ConfigNode(this.name);
                config.AddValue("Name", this.name);
                config.AddValue("ABS_MinSpd", this.abs_minSpd);
                config.AddValue("SAB_Allow", this.sab_allow);
                config.AddValue("SAB_HighTrigger", this.sab_highT);
                config.AddValue("SAB_LowTrigger", this.sab_lowT);
                config.Save(directory + this.fileName);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error("An error has occured while saving the preset.", e);
                ScreenMessages.PostScreenMessage("An error has occured while saving the preset. See KSP.log for more info.", 5, ScreenMessageStyle.UPPER_CENTER, Color.red);
                return false;
            }
        }
    }
}
