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
            var codes = new List<CodeInstruction>(instructions);
            return CodeReuse(codes);
        }
        
        [HarmonyDebug]
        [HarmonyPatch(typeof(ProfileManager), nameof(ProfileManager.LoadSaveData))]
        [HarmonyTranspiler]
        [HarmonyAfter("io.github.ross-carran.BattleTechTools")]
        static IEnumerable<CodeInstruction> Transpiler3(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = new List<CodeInstruction>(instructions);
            Label newLoc1 = generator.DefineLabel();
            Label newLoc2 = generator.DefineLabel();
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].ToString().Contains("call static bool BattleTech.DebugBridge::get_TestToolsEnabled()") && 
                    codes[i + 1].ToString().Contains("brfalse"))
                {
                    codes[i + 1] = new CodeInstruction(OpCodes.Brfalse, newLoc1);
                }

                if (codes[i].ToString().Contains("call static bool string::IsNullOrEmpty(string value)") &&
                    codes[i + 1].ToString().Contains("brtrue"))
                {
                    codes[i + 1] = new CodeInstruction(OpCodes.Brtrue, newLoc2);
                }

                if (codes[i].ToString()
                    .Contains("call static string UnityEngine.Application::get_persistentDataPath()"))
                {
                    MethodInfo myMeth = SymbolExtensions.GetMethodInfo((() => Settings.RootSaveDirPath()));
                    codes [i] = new CodeInstruction(OpCodes.Call, myMeth);
                    codes[i].labels.Add(newLoc1);
                    codes[i].labels.Add(newLoc2);
                }
            }
            return codes;
        }

        
        [HarmonyPatch(typeof(ProfileManager), nameof(ProfileManager.ClearProfiles))]
        [HarmonyTranspiler]
        [HarmonyAfter("io.github.ross-carran.BattleTechTools")]
        static IEnumerable<CodeInstruction> Transpiler4(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            return CodeReuse(codes);      
        }

        public static GameInstance ChangeLocalSaveFolder(GameInstance game)
        {
            game.SaveManager.saveSystem.cloudWriteLocation.rootPath = PathStringGenerator(SaveSystem.CloudFolder);
            game.SaveManager.saveSystem.localWriteLocation.rootPath = PathStringGenerator(SaveSystem.StandaloneFolder);
            return game;
        }

        public static string PathStringGenerator(string saveSystemFolderPath)
        {
            return Settings.RootSaveDirPath() +
                   Path.DirectorySeparatorChar +
                   Settings.UserDefinedDirectory +
                   Path.DirectorySeparatorChar +
                   saveSystemFolderPath;
        }

        private static IEnumerable<CodeInstruction> CodeReuse(IEnumerable<CodeInstruction> myInstructions)
        {
            var codes = new List<CodeInstruction>(myInstructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].ToString().Contains("static string UnityEngine.Application::get_persistentDataPath()"))
                {
                    MethodInfo myMeth = SymbolExtensions.GetMethodInfo((() => Settings.RootSaveDirPath()));
                    codes [i] = new CodeInstruction(OpCodes.Call, myMeth);
                    //s_log.Log("target Found");
                    
                }
                //s_log.Log(codes[i].ToString());
            }
            return codes;
        }
    }
}