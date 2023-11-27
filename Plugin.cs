using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using IntroTweaks.Core;

namespace IntroTweaks {
    [BepInPlugin(Metadata.GUID, Metadata.NAME, Metadata.VERSION)]
    public class Plugin : BaseUnityPlugin {
        internal static new ManualLogSource Logger { get; private set; }
        public static new PluginConfig Config { get; private set; }

        internal static string SelectedMode { get; private set; }

        private Harmony patcher;

        private void Awake() {
            Logger = base.Logger;
            Config = new(base.Config);

            if (!PluginEnabled(logIfDisabled: true)) return;

            Config.InitBindings();
            SelectedMode = Config.AUTO_SELECT_MODE.ToLower();

            try {
                patcher = new(Metadata.GUID);
                patcher.PatchAll();

                Logger.LogInfo("Plugin loaded.");
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }

        public bool PluginEnabled(bool logIfDisabled = false) {
            bool enabled = Config.PLUGIN_ENABLED;
            if (!enabled && logIfDisabled) {
                Logger.LogInfo("IntroTweaks disabled globally.");
            }

            return enabled;
        }
    }
}