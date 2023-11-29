using BepInEx.Configuration;

namespace IntroTweaks.Core {
    public class PluginConfig {
        readonly ConfigFile configFile;

        public bool PLUGIN_ENABLED { get; private set; }

        public string AUTO_SELECT_MODE { get; private set; }
        public bool AUTO_SELECT_HOST { get; private set; }

        public bool SKIP_BOOT_ANIMATION { get; private set; }

        public bool REMOVE_LAN_WARNING { get; private set; }
        public bool REMOVE_LAUNCHED_IN_LAN { get; private set; }
        public bool REMOVE_NEWS_PANEL { get; private set; }

        public string VERSION_TEXT { get; private set; }
        public float VERSION_TEXT_X { get; private set; }
        public float VERSION_TEXT_Y { get; private set; }

        public PluginConfig(ConfigFile cfg) {
            configFile = cfg;
            PLUGIN_ENABLED = NewEntry("bEnabled", true, "Enable or disable the plugin globally.");
        }

        private T NewEntry<T>(string key, T defaultVal, string description) {
            return configFile.Bind(Metadata.GUID, key, defaultVal, description).Value;
        }

        public void InitBindings() {
            AUTO_SELECT_MODE = NewEntry("sAutoSelectMode", "ONLINE",
                "Which mode to automatically enter into after the splash screen.\n" +
                "Valid options: ONLINE, LAN, OFF"
            );

            AUTO_SELECT_HOST = NewEntry("bAutoSelectHost", false,
                "Whether the 'Host' button is automatically selected when the Online/LAN menu loads."
            );

            SKIP_BOOT_ANIMATION = NewEntry("bSkipBootAnimation", true,
                "If the loading animation (booting OS) should be skipped."
            );

            REMOVE_LAN_WARNING = NewEntry("bRemoveLanWarning", true, "Remove the warning popup when hosting a LAN session.");
            REMOVE_LAUNCHED_IN_LAN = NewEntry("bRemoveLaunchedInLanText", true, "Remove the 'Launched in LAN mode' text below the Quit button.");
            REMOVE_NEWS_PANEL = NewEntry("bRemoveNewsPanel", false, "Remove the panel that displays news such as game updates.");

            VERSION_TEXT = NewEntry("sVersionText", "v$VERSION\n[MODDED]", 
                "Replace the game's version text with this custom text in the main menu.\n" +
                "To insert the version number, use the $VERSION syntax. E.g. Ver69 would be Ver$VERSION"
            );

            VERSION_TEXT_X = NewEntry("fVersionTextXPos", 1089.9f,
                "The position on the horizontal axis where the version text should be placed.\n" +
                "Positive = Right, Negative = Left"
            );

            VERSION_TEXT_Y = NewEntry("fVersionTextYPos", 553f,
                "The position on the vertical axis where the version text should be placed.\n" +
                "Positive = Up, Negative = Down"
            );
        }
    }
}
