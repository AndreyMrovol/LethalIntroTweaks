using UnityEngine;

namespace IntroTweaks.Utils {
    internal class RectUtil {
        internal static void ResetAnchoredPos(RectTransform rect) {
            rect.anchoredPosition = Vector2.zero;
            rect.anchoredPosition3D = Vector3.zero;
        }

        internal static void ResetPivot(RectTransform rect) {
             rect.pivot = Vector2.zero;
        }

        internal static void ResetSizeDelta(RectTransform rect) {
            rect.sizeDelta = Vector2.zero;
        }

        internal static void EditOffsets(RectTransform rect, Vector2 max, Vector2 min) {
            rect.offsetMax = max != null ? max : Vector2.zero;
            rect.offsetMin = min != null ? min : Vector2.zero;
        }

        internal static void EditAnchors(RectTransform rect, Vector2 max, Vector2 min) {
            rect.anchorMax = max != null ? max : Vector2.zero;
            rect.anchorMin = min != null ? min : Vector2.zero;
        }

        internal static void AnchorToBottom(RectTransform rect) {
            EditAnchors(rect, new(0.5f, 0), new(0.5f, 0));
            ResetAnchoredPos(rect);

            rect.rotation = Quaternion.identity;
            rect.position = new Vector3(
                Plugin.Config.VERSION_TEXT_X, 
                Plugin.Config.VERSION_TEXT_Y, 
                -37
            );
        }
    }
}
