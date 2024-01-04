using HarmonyLib;
using IntroTweaks.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IntroTweaks.Patches;

[HarmonyPatch(typeof(MenuManager))]
internal class MenuManagerPatch {
    public static int gameVer { get; private set; }
    public static TextMeshProUGUI versionText { get; private set; }

    public static Color32 DARK_ORANGE = new(175, 115, 0, 255);

    static GameObject activateCosmetics;

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
            if (Plugin.Config.FIX_MENU_PANELS) {
                // Make the white space equal on both sides of the panel.
                FixPanelAlignment(__instance.menuButtons);
                FixPanelAlignment(FindInParent(__instance.menuButtons, "LobbyHostSettings"));
                FixPanelAlignment(FindInParent(__instance.menuButtons, "LobbyList"));
                FixPanelAlignment(FindInParent(__instance.menuButtons, "LoadingScreen"));

                Plugin.Logger.LogDebug("Fixed menu panel alignment.");
            }

            if (Plugin.Config.IMPROVE_HOST_SCREEN) {
                // Hide the red background & private lobby tip
                __instance.HostSettingsScreen.transform.Find("Image").gameObject.SetActive(false);
                __instance.tipTextHostSettings.gameObject.SetActive(false);

                // Increase size of boxes
                var hostBox = __instance.HostSettingsOptionsLAN.transform.parent.parent;
                var filesBox = FindInParent(hostBox.gameObject, "FilesPanel").transform;

                hostBox.localScale = new(1.3f, 1.3f, 1.3f);
                filesBox.localScale = new(1.15f, 1.15f, 1.15f);

                RectUtil.EditOffsets(hostBox.GetComponent<RectTransform>(), new(80, 100), new(-180, -105));
                hostBox.localPosition = new(hostBox.localPosition.x - 60, 0, hostBox.localPosition.z);

                filesBox.localPosition = new(filesBox.localPosition.x - 80, 0, filesBox.localPosition.z);

                // Position boxes to screen center

            }

            GameObject[] buttons = [
                __instance.joinCrewButtonContainer,
                __instance.lanButtonContainer,
                GetButton(__instance.menuButtons, "HostButton"),
                GetButton(__instance.menuButtons, "SettingsButton"),
                GetButton(__instance.menuButtons, "Credits"),
                GetButton(__instance.menuButtons, "QuitButton"),
                GetButton(__instance.menuButtons, "ModSettingsButton")
            ];

            if (Plugin.Config.ALIGN_MENU_BUTTONS) {
                AlignButtons(buttons);
            }

            if (Plugin.Config.REMOVE_CREDITS_BUTTON) {
                RemoveCreditsButton(buttons);
            }

            bool fixCanvas = Plugin.Config.FIX_MENU_CANVAS;

            #region Handle MoreCompany edits if found.
            GameObject mc = GameObject.Find("GlobalScale");
            if (mc) {
                fixCanvas = false;

                mc.GetComponentInParent<Canvas>().pixelPerfect = true;
                GameObject cosmetics = mc.transform.Find("CosmeticsScreen").gameObject;

                #region Tweak character spin area.
                Transform spinArea = cosmetics.transform.Find("SpinAreaButton").transform;
                spinArea.localScale = new(0.48f, 0.55f, 0.46f);
                spinArea.position = new(421.65f, 245.7f, 200f);
                #endregion

                #region Button pos and bring to front.
                Transform exit = cosmetics.transform.Find("ExitButton").transform;
                Transform activate = FindInParent(cosmetics, "ActivateButton").transform;
                activateCosmetics = activate.gameObject;

                Vector3 buttonPos = new(424.06f, 241.65f, 166.2f);
                exit.position = activate.position = buttonPos;
                exit.SetAsLastSibling();
                #endregion

                #region Change cosmetics border scale.
                Transform border = cosmetics.transform.Find("CosmeticsHolderBorder").transform;
                border.localScale = new(2.4f, 2.1f, 1);
                #endregion

                #region Header scale & position.
                Transform header = __instance.menuButtons.transform.Find("HeaderImage").transform;
                header.localScale = new Vector3(5, 5, 5);

                Vector3 headerPos = new(1093.05f, 647.79f, -35.33f);
                header.position = headerPos;
                #endregion
            }

