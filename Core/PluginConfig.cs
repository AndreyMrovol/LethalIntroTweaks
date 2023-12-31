using BepInEx.Configuration;
using System;

namespace IntroTweaks.Core {
    public class PluginConfig {
        public struct Category {
            public static Category GENERAL => new("0 >> General << 0");
            public static Category INTRO_TWEAKS => new("1 >> Intro << 1");
            public static Category MENU_TWEAKS => new("2 >> Main Menu << 2");
            public static Category VERSION_TEXT => new("3 >> Custom Version Text << 3");
            public static Category MISC => new("4 >> Miscellaneous << 4");

            public string Value { get; private set; }

            private Category(string value) {
                Value = value;
            }
        }

        public bool PLUGIN_ENABLED { get; private set; }

        public bool SKIP_SPLASH_SCREENS { get; private set; }
        public bool SKIP_BOOT_ANIMATION { get; private set; }

        public string AUTO_SELECT_MODE { get; private set; }
        public bool AUTO_SELECT_HOST { get; private set; }

        public bool ALIGN_MENU_BUTTONS { get; private set; }
        public bool FIX_MENU_CANVAS { get; private set; }
        public bool FIX_MENU_PANELS { get; private set; }

        public bool REMOVE_LAN_WARNING { get; private set; }
        public bool REMOVE_LAUNCHED_IN_LAN { get; private set; }
        public bool REMOVE_NEWS_PANEL { get; private set; }
        public bool REMOVE_CREDITS_BUTTON { get; private set; }

        public bool CUSTOM_VERSION_TEXT { get; private set; }
        public string VERSION_TEXT { get; private set; }
        public string VERSION_TEXT_FORMAT { get; private set; }
        public float VERSION_TEXT_SIZE { get; private set; }
        public float VERSION_TEXT_X { get; private set; }
        public float VERSION_TEXT_Y { get; private set; }

        public bool DISABLE_FIRST_DAY_SFX { get; private set; }
        public int GAME_STARTUP_DISPLAY { get; private set; }

        [NonSerialized]
        readonly ConfigFile configFile;

        public PluginConfig(ConfigFile cfg) {
            configFile = cfg;
            PLUGIN_ENABLED = NewEntry("bEnabled", true, "Enable or disable the plugin globally.");

            SKIP_SPLASH_SCREENS = NewEntry(Category.INTRO_TWEAKS, "bSkipSplashScreens", true,
                "Skips those pesky Unity and Zeekers startup logos!"
            );
        }

        private T NewEntry<T>(string key, T defaultVal, string desc) {
            return NewEntry(Category.GENERAL, key, defaultVal, desc);
        }

        private T NewEntry<T>(Category category, string key, T defaultVal, string desc) {
            return configFile.Bind(category.Value, key, defaultVal, desc).Value;
        }

        public void InitBindings() {
            #region Options related to the intro.
            SKIP_BOOT_ANIMATION = NewEntry(Category.INTRO_TWEAKS, "bSkipBootAnimation", true,
                "If the loading animation (booting OS) should be skipped."
            );

            AUTO_SELECT_MODE = NewEntry(Category.INTRO_TWEAKS, "sAutoSelectMode", "ONLINE",
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

            FIX_MENU_CANVAS = NewEntry(Category.MENU_TWEAKS, "bFixMenuCanvas", true, 
                "Whether the main menu canvas should have its settings corrected.\n" + 
                "May cause overlapping issues, only turn it on if you aren't using other menu mods."
            );

            FIX_MENU_PANELS = NewEntry(Category.MENU_TWEAKS, "bFixMenuPanels", true, 
                "The main menu panels (host, servers, loading screen) all have anchoring, offset and sizing issues.\n" +
                "This option helps solve them and improve the look of the menu.\n\nMAY BREAK SOME MODS."
            );

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

            VERSION_TEXT_FORMAT = NewEntry(Category.VERSION_TEXT, "sVersionTextFormat", "FULL",
                "Determines how to display game version number.\n" +
                "Valid options: FULL, SHORT"
            );

            VERSION_TEXT_SIZE = NewEntry(Category.VERSION_TEXT, "fVersionTextSize", 20f, 
                "The font size of the version text. Min = 10, Max = 40."
            );

            VERSION_TEXT_X = NewEntry(Category.VERSION_TEXT, "fVersionTextXPos", 1089.86f,
                "The position on the horizontal axis where the version text should be placed.\n" +
                "Positive = Right, Negative = Left"
            );

            VERSION_TEXT_Y = NewEntry(Category.VERSION_TEXT, "fVersionTextYPos", 555f,
                "The position on the vertical axis where the version text should be placed.\n" +
                "Positive = Up, Negative = Down"
            );
            #endregion

            #region Misc options
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
}