using HarmonyLib;
using IntroTweaks.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace IntroTweaks.Patches;

[HarmonyPatch(typeof(MenuManager))]
internal class MenuManagerPatch {
    public static int gameVer { get; private set; }
    public static TextMeshProUGUI versionText { get; private set; }

    public static Color32 DARK_ORANGE = new(175, 115, 0, 255);

    static GameObject activateCosmetics;

    internal static Transform MenuContainer = null;
    internal static Transform MenuPanel = null;
    internal static GameObject VersionNum = null;

    static MenuManager Instance;

    internal static void TryReplaceVersionText() {
        if (!Plugin.Config.CUSTOM_VERSION_TEXT) return;
        if (!VersionNum || !MenuPanel) return;

        GameObject clone = Object.Instantiate(VersionNum, MenuPanel);

        clone.name = "VersionNumberText";

        versionText = InitTextMesh(clone.GetComponent<TextMeshProUGUI>());
        RectUtil.AnchorToBottom(versionText.gameObject.GetComponent<RectTransform>());

        VersionNum.SetActive(false);
    }

    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    static void Init(MenuManager __instance) {
        Instance = __instance;
        Instance.StartCoroutine(PatchMenuDelayed());
    }

    private static IEnumerator PatchMenuDelayed() {
        /* 
        *  Waits a single frame.
        *
        *  This is slightly hacky but ensures all references are not null
        *  and that the mod makes its changes after all others.
        */  
        yield return new WaitForSeconds(0);

        MenuContainer = GameObject.Find("MenuContainer")?.transform;
        MenuPanel = MenuContainer?.Find("MainButtons");
        VersionNum = MenuContainer?.Find("VersionNum")?.gameObject;

        PatchMenu();
    }

    static void PatchMenu() {
        try {
            if (Plugin.Config.FIX_MENU_PANELS) {
                // Make the white space equal on both sides of the panel.
                FixPanelAlignment(MenuPanel);

                FixPanelAlignment(MenuContainer.Find("LobbyHostSettings"));
                FixPanelAlignment(MenuContainer.Find("LobbyList"));
                FixPanelAlignment(MenuContainer.Find("LoadingScreen"));

                Plugin.Logger.LogDebug("Fixed menu panel alignment.");
            }

            IEnumerable<GameObject> buttons = MenuPanel
                .GetComponentsInChildren<Button>(true)
                .Select(b => b.gameObject);

            if (Plugin.Config.ALIGN_MENU_BUTTONS) {
                AlignButtons(buttons);
            }

            if (Plugin.Config.REMOVE_CREDITS_BUTTON) {
                RemoveCreditsButton(buttons);
            }

            bool fixCanvas = Plugin.Config.FIX_MENU_CANVAS;

            #region Handle MoreCompany edits if found.
            if (Plugin.ModInstalled("MoreCompany")) {
                Plugin.Logger.LogDebug("MoreCompany found! Edits have been made to UI elements.");

                fixCanvas = false;

                GameObject mc = GameObject.Find("GlobalScale");
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
                Transform header = Instance.menuButtons.transform.Find("HeaderImage").transform;
                header.localScale = new Vector3(5, 5, 5);

                Vector3 headerPos = new(1093.05f, 647.79f, -35.33f);
                header.position = headerPos;
                #endregion
            }
            #endregion

            TweakCanvasSettings(Instance.menuButtons, fixCanvas);
        }
        catch (Exception e) {
            Plugin.Logger.LogError($"Error occurred in Start patch. SAJ.\n{e}");
        }
            
        if (Plugin.Config.REMOVE_NEWS_PANEL) {
            Instance.NewsPanel.SetActive(false);
        }

        if (Plugin.Config.REMOVE_LAN_WARNING) {
            Instance.lanWarningContainer.SetActive(false);
        }

        if (Plugin.Config.REMOVE_LAUNCHED_IN_LAN) {
            GameObject lanModeText = Instance.launchedInLanModeText?.gameObject;
            if (lanModeText) {
                lanModeText.SetActive(false);
            }
        }

        if (Plugin.Config.AUTO_SELECT_HOST) {
            Instance.ClickHostButton();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch("Update")]
    static void UpdatePatch(MenuManager __instance) {
        bool onMenu = __instance.menuButtons.activeSelf;

        // Create the new game version text.
        if (versionText == null) TryReplaceVersionText();
        else {
            // Make sure the text is correct.
            versionText.text = versionText.text.Replace("$VERSION", gameVer.ToString());

            var textObj = versionText.gameObject;
            if (!textObj.activeSelf && onMenu) {
                textObj.SetActive(true);
            }
        }

        //if (activateCosmetics != null) {
        //    bool onHostScreen = __instance.HostSettingsScreen.activeSelf;
        //    bool cosmeticsOpen = activateCosmetics.transform.parent.gameObject.activeSelf;

        //    if (!activateCosmetics.activeSelf && onMenu && !cosmeticsOpen) {
        //        activateCosmetics.SetActive(true);
        //    }

        //    if (!onHostScreen && !onMenu) {
        //        activateCosmetics.SetActive(false);
        //    }
        //}
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

    static void FixPanelAlignment(Transform panel) {
        var rect = panel.gameObject.GetComponent<RectTransform>();

        RectUtil.ResetSizeDelta(rect);
        RectUtil.ResetAnchoredPos(rect);
        RectUtil.EditOffsets(rect, new(-20, -25), new(20, 25));

        FixScale(panel.gameObject);
    }

    internal static void FixScale(GameObject obj) {
        obj.transform.localScale = new(1.02f, 1.06f, 1.02f);
    }

    static void RemoveCreditsButton(IEnumerable<GameObject> buttons) {
        GameObject quitButton = buttons.First(b => b.name == "QuitButton");
        GameObject creditsButton = buttons.First(b => b.name == "Credits");

        // Disable credits button and move quit button there instead.
        creditsButton.SetActive(false);
        quitButton.transform.localPosition = creditsButton.transform.localPosition;

        // Move all buttons down slightly.
        foreach (GameObject obj in buttons) {
            if (!obj || obj == creditsButton) continue;

            var pos = obj.transform.localPosition;
            obj.transform.localPosition = new(pos.x, pos.y - 45, pos.z);
        }

        Plugin.Logger.LogDebug("Removed credits button.");
    }

    static void AlignButtons(IEnumerable<GameObject> buttons) {
        var hostButtonPos = buttons.First(b => b.name == "HostButton")
            .GetComponent<RectTransform>().localPosition;

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

    static GameObject GetButton(Transform panel, string name) {
        try {
            return panel.Find(name).gameObject;
        } catch(Exception e) { 
            if (name != "ModSettingsButton")
                Plugin.Logger.LogError($"Error getting button: {name}\n{e}");

            return null;
        }
    }

    static GameObject FindInParent(GameObject obj, string name) {
        Transform parent = obj.transform.parent;

        try {
            return parent.Find(name).gameObject;
        } catch(Exception e) { 
            Plugin.Logger.LogError($"Error finding '{name}' in: {parent.name}\n{e}");
            return null;
        }
    }
}