            TweakCanvasSettings(__instance.menuButtons, fixCanvas);
            #endregion
        }
        catch (Exception e) {
            Plugin.Logger.LogError(e);
        }
            
        if (Plugin.Config.REMOVE_NEWS_PANEL) {
            __instance.NewsPanel.SetActive(false);
        }

        if (Plugin.Config.REMOVE_LAN_WARNING) {
            __instance.lanWarningContainer.SetActive(false);
        }

        if (Plugin.Config.REMOVE_LAUNCHED_IN_LAN) {
            GameObject lanModeText = __instance.launchedInLanModeText?.gameObject;
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
    static void UpdatePatch(MenuManager __instance) {
        bool onMenu = __instance.menuButtons.activeSelf;
        bool onHostScreen = __instance.HostSettingsScreen.activeSelf;
        bool cosmeticsOpen = activateCosmetics.transform.parent.gameObject.activeSelf;

        // Override version text with game version.
        if (Plugin.Config.CUSTOM_VERSION_TEXT) {
            versionText.text = versionText.text.Replace("$VERSION", gameVer.ToString());

            var textObj = versionText.gameObject;
            if (!textObj.activeSelf && onMenu) {
                textObj.SetActive(true);
            }
        }

        if (!activateCosmetics.activeSelf && onMenu && !cosmeticsOpen) {
            activateCosmetics.SetActive(true);
        }

        if (!onHostScreen && !onMenu) {
            activateCosmetics.SetActive(false);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch("ClickHostButton")]
    static void DisableMenuOnHost(MenuManager __instance) {
        __instance.menuButtons.SetActive(false);

        if (!Plugin.Config.CUSTOM_VERSION_TEXT) return;
        versionText.gameObject.SetActive(false);
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
    }

    static void TweakCanvasSettings(GameObject panel, bool changeRenderMode) {
        // This is more future-proof than `panel.transform.parent.parent`.
        var canvas = panel.GetComponentInParent<Canvas>();
        canvas.pixelPerfect = true;

        if (changeRenderMode) {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    static void FixPanelAlignment(GameObject panel) {
        var rect = panel.GetComponent<RectTransform>();

        RectUtil.ResetSizeDelta(rect);
        RectUtil.ResetAnchoredPos(rect);
        RectUtil.EditOffsets(rect, new(-20, -25), new(20, 25));

        FixScale(panel);
    }

    internal static void FixScale(GameObject obj) {
        obj.transform.localScale = new(1.02f, 1.06f, 1.02f);
    }

    static void RemoveCreditsButton(IEnumerable<GameObject> buttons) {
        GameObject quitButton = buttons.First(b => b.name == "QuitButton");
        GameObject creditsButton = buttons.First(b => b.name == "Credits");

        // Disable credits button and move quit button there instead.
        creditsButton.SetActive(false);
        quitButton.transform.position = creditsButton.transform.position;

        // Move all buttons down slightly.
        foreach (GameObject obj in buttons) {
            if (!obj || obj == creditsButton) continue;

            var pos = obj.transform.position;
            obj.transform.position = new(pos.x, pos.y - 10f, pos.z);
        }

        Plugin.Logger.LogDebug("Removed credits button.");
    }

    static void AlignButtons(IEnumerable<GameObject> buttons) {
        var hostButtonPos = buttons.First(b => b.name == "HostButton").GetComponent<RectTransform>().localPosition;

        foreach (GameObject obj in buttons) {
            if (!obj) continue;

            #region Fix button rect
            RectTransform rect = obj.GetComponent<RectTransform>();

            int yOffset = Plugin.Config.REMOVE_CREDITS_BUTTON ? 20 : -5;
            rect.localPosition = new(hostButtonPos.x + 20, rect.localPosition.y + yOffset, hostButtonPos.z);
            #endregion

            #region Fix text mesh settings
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();

            FixScale(text.gameObject);
            TweakTextSettings(text);

            text.fontSize = 15;
            text.wordSpacing -= 25;
            //text.fontStyle = FontStyles.UpperCase;
            #endregion

            #region Fix button text rect
            var textRect = text.gameObject.GetComponent<RectTransform>();
            RectUtil.ResetAnchoredPos(textRect);
            RectUtil.ResetSizeDelta(textRect);
            RectUtil.EditOffsets(textRect, Vector2.zero, new(5, 0));
            #endregion
        }

        Plugin.Logger.LogDebug("Aligned menu buttons.");
    }

    static GameObject GetButton(GameObject panel, string name) {
        try {
            return panel.transform.Find(name).gameObject;
        } catch(Exception e) { 
            if (name != "ModSettingsButton")
                Plugin.Logger.LogError($"Error getting button: {name}\n{e}");

            return null;
        }
    }

    static GameObject FindInParent(GameObject child, string name) {
        Transform parent = child.transform.parent;

        try {
            return parent.Find(name).gameObject;
        } catch(Exception e) { 
            Plugin.Logger.LogError($"Error finding '{name}' in parent: {parent.name}\n{e}");
            return null;
        }
    }
}