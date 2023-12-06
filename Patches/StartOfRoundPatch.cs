using HarmonyLib;

namespace IntroTweaks.Patches {
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch {
        [HarmonyPrefix]
        [HarmonyPatch("PlayFirstDayShipAnimation")]
        static bool DisableSpeaker() {
            return !Plugin.Config.DISABLE_FIRST_DAY_SFX;
        }
    }
}
