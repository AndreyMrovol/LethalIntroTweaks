using HarmonyLib;
using System;

using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace IntroTweaks.Patches {
    [HarmonyPatch(typeof(MenuManager))]
    internal class MenuManagerPatch {
        public static int gameVer { get; private set; }
        public static TextMeshProUGUI versionText { get; private set; }

        [HarmonyPrefix]
        [HarmonyPatch("ClickHostButton")]
        static void DisableMenuOnHost(MenuManager __instance) {
            __instance.menuButtons.SetActive(false);
            if (Plugin.Config.CUSTOM_VERSION_TEXT) {
                versionText.transform.gameObject.SetActive(false);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        static bool ReplaceVersionText(MenuManager __instance) {
            if (Plugin.Config.CUSTOM_VERSION_TEXT) {
                GameObject original = __instance.versionNumberText?.transform.gameObject;
                if (!original) {
                    Plugin.Logger.LogError("Reference to `versionNumberText` is null!");
                }

                try {
                    GameObject clone = Object.Instantiate(original, __instance.menuButtons.transform);
                    original.SetActive(false);

                    clone.name = "VersionNumberText";

                    versionText = InitTextMesh(clone.GetComponent<TextMeshProUGUI>());
                    AnchorToBottom(clone.GetComponent<RectTransform>());
                } catch(Exception e) {
                    Plugin.Logger.LogError($"Error creating custom version text!\n{e}");
                }
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void StartPatch(MenuManager __instance) {
            try {
                GameObject[] buttons = [
                    GetButton(__instance.menuButtons, "HostButton"),
                    __instance.lanButtonContainer,
                    GetButton(__instance.menuButtons, "SettingsButton"),
                    GetButton(__instance.menuButtons, "Credits"),
                    GetButton(__instance.menuButtons, "QuitButton")
                ];

                FixPanelAlignment(__instance.menuButtons);
                AlignButtons(buttons);
            } catch(Exception e) {
                Plugin.Logger.LogError(e);
            }
            
            if (Plugin.Config.REMOVE_NEWS_PANEL) {
                __instance.NewsPanel.SetActive(false);
            }

            if (Plugin.Config.REMOVE_LAN_WARNING) {
                __instance.lanWarningContainer.SetActive(false);
            }

            if (Plugin.Config.REMOVE_LAUNCHED_IN_LAN) {
                GameObject lanModeText = __instance.launchedInLanModeText?.transform.gameObject;
                if (lanModeText) {
                    lanModeText.SetActive(false);
                }
            }

            if (Plugin.Config.AUTO_SELECT_HOST) {
                __instance.ClickHostButton();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        static void UpdatePatch() {
            // Override version text with game version.
            if (Plugin.Config.CUSTOM_VERSION_TEXT) {
                versionText.text = versionText.text.Replace("$VERSION", gameVer.ToString());
            }
        }

        static TextMeshProUGUI InitTextMesh(TextMeshProUGUI tmp) {
            int realVer = GameNetworkManager.Instance.gameVersionNum;
            string format = Plugin.Config.VERSION_TEXT_FORMAT.ToLower();
            gameVer = Mathf.Abs(format.Equals("short") ? realVer - 16440 : realVer);

            tmp.text = Plugin.Config.VERSION_TEXT;
            tmp.fontSize = Mathf.Clamp(Plugin.Config.VERSION_TEXT_SIZE, 10, 40);
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

        static void FixPanelAlignment(GameObject panel) {
            // Make the white space equal on both sides of the panel.
            var panelRect = panel.GetComponent<RectTransform>();
 
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.anchoredPosition3D = Vector3.zero;

            var panelPos = panelRect.position;
            panelRect.position = new Vector3(panelPos.x + 2, panelPos.y, panelPos.z);
        }

        static void AlignButtons(GameObject[] buttons) {
            // Align the messy menu buttons with each other.
            foreach (GameObject button in buttons) {
                var rect = button.GetComponent<RectTransform>();

                rect.anchoredPosition = Vector2Int.FloorToInt(rect.anchoredPosition);
                rect.anchoredPosition3D = Vector3Int.FloorToInt(rect.anchoredPosition3D);

                Vector3 buttonPos = rect.position;
                int newX = (int) Math.Floor(buttonPos.x);

                rect.position = new Vector3(newX, buttonPos.y, buttonPos.z);
            }

            Plugin.Logger.LogDebug("Aligned menu buttons.");
        }

        static GameObject GetButton(GameObject panel, string name) {
            try {
                return panel.transform.Find(name).gameObject;
            } catch(Exception e) { 
                Plugin.Logger.LogError($"Error getting button: {name}\n{e}");
                return null;
            }
        }
    }
}
