using KSP.UI.Screens;
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

        internal void Start() // Fetch textures from the game's database to use as icons in the applauncher
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
    public class AppLauncherButton : MonoBehaviour
    {
        private static AppLauncherButton instance;
        public static AppLauncherButton Instance => instance;

        private enum IconStyle
        {
            NORMAL,
            ABS,
            SAB,
            ACTIVE
        }
        private IconStyle currentIcon = IconStyle.NORMAL;

        public ApplicationLauncherButton appButton;

        public void Start()
        {
            instance = this;
            GameEvents.onGUIApplicationLauncherReady.Add(this.CreateAppButton);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(this.DestroyAppButton);
        }

        public void OnDestroy()
        {
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
        }

        private void OnButtonTrue()
        {
            PresetsHandler.Instance.mainGUI.window_enabled = true;
        }

        private void OnButtonFalse()
        {
            PresetsHandler.Instance.mainGUI.window_enabled = false;
            Settings.Instance.Save();
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
            if (appButton == null) return;
            switch (style)
            {
                case IconStyle.ABS:
                    appButton.SetTexture(AppIcons.ABS);
                    currentIcon = IconStyle.ABS;
                    break;
                case IconStyle.SAB:
                    appButton.SetTexture(AppIcons.SAB);
                    currentIcon = IconStyle.SAB;
                    break;
                case IconStyle.ACTIVE:
                    appButton.SetTexture(AppIcons.Active);
                    currentIcon = IconStyle.ACTIVE;
                    break;
                default:
                    appButton.SetTexture(AppIcons.Normal);
                    currentIcon = IconStyle.NORMAL;
                    break;
            }
        }
    }
}
