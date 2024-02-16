using HarmonyLib;
using IntroTweaks.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IntroTweaks.Patches;

[HarmonyPatch(typeof(InitializeGame))]
internal class InitializeGamePatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Start")]
    static void DisableBootAnimation(ref bool __runOriginal)
    {
        int startupDisplayIndex = Plugin.Config.GAME_STARTUP_DISPLAY.Value;
        if (startupDisplayIndex >= 0)
        {
            DisplayUtil.Move(startupDisplayIndex);
        }

        if (!Plugin.Config.SKIP_BOOT_ANIMATION.Value)
        {
            __runOriginal = true;
        }

        // https://github.com/flerouwu/LC_FastStartup/blob/main/FastStartup/TimeSavers/BootAnimSaver.cs#L19-L31
        // non-invasive way of skipping boot menus

        var game = Object.FindObjectOfType<InitializeGame>();
        if (game == null)
            return;

        Plugin.Logger.LogInfo("Skipping boot animation");
        game.runBootUpScreen = false;

        // Set animations to null just in case :3
        game.bootUpAnimation = null;
        game.bootUpAudio = null;
    }
}
