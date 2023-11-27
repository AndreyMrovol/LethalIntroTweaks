using BepInEx.Configuration;

namespace IntroTweaks.Core {
    public class PluginConfig {
        readonly ConfigFile configFile;

        public bool PLUGIN_ENABLED { get; private set; }

        public PluginConfig(ConfigFile cfg) {
            configFile = cfg;
            PLUGIN_ENABLED = NewEntry("bEnabled", true, "Enable or disable the plugin globally.");
        }

        private T NewEntry<T>(string key, T defaultVal, string description) {
            return configFile.Bind(Metadata.GUID, key, defaultVal, description).Value;
        }

        public void InitBindings() {

        }
    }
}
