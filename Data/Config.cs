using BepInEx.Configuration;

namespace IntroTweaks.Data;

public class Config {
    #region Properties
    #region General
    public ConfigEntry<bool> PLUGIN_ENABLED { get; private set; }
    #endregion

    #region Intro Tweaks
    public ConfigEntry<bool> SKIP_SPLASH_SCREENS { get; private set; }
    public ConfigEntry<bool> SKIP_BOOT_ANIMATION { get; private set; }

    public ConfigEntry<string> AUTO_SELECT_MODE { get; private set; }
    public ConfigEntry<bool> AUTO_SELECT_HOST { get; private set; }
    #endregion

    #region Menu Tweaks
    public ConfigEntry<bool> ALIGN_MENU_BUTTONS { get; private set; }
    public ConfigEntry<bool> FIX_MENU_CANVAS { get; private set; }
    public ConfigEntry<bool> FIX_MENU_PANELS { get; private set; }
    public ConfigEntry<bool> FIX_MORE_COMPANY { get; internal set; }
    //public bool IMPROVE_HOST_SCREEN { get; private set; }

    public ConfigEntry<bool> REMOVE_LAN_WARNING { get; private set; }
    public ConfigEntry<bool> REMOVE_LAUNCHED_IN_LAN { get; private set; }
    public ConfigEntry<bool> REMOVE_NEWS_PANEL { get; private set; }
    public ConfigEntry<bool> REMOVE_CREDITS_BUTTON { get; private set; }
    #endregion

    #region Version Text
    public ConfigEntry<bool> CUSTOM_VERSION_TEXT { get; private set; }
    public ConfigEntry<string> VERSION_TEXT { get; private set; }
    public ConfigEntry<float> VERSION_TEXT_SIZE { get; private set; }
    public ConfigEntry<float> VERSION_TEXT_OFFSET { get; private set; }
    public ConfigEntry<bool> ALWAYS_SHORT_VERSION { get; private set; }
    #endregion

    #region Misc
    public ConfigEntry<bool> AUTO_START_GAME { get; private set; }
    public ConfigEntry<bool> DISABLE_FIRST_DAY_SFX { get; private set; }
    public ConfigEntry<int> GAME_STARTUP_DISPLAY { get; private set; }
    #endregion

    readonly ConfigFile configFile;
    #endregion

    public Config(ConfigFile cfg) {
        configFile = cfg;

        PLUGIN_ENABLED = NewEntry("bEnabled", true, "Enable or disable the plugin globally.");

        SKIP_SPLASH_SCREENS = NewEntry(Category.INTRO_TWEAKS, "bSkipSplashScreens", true,
            "Skips those pesky Unity and Zeekers startup logos!"
        );
    }

    private ConfigEntry<T> NewEntry<T>(string key, T defaultVal, string desc) =>
        NewEntry(Category.GENERAL, key, defaultVal, desc);

    private ConfigEntry<T> NewEntry<T>(Category category, string key, T defaultVal, string desc) =>
        configFile.Bind(category.Value, key, defaultVal, desc);

