using HarmonyLib;
using IntroTweaks.Data;
using IntroTweaks.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace IntroTweaks.Patches;

[HarmonyPatch(typeof(MenuManager))]
internal class MenuManagerPatch {
    public static int realVer { get; internal set; }
    public static int gameVer { get; private set; }

    public static TextMeshProUGUI versionText { get; private set; }
    public static RectTransform versionTextRect { get; private set; }

    internal static GameObject VersionNum = null;
    internal static Transform MenuContainer = null;
    internal static Transform MenuPanel = null;

    public static Color32 DARK_ORANGE = new(175, 115, 0, 255);

    static MenuManager Instance;
    static Config Cfg => Plugin.Config;

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
        Cfg.ALWAYS_SHORT_VERSION.SettingChanged += (object s, EventArgs e) =>
            SetVersion();

        Cfg.VERSION_TEXT_SIZE.SettingChanged += (object s, EventArgs e) =>
            versionText.fontSize = Cfg.VERSION_TEXT_SIZE.ClampedValue(10, 40);

        Cfg.VERSION_TEXT_OFFSET.SettingChanged += (object s, EventArgs e) => 
            versionTextRect.RefreshPosition();

        try {
            if (Cfg.FIX_MENU_PANELS.Value) {
                // Make the white space equal on both sides of the panel.
                FixPanelAlignment(MenuPanel);

                FixPanelAlignment(MenuContainer.Find("LobbyHostSettings"));
                FixPanelAlignment(MenuContainer.Find("LobbyList"));
                FixPanelAlignment(MenuContainer.Find("LoadingScreen"));

                Plugin.Logger.LogDebug("Fixed menu panel alignment.");
            }

            #region Remove credits buttons and align others.
            IEnumerable<GameObject> buttons = MenuPanel
                .GetComponentsInChildren<Button>(true)
                .Select(b => b.gameObject);

            if (Cfg.ALIGN_MENU_BUTTONS.Value) {
                AlignButtons(buttons);
            }

            if (Cfg.REMOVE_CREDITS_BUTTON.Value) {
                RemoveCreditsButton(buttons);
            }
            #endregion

            #region Handle MoreCompany/AdvancedCompany.
            bool fixCanvas = Cfg.FIX_MENU_CANVAS.Value;

            bool acInstalled = Plugin.ModInstalled("AdvancedCompany");
            bool mcInstalled = Plugin.ModInstalled("MoreCompany");

            if (acInstalled || mcInstalled) {
                fixCanvas = false;
            }

            if (Cfg.FIX_MORE_COMPANY.Value) {
                if (mcInstalled && !acInstalled) {
                    bool fixedMC = FixMoreCompany();
                    string debugStr = fixedMC ? ". Edits have been made to its UI elements." : " but its UI elements do not exist!";
                    Plugin.Logger.LogDebug("MoreCompany found" + debugStr);
                }
            }
            #endregion

            TweakCanvasSettings(Instance.menuButtons, fixCanvas);
        }
        catch (Exception e) {
            Plugin.Logger.LogError($"An error occurred patching the menu. SAJ.\n{e}");
        }

        #region Hide UI elements
        if (Cfg.REMOVE_NEWS_PANEL.Value) {
            Instance.NewsPanel?.SetActive(false);
        }

        if (Cfg.REMOVE_LAN_WARNING.Value) {
            Instance.lanWarningContainer?.SetActive(false);
        }

        if (Cfg.REMOVE_LAUNCHED_IN_LAN.Value) {
            GameObject lanModeText = Instance.launchedInLanModeText?.gameObject;
            if (lanModeText) {
                lanModeText.SetActive(false);
            }
        }
        #endregion

