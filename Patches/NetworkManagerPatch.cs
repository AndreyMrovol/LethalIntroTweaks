using HarmonyLib;

namespace IntroTweaks.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
internal class NetworkManagerPatch {
    [HarmonyPrefix]
    [HarmonyPatch("Awake")]
    static void SetRealVersion(GameNetworkManager __instance) {
        MenuManagerPatch.realVer = __instance.gameVersionNum;
    }
}