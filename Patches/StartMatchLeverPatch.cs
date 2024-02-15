using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace IntroTweaks.Patches;

[HarmonyPatch(typeof(StartMatchLever))]
internal class StartMatchLeverPatch {
    [HarmonyPostfix]
	[HarmonyPatch("Start")]
	public static void StartMatch(StartMatchLever __instance) {
        bool autoStart = Plugin.Config.AUTO_START_GAME.Value;

		if (autoStart && !__instance.leverHasBeenPulled) {
			__instance.StartCoroutine(PullLeverAnim(__instance));
		}
	}

    static IEnumerator PullLeverAnim(StartMatchLever instance) {
        yield return new WaitForSeconds(1.5f);

        instance.leverAnimatorObject.SetBool("pullLever", true);
        instance.leverHasBeenPulled = true;
        instance.triggerScript.interactable = false;

        instance.PullLever();
    }
}