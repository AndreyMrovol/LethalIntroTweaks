using HarmonyLib;
using IntroTweaks.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IntroTweaks.Patches {
    [HarmonyPatch(typeof(MenuManager))]
    internal class MenuManagerPatch {
        public static int gameVer { get; private set; }
        public static TextMeshProUGUI versionText { get; private set; }

        public static List<GameObject> menuButtons { get; private set; }

        public static Color32 WHITE = new(0, 0, 0, 0);
        public static Color32 DARK_ORANGE = new(185, 120, 0, 255);
 
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
                GameObject original = __instance.menuButtons?.transform.parent.Find("VersionNum").gameObject;
                if (!original) {
                    Plugin.Logger.LogError("Failed to find original version text object.");
                }

                try {
                    GameObject clone = Object.Instantiate(original, __instance.menuButtons.transform);
                    original.SetActive(false);

                    clone.name = "VersionNumberText";

                    versionText = InitTextMesh(clone.GetComponent<TextMeshProUGUI>());
                    RectUtil.AnchorToBottom(clone.GetComponent<RectTransform>());
                }
                catch (Exception e) {
                    Plugin.Logger.LogError($"Error creating custom version text!\n{e}");
                }
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void StartPatch(MenuManager __instance) {
            try {
                // Make the white space equal on both sides of the panel.
                FixPanelAlignment(__instance.menuButtons);
                TweakCanvasSettings(__instance.menuButtons);
                TweakHostPanel(__instance.HostSettingsScreen);

                menuButtons = [
                    __instance.joinCrewButtonContainer,
                    __instance.lanButtonContainer,
                    GetButton(__instance.menuButtons, "HostButton"),
                    GetButton(__instance.menuButtons, "SettingsButton"),
                    GetButton(__instance.menuButtons, "Credits"),
                    GetButton(__instance.menuButtons, "QuitButton")
                ];

                // Make the messy menu buttons aligned with each other.
                AlignButtons(menuButtons);
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

            TweakTextSettings(tmp);

            return tmp;
        }

        static void TweakTextSettings(TextMeshProUGUI tmp, bool overflow = true, bool wordWrap = false) {
            if (overflow) tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.enableWordWrapping = wordWrap;

            tmp.faceColor = DARK_ORANGE;
            tmp.color = WHITE;
        }

        static void TweakCanvasSettings(GameObject panel) {
            // This is more future-proof than `panel.transform.parent.parent`.
            var canvas = panel.GetComponentInParent<Canvas>(); 

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = true;
        }

        static void TweakHostPanel(GameObject panel) {
            var rect = panel.GetComponent<RectTransform>();
            RectUtil.ResetAnchoredPos(rect);

            panel.transform.Find("Image").gameObject.SetActive(false);
        }

        static void FixPanelAlignment(GameObject panel) {
            var rect = panel.GetComponent<RectTransform>();

            RectUtil.EditOffsets(rect, new(-20, -20), new(20, 20));
            RectUtil.ResetAnchoredPos(rect);
            RectUtil.ResetSizeDelta(rect);

            Plugin.Logger.LogDebug("Fixed menu panel alignment.");
        }

        static void AlignButtons(IEnumerable<GameObject> buttons) {
            foreach (GameObject obj in buttons) {
                RectTransform rect = obj.GetComponent<RectTransform>();

                RectUtil.ResetPivot(rect);
                RectUtil.EditOffsets(rect, new(rect.offsetMax.x -5, rect.offsetMax.y), rect.offsetMin);
                rect.anchoredPosition = new(50, rect.anchoredPosition.y + 2);

                TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
                TweakTextSettings(text);

                text.wordSpacing -= 25;
                //text.fontStyle = FontStyles.UpperCase;
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
