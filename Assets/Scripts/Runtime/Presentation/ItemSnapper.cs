using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ドラッグ中アイテムの壁・設置済みアイテムへのスナップ計算。
/// </summary>
public static class ItemSnapper
{
    /// <summary>
    /// アイテム中心位置にスナップを適用して返す。
    /// centerはビジュアル座標（箱の中心が原点）。
    /// </summary>
    public static Vector3 Snap(
        Vector3 center, Vector3 itemSize,
        Vector3 boxSize, IReadOnlyList<PlacedItem> placedItems,
        float threshold)
    {
        var halfItem = itemSize * 0.5f;
        // 論理座標→ビジュアル座標の変換オフセット
        var boxOffset = -boxSize * 0.5f;

        center.x = SnapAxis(center.x, halfItem.x, boxSize.x, boxOffset.x, placedItems, 0, threshold);
        center.y = SnapAxis(center.y, halfItem.y, boxSize.y, boxOffset.y, placedItems, 1, threshold);
        center.z = SnapAxis(center.z, halfItem.z, boxSize.z, boxOffset.z, placedItems, 2, threshold);

        return center;
    }

    static float SnapAxis(
        float centerPos, float halfSize,
        float boxAxisSize, float boxOffset,
        IReadOnlyList<PlacedItem> placedItems,
        int axis, float threshold)
    {
        float itemMin = centerPos - halfSize;
        float itemMax = centerPos + halfSize;

        float bestDist = threshold;
        float bestShift = 0f;

        // 箱の壁（ビジュアル座標: -boxAxisSize/2 〜 +boxAxisSize/2）
        float wallMin = -boxAxisSize * 0.5f;
        float wallMax = boxAxisSize * 0.5f;
        CheckFace(itemMin, wallMin, ref bestDist, ref bestShift);
        CheckFace(itemMax, wallMax, ref bestDist, ref bestShift);

        // 設置済みアイテムの面（論理座標をビジュアル座標に変換）
        for (int i = 0; i < placedItems.Count; i++)
        {
            var bounds = placedItems[i].GetBounds();
            float otherMin = GetAxis(bounds.min, axis) + boxOffset;
            float otherMax = GetAxis(bounds.max, axis) + boxOffset;

            CheckFace(itemMax, otherMin, ref bestDist, ref bestShift);
            CheckFace(itemMin, otherMax, ref bestDist, ref bestShift);
        }

        return centerPos + bestShift;
    }

    static void CheckFace(float itemFace, float targetFace, ref float bestDist, ref float bestShift)
    {
        float dist = Mathf.Abs(itemFace - targetFace);
        if (dist >= bestDist) return;

        bestDist = dist;
        bestShift = targetFace - itemFace;
    }

    static float GetAxis(Vector3 v, int axis) => axis switch
    {
        0 => v.x,
        1 => v.y,
        _ => v.z,
    };
}
