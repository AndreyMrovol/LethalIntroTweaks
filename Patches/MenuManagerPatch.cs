using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace IntroTweaks.Patches {
    [HarmonyPatch(typeof(MenuManager))]
    internal class MenuManagerPatch {
        public static int gameVer { get; private set; }
        public static TextMeshProUGUI versionText { get; private set; }

        [HarmonyPrefix]
        [HarmonyPatch("ClickHostButton")]
        static void DisableMenuOnHost(MenuManager __instance) {
            __instance.menuButtons.SetActive(false);
            versionText.transform.gameObject.SetActive(false);
        }

        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        static bool ReplaceVersionText(MenuManager __instance) {
            if (Plugin.Config.REPLACE_VERSION_TEXT) {
            GameObject original = __instance.versionNumberText.transform.gameObject;
            GameObject clone = Object.Instantiate(original, __instance.menuButtons.transform);
            original.SetActive(false);

            clone.name = "VersionNumberText";

            versionText = InitTextMesh(clone.GetComponent<TextMeshProUGUI>());
            AnchorToBottom(clone.GetComponent<RectTransform>());
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void StartPatch(MenuManager __instance) {
            if (Plugin.Config.REMOVE_NEWS_PANEL) {
                Object.Destroy(__instance.NewsPanel);
            }

            if (Plugin.Config.REMOVE_LAN_WARNING) {
                Object.Destroy(__instance.lanWarningContainer);
            }

            if (Plugin.Config.REMOVE_LAUNCHED_IN_LAN) {
                Object.Destroy(__instance.launchedInLanModeText.transform.gameObject);
            }

            if (Plugin.Config.AUTO_SELECT_HOST) {
                __instance.ClickHostButton();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        static void UpdatePatch(MenuManager __instance) {
            // Override version text with game version.
            if (Plugin.Config.REPLACE_VERSION_TEXT) {
            versionText.text = versionText.text.Replace("$VERSION", gameVer.ToString());
            }

            //bool atMenu = __instance.menuButtons.activeSelf;
            //bool pressedEsc = Keyboard.current.escapeKey.wasPressedThisFrame;

            //if (atMenu && pressedEsc) {
            //    Plugin.SelectedMode = "off";
            //    GameNetworkManager.Instance.disableSteam = !__instance.lanButtonContainer.activeSelf;

            //    SceneManager.LoadScene("InitSceneLaunchOptions");
            //}
        }

        static TextMeshProUGUI InitTextMesh(TextMeshProUGUI tmp) {
            gameVer = GameNetworkManager.Instance.gameVersionNum - 16440;

            tmp.text = Plugin.Config.VERSION_TEXT;
            tmp.fontSize = tmp.fontSizeMin = 22f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;

            return tmp;
        }

        static void AnchorToBottom(RectTransform rect) {
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);

            rect.anchoredPosition = Vector2.zero;
            rect.anchoredPosition3D = Vector3.zero;

            rect.rotation = Quaternion.identity;
            rect.position = new Vector3(
                Plugin.Config.VERSION_TEXT_X, 
                Plugin.Config.VERSION_TEXT_Y, 
                -37
            );
        }
    }
}
