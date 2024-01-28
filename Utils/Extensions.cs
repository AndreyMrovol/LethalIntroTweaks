using UnityEngine;

namespace IntroTweaks.Utils;

internal static class Extensions {
    internal static bool IsAbove(this Transform cur, Transform target) {
        return cur.localPosition.y > target.localPosition.y;
    }

    internal static void ResetAnchoredPos(this RectTransform rect) {
        rect.anchoredPosition = Vector2.zero;
        rect.anchoredPosition3D = Vector3.zero;
    }

    internal static void ResetPivot(this RectTransform rect) {
        rect.pivot = Vector2.zero;
    }

    internal static void ResetSizeDelta(this RectTransform rect) {
        rect.sizeDelta = Vector2.zero;
    }

    internal static void EditOffsets(this RectTransform rect, Vector2 max, Vector2 min) {
        rect.offsetMax = max;
        rect.offsetMin = min;
    }

    internal static void EditAnchors(this RectTransform rect, Vector2 max, Vector2 min) {
        rect.anchorMax = max;
        rect.anchorMin = min;
    }

    internal static void AnchorToBottom(this RectTransform rect) {
        ResetSizeDelta(rect);
        ResetAnchoredPos(rect);

        EditAnchors(rect, new(0.5f, 0), new(0.5f, 0));
        EditOffsets(rect, new(0, 0), new(0, 0));

        rect.localRotation = Quaternion.identity;
        rect.localPosition = new(0, -205, 0);
    }

    internal static void SetLocalX(this RectTransform rect, float newX) {
        rect.localPosition = new(newX, rect.localPosition.y, rect.localPosition.z);
    }

    internal static void FixScale(this Transform transform) {
        transform.localScale = new(1.02f, 1.06f, 1.02f);
    }
}