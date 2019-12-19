using System;
using UnityEngine;
using KSP.UI.Screens;
using System.IO;

namespace SafeBrakes
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AppLauncherButton : MonoBehaviour
    {
        private enum IconStyle
        {
            NORMAL,
            ABS,
            SAB,
            ACTIVE
        }
        private static IconStyle currentIcon = IconStyle.NORMAL;
        
        private static Texture2D app_icon_normal;
        private static Texture2D app_icon_active;
        private static Texture2D app_icon_ABS;
        private static Texture2D app_icon_SAB;
        
        public static ApplicationLauncherButton appButton;

        public void Start()
        {
            string texturePath = PresetsHandler.assembly_dir.Replace("Plugins", "Textures/");

            app_icon_normal = new Texture2D(36, 36);
            app_icon_active = new Texture2D(36, 36);
            app_icon_ABS = new Texture2D(36, 36);
            app_icon_SAB = new Texture2D(36, 36);
            app_icon_normal.LoadImage(File.ReadAllBytes(texturePath + "appIcon_N.png"));
            app_icon_active.LoadImage(File.ReadAllBytes(texturePath + "appIcon_A.png"));
            app_icon_ABS.LoadImage(File.ReadAllBytes(texturePath + "appIcon_ABS.png"));
            app_icon_SAB.LoadImage(File.ReadAllBytes(texturePath + "appIcon_SAB.png"));
            
            GameEvents.onGUIApplicationLauncherReady.Add(this.CreateAppButton);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(this.DestroyAppButton);
        }

        public void OnDestroy()
        {
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
            if (appButton != null)
            {
                Logger.Log("Destroying app button.");
                ApplicationLauncher.Instance.RemoveModApplication(appButton);
                Destroy(appButton);
                appButton = null;
            }
    }

        private void CreateAppButton()
        {
            if (appButton == null)
            {
                Logger.Log("Creating app button.");
                appButton = ApplicationLauncher.Instance.AddModApplication(
                    OnButtonTrue, //onTrue
                    OnButtonFalse, //onFalse
                    null, //onHover
                    null, //onHoverOut
                    null, //onEnable
                    null, //onDisable
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW,
                    app_icon_normal
                    );
            }
        }

        private void OnButtonTrue()
        {
            PresetsHandler.mainGUI.window_enabled = true;
        }

        private void OnButtonFalse()
        {
            if (PresetsHandler.mainGUI.confirmGUI != null) PresetsHandler.mainGUI.confirmGUI.CloseGUI();
            PresetsHandler.mainGUI.window_enabled = false;
            Settings.Fetch.Save();
        }

        public static void ABS_active(bool state)
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

        public static void SAB_active(bool state)
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

        private static void ChangeIcon(IconStyle style)
        {
            if (appButton == null) return;
            switch (style)
            {
                case IconStyle.ABS:
                    appButton.SetTexture(app_icon_ABS);
                    currentIcon = IconStyle.ABS;
                    break;
                case IconStyle.SAB:
                    appButton.SetTexture(app_icon_SAB);
                    currentIcon = IconStyle.SAB;
                    break;
                case IconStyle.ACTIVE:
                    appButton.SetTexture(app_icon_active);
                    currentIcon = IconStyle.ACTIVE;
                    break;
                default:
                    appButton.SetTexture(app_icon_normal);
                    currentIcon = IconStyle.NORMAL;
                    break;
            }
        }
    }
}
