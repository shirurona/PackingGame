using UnityEngine;

/// <summary>
/// コンテナ（箱）を表現するコンポーネント
/// </summary>
public class BoxContainer : MonoBehaviour
{
    [SerializeField] private Vector3 _size = new Vector3(10f, 10f, 10f);

    /// <summary>
    /// コンテナのサイズ（ローカル空間）
    /// </summary>
    public Vector3 Size
    {
        get => _size;
        set => _size = value;
    }

    /// <summary>
    /// コンテナのワールド空間でのBoundsを取得
    /// </summary>
    public Bounds GetWorldBounds()
    {
        return new Bounds(transform.position, _size);
    }

    /// <summary>
    /// 指定されたBoundsが許容範囲を考慮してコンテナ内に収まっているかを判定
    /// </summary>
    /// <param name="itemBounds">判定対象のBounds</param>
    /// <param name="tolerance">許容範囲（単位: Unity units）</param>
    /// <returns>コンテナ内に収まっている場合true</returns>
    public bool Contains(Bounds itemBounds, float tolerance = 0f)
    {
        Bounds containerBounds = GetWorldBounds();

        // 許容範囲を考慮してコンテナBoundsを拡張
        Bounds expandedBounds = new Bounds(
            containerBounds.center,
            containerBounds.size + Vector3.one * tolerance * 2f
        );

        // アイテムの最小・最大点がすべてコンテナ内に収まっているか確認
        return expandedBounds.Contains(itemBounds.min) && expandedBounds.Contains(itemBounds.max);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Bounds bounds = GetWorldBounds();
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
