using System;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

using static UnityEngine.Rendering.SplashScreen;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

using IntroTweaks.Core;
using BepInEx.Bootstrap;
using System.Linq;
using IntroTweaks.Patches;

namespace IntroTweaks;

[BepInPlugin(Metadata.GUID, Metadata.NAME, Metadata.VERSION)]
public class Plugin : BaseUnityPlugin {
    internal static new ManualLogSource Logger { get; private set; }
    public static new PluginConfig Config { get; private set; }

    internal static string SelectedMode;

    private Harmony patcher;

    static bool menuLoaded = false;

    public static bool ModInstalled(string name) {
        return Chainloader.PluginInfos.Keys.Any(p => p.Contains(name));
    }

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

        // Not really a 'real' skip, but good enough for the time being.
        while (!menuLoaded) {
            SplashScreen.Stop(StopBehavior.StopImmediate);
        };
    }

    private void SceneLoaded(Scene scene, LoadSceneMode _) {
        switch(scene.name) {
            case "InitScene":
            case "InitSceneLaunchOptions":
            case "MainMenu": {
                menuLoaded = true;
                break;
            }
        }
    }
}