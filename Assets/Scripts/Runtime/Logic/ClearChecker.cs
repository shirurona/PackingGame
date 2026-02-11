using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全アイテムが箱内に収まり重なりがないか判定する。
/// </summary>
public static class ClearChecker
{
    public static bool IsStageClear(
        Vector3 boxSize,
        List<PlacedItem> placedItems,
        int totalItemCount,
        float tolerance = 0.01f)
    {
        // 未配置アイテムがあればクリアではない
        if (placedItems.Count != totalItemCount) return false;

        var boxBounds = new Bounds(boxSize * 0.5f, boxSize);
        
        for (int i = 0; i < placedItems.Count; i++)
        {
            var bounds = placedItems[i].GetBounds();

            // 箱からはみ出していないか
            if (bounds.min.x < boxBounds.min.x - tolerance ||
                bounds.min.y < boxBounds.min.y - tolerance ||
                bounds.min.z < boxBounds.min.z - tolerance ||
                bounds.max.x > boxBounds.max.x + tolerance ||
                bounds.max.y > boxBounds.max.y + tolerance ||
                bounds.max.z > boxBounds.max.z + tolerance)
                return false;

            // 他アイテムと重なっていないか
            for (int j = i + 1; j < placedItems.Count; j++)
            {
                var other = placedItems[j].GetBounds();
                if (OverlapVolume(bounds, other) > tolerance)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 2つのBoundsの重なり体積を返す。重なりがなければ0。
    /// </summary>
    static float OverlapVolume(Bounds a, Bounds b)
    {
        float ox = Mathf.Max(0, Mathf.Min(a.max.x, b.max.x) - Mathf.Max(a.min.x, b.min.x));
        float oy = Mathf.Max(0, Mathf.Min(a.max.y, b.max.y) - Mathf.Max(a.min.y, b.min.y));
        float oz = Mathf.Max(0, Mathf.Min(a.max.z, b.max.z) - Mathf.Max(a.min.z, b.min.z));
        return ox * oy * oz;
    }
}
