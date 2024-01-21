using UnityEngine;

namespace IntroTweaks.Utils;

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
        rect.offsetMax = max;
        rect.offsetMin = min;
    }

    internal static void EditAnchors(RectTransform rect, Vector2 max, Vector2 min) {
        rect.anchorMax = max;
        rect.anchorMin = min;
    }

    internal static void AnchorToBottom(RectTransform rect) {
        ResetSizeDelta(rect);
        ResetAnchoredPos(rect);

        EditAnchors(rect, new(0.5f, 0), new(0.5f, 0));
        EditOffsets(rect, new(0, 0), new(0, 0));

        rect.localRotation = Quaternion.identity;
        rect.localPosition = new(0, -205, 0);
    }

    internal static bool IsAbove(Transform cur, Transform target) {
        return cur.localPosition.y > target.localPosition.y;
    }
}