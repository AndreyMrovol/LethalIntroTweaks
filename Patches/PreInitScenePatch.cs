using HarmonyLib;
using UnityEngine.SceneManagement;

namespace IntroTweaks.Patches {
    [HarmonyPatch(typeof(PreInitSceneScript))]
    internal class PreSceneInitPatch {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void FinishedFirstLaunch() {
            IngamePlayerSettings.Instance?.SetPlayerFinishedLaunchOptions();
        }

        [HarmonyPostfix]
        [HarmonyPatch("SkipToFinalSetting")]
        internal static void SkipToSelectedMode(PreInitSceneScript __instance, ref bool ___choseLaunchOption) {
            if (Plugin.SelectedMode != "online" && Plugin.SelectedMode != "lan") {
                return;
            }

            #region Auto-skip
            __instance.LaunchSettingsPanels.Do(panel => panel.SetActive(false));
            __instance.currentLaunchSettingPanel = 0;
            __instance.headerText.text = "";
            __instance.blackTransition.gameObject.SetActive(false);
            __instance.continueButton.gameObject.SetActive(false);

            ___choseLaunchOption = true;
            __instance.mainAudio.PlayOneShot(__instance.selectSFX);
            #endregion

            bool online = Plugin.SelectedMode == "online";
            string sceneToLoad = online ? "InitScene" : "InitSceneLANMode";

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}