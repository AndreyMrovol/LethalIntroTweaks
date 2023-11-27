using HarmonyLib;

namespace IntroTweaks.Patches {
    [HarmonyPatch(typeof(MenuManager))]
    internal class MenuManagerPatch {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void AutoSelectHost(MenuManager __instance) {
            if (!Plugin.Config.AUTO_SELECT_HOST)
                return;

            if (Plugin.SelectedMode.Equals("lan")) {
                __instance.lanWarningContainer.SetActive(false);
            }

            __instance.ClickHostButton();
        }
    }
}
