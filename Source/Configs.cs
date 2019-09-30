using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

namespace SafeBrakes
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    class Configs : MonoBehaviour
    {
        Dictionary<string, string> mod_settings;

        public void Awake()
        {
            //loading the plugin's cfg file
            FileStream PluginCfgStream = new FileStream(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", "/").Replace("Plugins", "SafeBrakes.cfg"), FileMode.Open, FileAccess.Read);
            Dictionary<string, string> PluginCfg = Deserialize(PluginCfgStream);
            //loading the mod's settings
            string cfgFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("\\", "/").Replace("Plugins", PluginCfg["SettingsFile"]);
            FileStream cfgFileStream = new FileStream(cfgFile, FileMode.Open, FileAccess.Read);
            mod_settings = Deserialize(cfgFileStream);
        }

        public Dictionary<string, string> Deserialize(FileStream fileStream)
        {
            StreamReader streamReader = new StreamReader(fileStream);
            Dictionary<string, string> configs = new Dictionary<string, string>();
            int line_nbr = 0;
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                line_nbr++;
                if (line.Length > 0 && !line.StartsWith("#"))
                {
                    try
                    {
                        string setting_name = line.Substring(0, line.IndexOf("=")).Trim();
                        string setting_value = line.Substring(line.IndexOf("=") + 1, line.IndexOf(";") - line.IndexOf("=") - 1).Trim();
                        configs.Add(setting_name, setting_value);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"SafeBrakes => {Path.GetFileName(fileStream.Name)} L.{line_nbr}: an error has occurred while loading the setting!");
                        Debug.LogException(e);
                    }
                }
            }

            streamReader.Close();
            return configs;
        }

        public T Fetch<T>(string key, T defaultValue)
        {
            try
            {
                return (T)Convert.ChangeType(mod_settings[key], typeof(T));
            }
            catch (Exception e)
            {
                return defaultValue;
            }
        }

    }
}