    public void InitBindings() {
        #region Options related to the intro.
        SKIP_BOOT_ANIMATION = NewEntry(Category.INTRO_TWEAKS, "bSkipBootAnimation", true,
            "If the loading animation (booting OS) should be skipped."
        );

        AUTO_SELECT_MODE = NewEntry(Category.INTRO_TWEAKS, "sAutoSelectMode", "OFF",
            "Which mode to automatically enter into after the splash screen.\n" +
            "Valid options: ONLINE, LAN, OFF"
        );

        AUTO_SELECT_HOST = NewEntry(Category.INTRO_TWEAKS, "bAutoSelectHost", false,
            "Whether the 'Host' button is automatically selected when the Online/LAN menu loads."
        );
        #endregion

        #region Tweaks to the main menu
        ALIGN_MENU_BUTTONS = NewEntry(Category.MENU_TWEAKS, "bAlignMenuButtons", true,
            "If the main menu buttons should align with each other."
        );

        FIX_MENU_CANVAS = NewEntry(Category.MENU_TWEAKS, "bFixMenuCanvas", false,
            "Whether the main menu canvas should have its settings corrected.\n" +
            "May cause overlapping issues, only enable it if you don't use other mods that edit the menu."
        );

        FIX_MENU_PANELS = NewEntry(Category.MENU_TWEAKS, "bFixMenuPanels", false,
            "The main menu panels (host, servers, loading screen) all have anchoring, offset and sizing issues.\n" +
            "This option helps solve them and improve the look of the menu.\n\nMAY BREAK SOME MODS."
        );

        FIX_MORE_COMPANY = NewEntry(Category.MENU_TWEAKS, "bFixMoreCompany", true,
            "Whether to apply fixes to MoreCompany UI elements.\n" +
            "Fixes include: button placement, header positioning & scaling of cosmetics border.\n\n" +
            "PRONE TO INCOMPATIBILITIES! TURN THIS OFF IF YOU ENCOUNTER BREAKING BUGS."
        );

        //IMPROVE_HOST_SCREEN = NewEntry(Category.MENU_TWEAKS, "bImproveHostScreen", true,
        //    "Should improvements be made to the host screen?"
        //);

        REMOVE_LAN_WARNING = NewEntry(Category.MENU_TWEAKS, "bRemoveLanWarning", true,
            "Hides the warning popup when hosting a LAN session."
        );

        REMOVE_LAUNCHED_IN_LAN = NewEntry(Category.MENU_TWEAKS, "bRemoveLaunchedInLanText", true,
            "Hides the 'Launched in LAN mode' text below the Quit button."
        );

        REMOVE_NEWS_PANEL = NewEntry(Category.MENU_TWEAKS, "bRemoveNewsPanel", false,
            "Hides the panel that displays news such as game updates."
        );

        REMOVE_CREDITS_BUTTON = NewEntry(Category.MENU_TWEAKS, "bRemoveCreditsButton", true,
            "Hides the 'Credits' button on the main menu. The other buttons are automatically adjusted."
        );
        #endregion

        #region Options to control custom version text
        CUSTOM_VERSION_TEXT = NewEntry(Category.VERSION_TEXT, "bCustomVersionText", true,
            "Whether to replace the game's version text with a custom alternative."
        );

        VERSION_TEXT = NewEntry(Category.VERSION_TEXT, "sVersionText", "v$VERSION\n[MODDED]",
            "Replace the game's version text with this custom text in the main menu.\n" +
            "To insert the version number, use the $VERSION syntax. E.g. Ver69 would be Ver$VERSION"
        );

        VERSION_TEXT_SIZE = NewEntry(Category.VERSION_TEXT, "fVersionTextSize", 20f,
            "The font size of the version text. Min = 10, Max = 40."
        );

        VERSION_TEXT_OFFSET = NewEntry(Category.VERSION_TEXT, "fVersionTextOffset", 0f,
            "Use this option to adjust the Y position of the version text if it's out of place.\n" +
            "For example, when using 3 lines of text, a small positive value would move it back up."
        );

        ALWAYS_SHORT_VERSION = NewEntry(Category.VERSION_TEXT, "bAlwaysShortVersion", true,
            "If the custom version text should always show the short 'real' version.\n" +
            "This will ignore mods like LC_API and MoreCompany that change the game version."
        );
        #endregion

        #region Misc options
        AUTO_START_GAME = NewEntry(Category.MISC, "bAutoStartGame", false, 
            "If enabled, the lever will be pulled automatically to begin the landing sequence."
        );

        DISABLE_FIRST_DAY_SFX = NewEntry(Category.MISC, "bDisableFirstDaySFX", false,
            "Toggles the first day ship speaker SFX."
        );

        GAME_STARTUP_DISPLAY = NewEntry(Category.MISC, "iGameStartupDisplay", 0,
            "The index of the monitor to display the game on when starting.\n" +
            "You can find these indexes in your Windows display settings.\n" +
            "Defaults to 0 (main monitor)."
        );
        #endregion
    }
}