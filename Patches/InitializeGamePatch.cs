using HarmonyLib;
using IntroTweaks.Utils;
using UnityEngine.SceneManagement;

namespace IntroTweaks.Patches {
    [HarmonyPatch(typeof(InitializeGame))]
    internal class InitializeGamePatch {
        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        static void DisableBootAnimation(ref bool __runOriginal) {
            DisplayUtil.Move(Plugin.Config.GAME_STARTUP_DISPLAY);

            if (Plugin.Config.SKIP_BOOT_ANIMATION) {
                SceneManager.LoadScene("MainMenu");
                __runOriginal = false;
            }
        }
    }
}
