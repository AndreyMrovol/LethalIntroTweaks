using HarmonyLib;
using UnityEngine.SceneManagement;

namespace IntroTweaks.Patches;

[HarmonyPatch(typeof(PreInitSceneScript))]
internal class PreInitScenePatch {
    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    static void FinishedFirstLaunch() {
        IngamePlayerSettings.Instance?.SetPlayerFinishedLaunchOptions();
    }

    [HarmonyPostfix]
    [HarmonyPatch("SkipToFinalSetting")]
    internal static void SkipToSelectedMode(PreInitSceneScript __instance, ref bool ___choseLaunchOption) {
        string mode = Plugin.SelectedMode;
        if (mode != "online" && mode != "lan") return;

        #region Skip panels & play sound
        __instance.LaunchSettingsPanels.Do(panel => panel.SetActive(false));
        __instance.currentLaunchSettingPanel = 0;
        __instance.headerText.text = "";
        __instance.blackTransition.gameObject.SetActive(false);
        __instance.continueButton.gameObject.SetActive(false);

        ___choseLaunchOption = true;
        __instance.mainAudio.PlayOneShot(__instance.selectSFX);
        #endregion

        #region Choose scene and load
        bool online = Plugin.SelectedMode == "online";
        string sceneToLoad = online ? "InitScene" : "InitSceneLANMode";

        if (Plugin.ModInstalled("LethalLevelLoader")) {
            Plugin.Logger.LogWarning(
                "\n===========================================================================================\n" +
                $"LethalLevelLoader was found.\nSkipping to {mode.ToUpper()} is delayed until all bundles have loaded.\n\n" +
                "This is temporary fix specfically for LLL, consider setting `bAutoSelectMode` to OFF instead!\n" +
                "Ideally, LLL should address this by loading in Awake or using DontDestroyOnLoad.\n" +
                "============================================================================================="
            );

            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
        #endregion
    }
}