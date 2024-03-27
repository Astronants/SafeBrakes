using System.IO;

namespace SafeBrakes
{
    internal class DirUtils
    {
        internal static readonly string GameDataDir = UrlDir.CreateApplicationPath("GameData");
        internal static readonly string ModDir = Path.Combine(GameDataDir, "SafeBrakes");
        internal static readonly string AppIconsDir = Path.Combine(ModDir, "Textures");
        internal static readonly string PresetsDir = Path.Combine(ModDir, "Settings");
    }
}
