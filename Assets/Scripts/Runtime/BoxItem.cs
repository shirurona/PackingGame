using UnityEngine;

namespace PackingGame
{
    /// <summary>
    /// 直方体アイテムを表現するコンポーネント
    /// </summary>
    public class BoxItem : MonoBehaviour
    {
        [SerializeField] private Vector3 _size = Vector3.one;

        /// <summary>
        /// アイテムのサイズ
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
            // 8つの角を取得
            Vector3[] corners = new Vector3[8];
            Vector3 halfSize = _size * 0.5f;

            corners[0] = transform.TransformPoint(new Vector3(-halfSize.x, -halfSize.y, -halfSize.z));
            corners[1] = transform.TransformPoint(new Vector3(halfSize.x, -halfSize.y, -halfSize.z));
            corners[2] = transform.TransformPoint(new Vector3(-halfSize.x, halfSize.y, -halfSize.z));
            corners[3] = transform.TransformPoint(new Vector3(halfSize.x, halfSize.y, -halfSize.z));
            corners[4] = transform.TransformPoint(new Vector3(-halfSize.x, -halfSize.y, halfSize.z));
            corners[5] = transform.TransformPoint(new Vector3(halfSize.x, -halfSize.y, halfSize.z));
            corners[6] = transform.TransformPoint(new Vector3(-halfSize.x, halfSize.y, halfSize.z));
            corners[7] = transform.TransformPoint(new Vector3(halfSize.x, halfSize.y, halfSize.z));

            // 最小・最大座標を計算
            Vector3 min = corners[0];
            Vector3 max = corners[0];

            for (int i = 1; i < corners.Length; i++)
            {
                min = Vector3.Min(min, corners[i]);
                max = Vector3.Max(max, corners[i]);
            }

            Vector3 center = (min + max) * 0.5f;
            Vector3 size = max - min;

            return new Bounds(center, size);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, _size);
            Gizmos.matrix = oldMatrix;
        }
    }
}
