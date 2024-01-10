using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace IntroTweaks.Patches {
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch {
        [HarmonyPostfix]
        [HarmonyPatch("firstDayAnimation")]
        static IEnumerator DisableFirstDaySFX(IEnumerator result, StartOfRound __instance) {
            // Run original.
            while (result.MoveNext()) {
                yield return result.Current;
            }

            if (Plugin.Config.DISABLE_FIRST_DAY_SFX) {
                StopSpeaker(__instance.speakerAudioSource);
            }
        }

        static void StopSpeaker(AudioSource source) {
            if (source.isPlaying) {
                source.Stop();
            }
        }
    }
}