        if (Cfg.AUTO_SELECT_HOST.Value) {
            Instance.ClickHostButton();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch("Update")]
    static void UpdatePatch(MenuManager __instance) {
        bool onMenu = __instance.menuButtons.activeSelf;

        // Create the new game version text if not made already.
        if (versionText == null) TryReplaceVersionText();
        else {
            // Make sure the text is correct.
            versionText.text = Cfg.VERSION_TEXT.Value.Replace("$VERSION", $"{gameVer}");

            var textObj = versionText.gameObject;
            if (!textObj.activeSelf && onMenu) {
                textObj.SetActive(true);
            }
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch("ClickHostButton")]
    static void DisableMenuOnHost() {
        MenuPanel?.gameObject.SetActive(false);
    }

    static bool FixMoreCompany() {
        #region Grab references to MC UI elements.
        GameObject mc = GameObject.Find("GlobalScale");
        if (!mc) return false;

        GameObject cosmetics = mc.transform.Find("CosmeticsScreen").gameObject;

        mc.GetComponentInParent<Canvas>().pixelPerfect = true;
        #endregion

        #region Tweak character spin area.
        Transform spinArea = cosmetics.transform.Find("SpinAreaButton").transform;
        spinArea.localScale = new(0.48f, 0.55f, 0.46f);
        spinArea.position = new(421.65f, 245.7f, 200f);
        #endregion

        #region Button pos and bring to front.
        var activate = cosmetics.FindInParent("ActivateButton").GetComponent<RectTransform>();
        var exit = cosmetics.transform.Find("ExitButton").GetComponent<RectTransform>();

        activate.AnchorToBottomRight();
        exit.AnchorToBottomRight();
        exit.SetAsLastSibling();
        #endregion

        #region Change cosmetics border scale.
        Transform border = cosmetics.transform.Find("CosmeticsHolderBorder").transform;
        border.localScale = new(2.4f, 2.1f, 1);
        #endregion

        #region Header scale & position.
        Transform header = Instance.menuButtons.transform.Find("HeaderImage").transform;
        header.localScale = new(4.9f, 4.9f, 4.9f);
        header.localPosition = new(header.localPosition.x, header.localPosition.y + 35, 0);
        #endregion

        return mc != null;
    }

    static void RemoveCreditsButton(IEnumerable<GameObject> buttons) {
        GameObject quitButton = buttons.First(b => b.name == "QuitButton");
        GameObject creditsButton = buttons.First(b => b.name == "Credits");

        // Disable credits button and move quit button there instead.
        creditsButton.SetActive(false);

        // Move all buttons before the credits button down.
        var creditsRect = creditsButton.GetComponent<RectTransform>();
        var creditsHeight = creditsRect.rect.height * 1.3f;

        buttons.Do(obj => {
            if (!obj || obj == creditsButton) return;

            var cur = obj.transform;
            var pos = cur.localPosition;

            if (obj.transform.IsAbove(creditsRect)) {
                cur.localPosition = new(pos.x, pos.y - creditsHeight, pos.z);
            }
        });

        Plugin.Logger.LogDebug("Removed credits button.");
    }

    static void AlignButtons(IEnumerable<GameObject> buttons) {
        RectTransform hostButton = buttons
            .First(b => b.name == "HostButton")
            .GetComponent<RectTransform>();

        hostButton.SetLocalX(hostButton.localPosition.x + 15);

        foreach (GameObject obj in buttons) {
            if (!obj) {
                Plugin.Logger.LogDebug($"Could not align button {obj.name}");
                continue;
            }

            #region Move right slightly.
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = hostButton.sizeDelta;
            rect.localPosition = new(hostButton.localPosition.x, rect.localPosition.y, hostButton.localPosition.z);
            #endregion

            #region Fix text mesh settings
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>(true);

            text.transform.FixScale();
            TweakTextSettings(text);

            text.fontSize = 15;
            text.wordSpacing -= 25;
            #endregion

            #region Fix button text rect
            var textRect = text.gameObject.GetComponent<RectTransform>();
            textRect.ResetAnchoredPos();
            textRect.EditOffsets(Vector2.zero, new(5, 0));
            #endregion
        }

        Plugin.Logger.LogDebug("Aligned menu buttons.");
    }

    internal static void TryReplaceVersionText() {
        if (!Cfg.CUSTOM_VERSION_TEXT.Value) return;
        if (VersionNum == null || MenuPanel == null) return;

        GameObject clone = Object.Instantiate(VersionNum, MenuPanel);
        clone.name = "VersionNumberText";

        versionText = InitTextMesh(clone.GetComponent<TextMeshProUGUI>());
        versionTextRect = versionText.gameObject.GetComponent<RectTransform>();
        versionTextRect.AnchorToBottom();

        VersionNum.SetActive(false);
    }

    static void SetVersion() {
        bool alwaysShort = Cfg.ALWAYS_SHORT_VERSION.Value;
        int curVer = Math.Abs(GameNetworkManager.Instance.gameVersionNum);
        gameVer = alwaysShort ? realVer : (curVer != realVer ? curVer : realVer);
    }

    static TextMeshProUGUI InitTextMesh(TextMeshProUGUI tmp) {
        SetVersion();

        tmp.text = Cfg.VERSION_TEXT.Value;
        tmp.fontSize = Cfg.VERSION_TEXT_SIZE.ClampedValue(10, 40);
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
        // More future-proof than `panel.transform.parent.parent`.
        var canvas = panel.GetComponentInParent<Canvas>();
        canvas.pixelPerfect = true;

        if (changeRenderMode) {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    static void FixPanelAlignment(Transform panel) {
        var rect = panel.gameObject.GetComponent<RectTransform>();

        rect.ResetSizeDelta();
        rect.ResetAnchoredPos();
        rect.EditOffsets(new(-20, -25), new(20, 25));

        panel.FixScale();
    }
}