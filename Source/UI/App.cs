using KSP.UI.Screens;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace SafeBrakes.UI
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    internal class AppIcons : MonoBehaviour
    {
        internal static Texture2D Normal { get; private set; }
        internal static Texture2D Active { get; private set; }
        internal static Texture2D ABS    { get; private set; }
        internal static Texture2D SAB    { get; private set; }

        internal void Start() // Fetch textures from the game's database to be used as icons by the applauncherbutton
        {
            Normal = GetDataBaseTexture("appIcon_N");
            Active = GetDataBaseTexture("appIcon_A");
            ABS = GetDataBaseTexture("appIcon_ABS");
            SAB = GetDataBaseTexture("appIcon_SAB");

        }
        private Texture2D GetDataBaseTexture(string textureName)
        {
            string path = KSPUtil.GetRelativePath(UrlDir.PathCombine(DirUtils.AppIconsDir, textureName), DirUtils.GameDataDir);
            return GameDatabase.Instance.GetTexture(path.Replace('\\', '/'), false);
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    internal class App : MonoBehaviour
    {
        private static App instance;
        public static App Instance => instance;

        internal MainWindow Window { get; private set; }


        private enum IconStyle
        {
            NORMAL,
            ABS,
            SAB,
            ACTIVE
        }
        private IconStyle currentIcon = IconStyle.NORMAL;

        public ApplicationLauncherButton button;
        internal readonly Settings settings = new Settings();
        internal PresetCollection presets = new PresetCollection();

        public void Start()
        {
            instance = this;
            GameEvents.onGUIApplicationLauncherReady.Add(this.CreateAppButton);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(this.DestroyAppButton);
            LoadPresets();
        }

        public void LoadPresets()
        {
            presets.Clear();
            foreach (var file in Directory.GetFiles(DirUtils.PresetsDir, "*.cfg"))
            {
                try
                {
                    Preset cfg = Preset.Load(file);
                    if (cfg.FileName == "Default.cfg") presets.Insert(0, cfg);
                    else presets.Add(cfg);
                }
                catch (Exception e)
                {
                    Logger.Error($"{Path.GetFileName(file)}: an error has occured while loading the config.", e);
                }
            }

            settings.SetSelectedPreset(ref presets);
            settings.Save();
        }

        public void OnDestroy()
        {
            if (Window != null) Destroy(Window);
            Window = null;
            instance = null;
            GameEvents.onGUIApplicationLauncherReady.Remove(this.CreateAppButton);
            GameEvents.onGUIApplicationLauncherUnreadifying.Remove(this.DestroyAppButton);
            DestroyAppButton();
        }

        private void DestroyAppButton(GameScenes data)
        {
            DestroyAppButton();
        }
        private void DestroyAppButton()
        {
            if (button != null)
            {
                Logger.Log("Destroying app button.");
                ApplicationLauncher.Instance.RemoveModApplication(button);
                Destroy(button);
                button = null;
            }
        }

        private void CreateAppButton()
        {
            if (button != null) return;

            Logger.Log("Creating app button.");
            button = ApplicationLauncher.Instance.AddModApplication(
                OnButtonTrue,   //onTrue
                OnButtonFalse,  //onFalse
                null,           //onHover
                null,           //onHoverOut
                null,           //onEnable
                null,           //onDisable
                ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW,
                AppIcons.Normal
                );
        }

        private void OnButtonTrue()
        {
            if (Window == null)
            {
                Window = gameObject.AddComponent<MainWindow>();
                Window.app = this;
                Window.SetPage(typeof(PresetPage));
            }
            Window.enabled = true;
        }

        private void OnButtonFalse()
        {
            settings.Save();
            Window.enabled = false;
        }

        public void ABS_active(bool state)
        {
            switch (state)
            {
                case false:
                    ChangeIcon(currentIcon == IconStyle.SAB ? IconStyle.SAB : IconStyle.NORMAL);
                    break;
                case true:
                    ChangeIcon(currentIcon == IconStyle.SAB ? IconStyle.ACTIVE : IconStyle.ABS);
                    break;
            }
        }

        public void SAB_active(bool state)
        {
            switch (state)
            {
                case false:
                    ChangeIcon(currentIcon == IconStyle.ABS ? IconStyle.ABS : IconStyle.NORMAL);
                    break;
                case true:
                    ChangeIcon(currentIcon == IconStyle.ABS ? IconStyle.ACTIVE : IconStyle.SAB);
                    break;
            }
        }

        private void ChangeIcon(IconStyle style)
        {
            if (button == null) return;
            switch (style)
            {
                case IconStyle.ABS:
                    button.SetTexture(AppIcons.ABS);
                    currentIcon = IconStyle.ABS;
                    break;
                case IconStyle.SAB:
                    button.SetTexture(AppIcons.SAB);
                    currentIcon = IconStyle.SAB;
                    break;
                case IconStyle.ACTIVE:
                    button.SetTexture(AppIcons.Active);
                    currentIcon = IconStyle.ACTIVE;
                    break;
                default:
                    button.SetTexture(AppIcons.Normal);
                    currentIcon = IconStyle.NORMAL;
                    break;
            }
        }
    }
}
