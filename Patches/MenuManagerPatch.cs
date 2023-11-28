using HarmonyLib;
using TMPro;
using UnityEngine;

namespace IntroTweaks.Patches {
    [HarmonyPatch(typeof(MenuManager))]
    internal class MenuManagerPatch {
        static RectTransform versionTextRect = null;

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        static void OverrideVersionPosition() {
            versionTextRect.anchoredPosition = Vector2.zero;
            versionTextRect.anchoredPosition3D = Vector3.zero;

            versionTextRect.position = new Vector3(1085, 555f, -37);
            versionTextRect.rotation = Quaternion.identity;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        static bool DisableVersionText(MenuManager __instance) {
            GameObject original = __instance.versionNumberText.transform.gameObject;
            GameObject clone = Object.Instantiate(original, original.transform.parent);
            original.SetActive(false);

            clone.name = "VersionNumberText";

            TextMeshProUGUI tmpVersionText = clone.GetComponent<TextMeshProUGUI>();
            string gameVer = $"{GameNetworkManager.Instance.gameVersionNum - 16440}";

            tmpVersionText.text = Plugin.Config.VERSION_TEXT.Replace("$VERSION", gameVer);
            tmpVersionText.fontSize = tmpVersionText.fontSizeMin = 17f;
            tmpVersionText.enableAutoSizing = true;

            tmpVersionText.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            tmpVersionText.alignment = TextAlignmentOptions.Center;

            versionTextRect = clone.GetComponent<RectTransform>();

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void AutoSelectHost(MenuManager __instance) {
            __instance.lanWarningContainer.SetActive(false);

            if (Plugin.Config.AUTO_SELECT_HOST) {
                __instance.ClickHostButton();
            }
        }
    }
}
