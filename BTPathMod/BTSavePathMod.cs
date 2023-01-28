using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using BattleTech;
using BattleTech.Save.Core;
using BTPathMod.Data;
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
         
            Harmony.CreateAndPatchAll(typeof(BTSavePathMod));
        
            s_log.Log("Started");
        }
        
        /**
         * Transpiler that redirects Gameinstance Initialisation so it goes through ChangeLocalSaveFolder First
         */
        [HarmonyDebug]
        [HarmonyPatch(typeof(UnityGameInstance), nameof(UnityGameInstance.Reset))]
        [HarmonyTranspiler]
        [HarmonyAfter("io.github.ross-carran.BattleTechTools")]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            foreach (var t in codes)
            {
                if (t.ToString().Contains("BattleTech.UnityGameInstance::set_Game(BattleTech.GameInstance value)"))
                {
                    t.opcode = OpCodes.Callvirt;
                    MethodInfo methTesting = SymbolExtensions.GetMethodInfo(() => ChangeLocalSaveFolder(new GameInstance()));
                    yield return new CodeInstruction(OpCodes.Call, methTesting);
                }
                yield return t;
            }
        }
        
        /**
         * This method generates the new path for the save folder
         * Currently This changes the path to the users BattleTech game folder then adds the user inputted folder
         * resulting in somefilestructure\{battletechgamefolder}\{userdefinedsavefolder}\{S0} system decides weather / or \ is used
         * Path.DirectorySeperatorChar is used to make sure the path is generated properly regardless of operating system.
         * Will more then likely make a settings class to give more control over this process
         */
        public static GameInstance ChangeLocalSaveFolder(GameInstance game)
        {
            var userDir = "ModTest";
            var test = Settings.rootSaveDir;
            game.SaveManager.saveSystem.cloudWriteLocation.rootPath = Directory.GetCurrentDirectory() +
                                                                      Path.DirectorySeparatorChar + userDir +
                                                                      Path.DirectorySeparatorChar +
                                                                      SaveSystem.CloudFolder;
            game.SaveManager.saveSystem.localWriteLocation.rootPath = Directory.GetCurrentDirectory() +
                                                                      Path.DirectorySeparatorChar + userDir +
                                                                      Path.DirectorySeparatorChar + 
                                                                      SaveSystem.StandaloneFolder;
            return game;
        }
    }
}