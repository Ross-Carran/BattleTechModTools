using System.IO;
using BattleTech;
using HarmonyLib;
using HBS.Logging;

namespace BTPathMod
{
    
    public static class BTSavePathMod
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(BTPathMod));
        public static void Start()
        {
            s_log.Log("Starting");
            s_log.Log(GamePath());

            Harmony.CreateAndPatchAll(typeof(BTSavePathMod));
        
            s_log.Log("Started");
        }
    
        private static string GamePath()
        {
            var myPath = Directory.GetCurrentDirectory();
            return myPath;
        }

        [HarmonyPatch(typeof(UnityGameInstance), nameof(UnityGameInstance.Reset))]
        [HarmonyPostfix]
        [HarmonyAfter("io.github.ross-carran.BattleTechTools")]
        public static void GetSaveGamePath(UnityGameInstance ___instance)
        {
            s_log.Log(___instance.Game.SaveManager.saveSystem.localWriteLocation.rootPath);
            ___instance.Game = Modified(new GameInstance());
        }

        public static GameInstance Modified(GameInstance patchInstance)
        {
            
            return patchInstance;
        }
    }
}