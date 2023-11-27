using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IntroTweaks.Patches {
    [HarmonyPatch(typeof(PreInitSceneScript))]
    internal class PreSceneInitPatch {
        [HarmonyPrefix]
        [HarmonyPatch("SkipToFinalSetting")]
        static bool OverrideSkipToFinal() {
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void SkipToOnline(ref PreInitSceneScript __instance, ref bool ___choseLaunchOption) {
            __instance.LaunchSettingsPanels = new GameObject[0];
            __instance.currentLaunchSettingPanel = 0;
            __instance.headerText.text = "";

            ___choseLaunchOption = true;
            __instance.mainAudio.PlayOneShot(__instance.selectSFX);

            IngamePlayerSettings.Instance.SetPlayerFinishedLaunchOptions();
            IngamePlayerSettings.Instance.SaveChangedSettings();

            if (IngamePlayerSettings.Instance.encounteredErrorDuringSave)
                return;

            SceneManager.LoadSceneAsync("InitScene");
        }
    }
}