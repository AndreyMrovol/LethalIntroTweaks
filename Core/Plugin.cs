using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using IntroTweaks.Core;
using UnityEngine.Rendering;

using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.SplashScreen;

namespace IntroTweaks {
    [BepInPlugin(Metadata.GUID, Metadata.NAME, Metadata.VERSION)]
    public class Plugin : BaseUnityPlugin {
        internal static new ManualLogSource Logger { get; private set; }
        public static new PluginConfig Config { get; private set; }

        internal static string SelectedMode;

        private Harmony patcher;

        private void Awake() {
            Logger = base.Logger;
            Config = new(base.Config);

            if (!PluginEnabled(logDisabled: true)) return;

            if (Config.SKIP_SPLASH_SCREENS) 
                SkipSplashScreen();

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

        public bool PluginEnabled(bool logDisabled = false) {
            bool enabled = Config.PLUGIN_ENABLED;
            if (!enabled && logDisabled) {
                Logger.LogInfo("IntroTweaks disabled globally.");
            }

            return enabled;
        }

        void SkipSplashScreen() {
            Logger.LogDebug("Skipping splash screens. Ew.");
            Task.Factory.StartNew(() => {
                do {
                    SplashScreen.Stop(StopBehavior.StopImmediate);
                } while (Time.realtimeSinceStartup < 10);
            });
        }
    }
}