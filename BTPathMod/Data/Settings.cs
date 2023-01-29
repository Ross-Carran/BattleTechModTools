using System.IO;

namespace BTPathMod.Data
{
    public static class Settings
    {
        private static string RootSaveDir = "GameSaves";
        public static string UserDefinedDirectory = "MyTest3";
        public static string RootSaveDirPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + RootSaveDir;
    }
}