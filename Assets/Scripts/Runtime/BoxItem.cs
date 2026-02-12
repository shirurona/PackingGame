using UnityEngine;

/// <summary>
/// 直方体アイテムを表現するコンポーネント
/// </summary>
public class BoxItem : MonoBehaviour
{
    [SerializeField] private Vector3 _size = Vector3.one;

    /// <summary>
    /// アイテムのサイズ（ローカル空間）
    /// </summary>
    public Vector3 Size
    {
        get => _size;
        set => _size = value;
    }

    /// <summary>
    /// 回転を考慮したワールド空間でのBoundsを取得
    /// </summary>
    public Bounds GetWorldBounds()
    {
        // 8つの角の座標をローカル空間で計算
        Vector3 halfSize = _size * 0.5f;
        Vector3[] localCorners = new Vector3[8]
        {
            new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
            new Vector3( halfSize.x, -halfSize.y, -halfSize.z),
            new Vector3(-halfSize.x,  halfSize.y, -halfSize.z),
            new Vector3( halfSize.x,  halfSize.y, -halfSize.z),
            new Vector3(-halfSize.x, -halfSize.y,  halfSize.z),
            new Vector3( halfSize.x, -halfSize.y,  halfSize.z),
            new Vector3(-halfSize.x,  halfSize.y,  halfSize.z),
            new Vector3( halfSize.x,  halfSize.y,  halfSize.z)
        };

        // ワールド空間に変換
        Vector3[] worldCorners = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            worldCorners[i] = transform.TransformPoint(localCorners[i]);
        }

        // ワールド空間での最小・最大を求める
        Vector3 min = worldCorners[0];
        Vector3 max = worldCorners[0];
        for (int i = 1; i < 8; i++)
        {
            min = Vector3.Min(min, worldCorners[i]);
            max = Vector3.Max(max, worldCorners[i]);
        }

        // Boundsを作成
        Vector3 center = (min + max) * 0.5f;
        Vector3 size = max - min;
        return new Bounds(center, size);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Bounds bounds = GetWorldBounds();
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
