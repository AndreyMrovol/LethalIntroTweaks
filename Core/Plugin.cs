using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using IntroTweaks.Core;
using UnityEngine.Rendering;

using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.SplashScreen;
using UnityEngine.SceneManagement;

namespace IntroTweaks;

[BepInPlugin(Metadata.GUID, Metadata.NAME, Metadata.VERSION)]
public class Plugin : BaseUnityPlugin {
    internal static new ManualLogSource Logger { get; private set; }
    public static new PluginConfig Config { get; private set; }

    internal static string SelectedMode;

    private Harmony patcher;

    static bool menuLoaded = false;

    private void Awake() {
        Logger = base.Logger;
        Config = new(base.Config);

        if (!PluginEnabled(logDisabled: true)) return;

        SceneManager.sceneLoaded += SceneLoaded;

        if (Config.SKIP_SPLASH_SCREENS) {
            Task.Run(SkipSplashScreen);
        }

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

        do {
            // Not really a 'real' skip, but good enough for the time being.
            SplashScreen.Stop(StopBehavior.StopImmediate);
        } while (!menuLoaded && Time.realtimeSinceStartup < 8);
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode) {
        switch(scene.name) {
            case "MainMenu": {
                menuLoaded = true;
                break;
            }
        }
    }
}