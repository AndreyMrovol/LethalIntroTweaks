using HarmonyLib;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace IntroTweaks.Patches {
    [HarmonyPatch(typeof(MenuManager))]
    internal class MenuManagerPatch {
        public static int gameVer { get; private set; }
        public static TextMeshProUGUI versionText { get; private set; }

        public static List<GameObject> menuButtons { get; private set; }
        static Button cancelLoadingButton;

        static MenuManager Instance;

        [HarmonyPrefix]
        [HarmonyPatch("ClickHostButton")]
        static void DisableMenuOnHost(MenuManager __instance) {
            __instance.menuButtons.SetActive(false);
            if (Plugin.Config.CUSTOM_VERSION_TEXT) {
                versionText.transform.gameObject.SetActive(false);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("SetLoadingScreen")]
        static void AddLoadingScreenBackButton(MenuManager __instance, ref bool isLoading) {
            if (!isLoading) return;

            try {
                __instance.loadingScreen.SetActive(true);
                cancelLoadingButton.gameObject.SetActive(true);
            } catch (Exception e) {
                Plugin.Logger.LogError(e);
            }
        }

        static void InitCancelLoadingButton(MenuManager instance) {
            GameObject original = GetButton(instance.menuButtons, "QuitButton");
            if (!original) {
                Plugin.Logger.LogError("Failed to find original version text object.");
            }

            GameObject clone = Object.Instantiate(original, instance.loadingScreen.transform);
            clone.name = "LoadingScreenBackButton";

            cancelLoadingButton = clone.GetComponent<Button>();
            cancelLoadingButton.onClick.RemoveAllListeners();
            cancelLoadingButton.onClick.AddListener(LoadingScreenBackButtonClick);

            clone.GetComponentInChildren<TextMeshProUGUI>().text = "> Cancel";
        }

        static void LoadingScreenBackButtonClick() {
            Instance.MenuAudio.volume = 0.5f;
            Instance.menuButtons.SetActive(true);
            Instance.loadingScreen.SetActive(false);
            Instance.serverListUIContainer.SetActive(false);

            // TODO: Somehow prevent lobby from loading.
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
                    AnchorToBottom(clone.GetComponent<RectTransform>());
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
            Instance = __instance;
            InitCancelLoadingButton(__instance);

            try {
                // Make the white space equal on both sides of the panel.
                FixPanelAlignment(__instance.menuButtons);
                TweakCanvasSettings(__instance.menuButtons);

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

        static void TweakCanvasSettings(GameObject panel) {
            // This is more future-proof than `panel.transform.parent.parent`.
            var canvas = panel.GetComponentInParent<Canvas>(); 

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = true;
        }

        static void FixPanelAlignment(GameObject panel) {
            var panelRect = panel.GetComponent<RectTransform>();

            panelRect.anchoredPosition = Vector2.zero;
            panelRect.anchoredPosition3D = Vector3.zero;

            Plugin.Logger.LogDebug("Fixed menu panel alignment.");
        }

        static void AlignButtons(IEnumerable<GameObject> buttons) {
            foreach (GameObject obj in buttons) {
                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.pivot = Vector2.zero;
                rect.anchoredPosition = new Vector2(50, rect.anchoredPosition.y + 2);
                rect.offsetMax = new Vector2(rect.offsetMax.x - 5, rect.offsetMax.y);

                TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
                text.wordSpacing -= 25;
                //text.fontStyle = FontStyles.UpperCase;
                text.overflowMode = TextOverflowModes.Overflow;
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
