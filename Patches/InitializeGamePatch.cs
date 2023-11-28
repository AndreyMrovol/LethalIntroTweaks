using HarmonyLib;
using UnityEngine.SceneManagement;

namespace IntroTweaks.Patches {
    [HarmonyPatch(typeof(InitializeGame))]
    internal class InitializeGamePatch {
        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        static void DisableBootAnimation(ref bool __runOriginal) {
            if (Plugin.Config.SKIP_BOOT_ANIMATION) {
                SceneManager.LoadScene("MainMenu");
                __runOriginal = false;
            }
        }
    }
}
