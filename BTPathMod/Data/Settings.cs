using System.IO;

namespace BTPathMod.Data
{
    public static class Settings
    {
        private static string RootSaveDir = "GameSaves";
        public static string UserDefinedDirectory = "Player-Fatigue-Testing";

        public static string RootSaveDirPath()
        {
            return Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + RootSaveDir;
        } 
    }
}