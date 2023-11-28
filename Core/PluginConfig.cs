using BepInEx.Configuration;

namespace IntroTweaks.Core {
    public class PluginConfig {
        readonly ConfigFile configFile;

        public bool PLUGIN_ENABLED { get; private set; }

        public string AUTO_SELECT_MODE { get; private set; }
        public bool AUTO_SELECT_HOST { get; private set; }

        public bool SKIP_BOOT_ANIMATION { get; private set; }
        public string VERSION_TEXT { get; private set; }

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

            AUTO_SELECT_HOST = NewEntry("bAutoSelectHost", true,
                "Whether the 'Host' button is automatically selected when the Online/LAN menu loads."
            );

            SKIP_BOOT_ANIMATION = NewEntry("bSkipBootAnimation", true,
                "If the loading animation (booting OS) should be skipped."
            );

            VERSION_TEXT = NewEntry("bVersionText", "v$VERSION\n[MODDED]", 
                "Replace the game's version text with this custom text in the main menu.\n" +
                "To insert the version number, use the $VERSION syntax. E.g. Ver69 would be Ver$VERSION"
            );
        }
    }
}
