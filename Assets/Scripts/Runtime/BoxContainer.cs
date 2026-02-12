using UnityEngine;

namespace PackingGame
{
    /// <summary>
    /// コンテナ（箱）を表現するコンポーネント
    /// </summary>
    public class BoxContainer : MonoBehaviour
    {
        [SerializeField] private Vector3 _size = new Vector3(10f, 10f, 10f);

        /// <summary>
        /// コンテナのサイズ
        /// </summary>
        public Vector3 Size
        {
            get => _size;
            set => _size = value;
        }

        /// <summary>
        /// コンテナのワールド空間でのBounds
        /// </summary>
        public Bounds GetWorldBounds()
        {
            return new Bounds(transform.position, _size);
        }

        /// <summary>
        /// 指定されたBoundsが許容範囲を考慮してコンテナ内に完全に収まっているかを判定
        /// </summary>
        /// <param name="bounds">判定対象のBounds</param>
        /// <param name="tolerance">許容範囲（単位: Unity units）</param>
        /// <returns>完全に収まっていればtrue</returns>
        public bool Contains(Bounds bounds, float tolerance)
        {
            Bounds containerBounds = GetWorldBounds();

            // 許容範囲を考慮してコンテナのBoundsを拡張
            Bounds expandedBounds = new Bounds(
                containerBounds.center,
                containerBounds.size + Vector3.one * tolerance * 2f
            );

            // 対象のBoundsがすべて拡張されたコンテナ内に収まっているかチェック
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            Vector3 containerMin = expandedBounds.min;
            Vector3 containerMax = expandedBounds.max;

            return min.x >= containerMin.x && max.x <= containerMax.x &&
                   min.y >= containerMin.y && max.y <= containerMax.y &&
                   min.z >= containerMin.z && max.z <= containerMax.z;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, _size);
        }
    }
}
