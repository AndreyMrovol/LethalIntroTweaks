using HarmonyLib;
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
        static void SkipToOnline(PreInitSceneScript __instance, ref bool ___choseLaunchOption) {
            if (Plugin.SelectedMode.Equals("off")) {
                return;
            }

            #region Auto-skip
            __instance.LaunchSettingsPanels.Do(p => p.gameObject.SetActive(false));
            __instance.currentLaunchSettingPanel = 0;
            __instance.headerText.text = "";
            __instance.blackTransition.gameObject.SetActive(false);
            __instance.continueButton.gameObject.SetActive(false);

            ___choseLaunchOption = true;
            __instance.mainAudio.PlayOneShot(__instance.selectSFX);

            IngamePlayerSettings.Instance.SetPlayerFinishedLaunchOptions();
            IngamePlayerSettings.Instance.SaveChangedSettings();

            if (IngamePlayerSettings.Instance.encounteredErrorDuringSave)
                return;

            string sceneToLoad = Plugin.SelectedMode.Equals("online") ? "InitScene" : "InitSceneLANMode";
            SceneManager.LoadScene(sceneToLoad);
            #endregion
        }
    }
}