using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using BattleTech;
using BattleTech.Save;
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

        [HarmonyPatch(typeof(ProfileManager), nameof(ProfileManager.Save))]
        [HarmonyTranspiler]
        [HarmonyAfter("io.github.ross-carran.BattleTechTools")]
        static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>();
            return codes;
        }

        [HarmonyPatch(typeof(ProfileManager), nameof(ProfileManager.LoadSaveData))]
        [HarmonyTranspiler]
        [HarmonyAfter("io.github.ross-carran.BattleTechTools")]
        static IEnumerable<CodeInstruction> Transpiler3(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>();
            return codes; 
        }

        [HarmonyPatch(typeof(ProfileManager), nameof(ProfileManager.ClearProfiles))]
        [HarmonyTranspiler]
        [HarmonyAfter("io.github.ross-carran.BattleTechTools")]
        static IEnumerable<CodeInstruction> Transpiler4(IEnumerable<CodeInstruction> instruction)
        {
            var codes = new List<CodeInstruction>();
            return codes;       
        }

        public static GameInstance ChangeLocalSaveFolder(GameInstance game)
        {
            game.SaveManager.saveSystem.cloudWriteLocation.rootPath = PathStringGenerator(SaveSystem.CloudFolder);
            game.SaveManager.saveSystem.localWriteLocation.rootPath = PathStringGenerator(SaveSystem.StandaloneFolder);
            return game;
        }

        public static string PathStringGenerator(string saveSystemFolderPath)
        {
            return Settings.RootSaveDirPath +
                   Path.DirectorySeparatorChar +
                   Settings.UserDefinedDirectory +
                   Path.DirectorySeparatorChar +
                   saveSystemFolderPath;
        }
    }
}