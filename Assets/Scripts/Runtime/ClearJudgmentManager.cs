using UnityEngine;
using R3;
using System.Linq;

namespace PackingGame
{
    /// <summary>
    /// ステージクリア判定を管理するコンポーネント
    /// </summary>
    public class ClearJudgmentManager : MonoBehaviour
    {
        [SerializeField] private BoxContainer _container;
        [SerializeField] private float _tolerance = 0.1f;
        [SerializeField] private bool _enableAutoJudgment = true;
        [SerializeField] private bool _enableDebugLog = false;

        private Subject<Unit> _onClearSubject;
        private bool _wasCleared = false;

        /// <summary>
        /// 許容範囲（単位: Unity units）
        /// </summary>
        public float Tolerance
        {
            get => _tolerance;
            set => _tolerance = value;
        }

        /// <summary>
        /// クリアイベント（R3のObservable）
        /// </summary>
        public Observable<Unit> OnClear => _onClearSubject;

        private void Awake()
        {
            _onClearSubject = new Subject<Unit>();
        }

        private void Update()
        {
            if (_enableAutoJudgment)
            {
                CheckClearCondition();
            }
        }

        /// <summary>
        /// クリア条件を判定
        /// </summary>
        public void CheckClearCondition()
        {
            if (_container == null)
            {
                if (_enableDebugLog)
                {
                    Debug.LogWarning("Container is not assigned.");
                }
                return;
            }

            // すべてのBoxItemを取得
            BoxItem[] items = FindObjectsByType<BoxItem>(FindObjectsSortMode.None);

            if (items.Length == 0)
            {
                if (_enableDebugLog)
                {
                    Debug.Log("No BoxItems found.");
                }
                return;
            }

            // すべてのアイテムがコンテナ内に収まっているか判定
            bool allContained = items.All(item =>
            {
                Bounds itemBounds = item.GetWorldBounds();
                bool contained = _container.Contains(itemBounds, _tolerance);

                if (_enableDebugLog && !contained)
                {
                    Debug.Log($"Item {item.name} is not fully contained in the container.");
                }

                return contained;
            });

            // クリア状態が変化した場合のみイベント発行
            if (allContained && !_wasCleared)
            {
                _wasCleared = true;
                _onClearSubject.OnNext(Unit.Default);

                if (_enableDebugLog)
                {
                    Debug.Log("★ Stage Clear!");
                }
            }
            else if (!allContained && _wasCleared)
            {
                _wasCleared = false;

                if (_enableDebugLog)
                {
                    Debug.Log("Clear condition is no longer met.");
                }
            }
        }

        /// <summary>
        /// クリア状態をリセット
        /// </summary>
        public void ResetJudgment()
        {
            _wasCleared = false;

            if (_enableDebugLog)
            {
                Debug.Log("Clear judgment has been reset.");
            }
        }

        private void OnDrawGizmos()
        {
            if (_container == null) return;

            // 許容範囲を黄色の半透明で可視化
            Bounds containerBounds = _container.GetWorldBounds();
            Bounds expandedBounds = new Bounds(
                containerBounds.center,
                containerBounds.size + Vector3.one * _tolerance * 2f
            );

            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireCube(expandedBounds.center, expandedBounds.size);
        }

        private void OnDestroy()
        {
            _onClearSubject?.Dispose();
        }
    }
}
