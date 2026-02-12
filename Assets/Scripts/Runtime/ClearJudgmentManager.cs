using UnityEngine;
using R3;

/// <summary>
/// ステージクリア判定を管理するマネージャー
/// </summary>
public class ClearJudgmentManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private BoxContainer _container;

    [Header("設定")]
    [SerializeField, Tooltip("許容範囲（単位: Unity units）")]
    private float _tolerance = 0.1f;

    [SerializeField, Tooltip("Update毎に自動で判定を行うか")]
    private bool _enableAutoJudgment = true;

    [SerializeField, Tooltip("デバッグログを出力するか")]
    private bool _enableDebugLog = false;

    private Subject<Unit> _onClearSubject = new Subject<Unit>();
    private bool _wasCleared = false;

    /// <summary>
    /// クリア時に発行されるObservable
    /// </summary>
    public Observable<Unit> OnClear => _onClearSubject;

    /// <summary>
    /// 許容範囲
    /// </summary>
    public float Tolerance
    {
        get => _tolerance;
        set => _tolerance = value;
    }

    private void Update()
    {
        if (_enableAutoJudgment)
        {
            CheckClearCondition();
        }
    }

    /// <summary>
    /// クリア条件を判定する
    /// </summary>
    /// <returns>クリア条件を満たしている場合true</returns>
    public bool CheckClearCondition()
    {
        if (_container == null)
        {
            if (_enableDebugLog)
            {
                Debug.LogWarning("Container が設定されていません");
            }
            return false;
        }

        // シーン内のすべてのBoxItemを取得
        BoxItem[] items = FindObjectsByType<BoxItem>(FindObjectsSortMode.None);

        if (items.Length == 0)
        {
            if (_enableDebugLog)
            {
                Debug.Log("BoxItem が見つかりません");
            }
            return false;
        }

        // すべてのアイテムがコンテナ内に収まっているか判定
        bool allItemsContained = true;
        foreach (BoxItem item in items)
        {
            Bounds itemBounds = item.GetWorldBounds();
            if (!_container.Contains(itemBounds, _tolerance))
            {
                allItemsContained = false;
                if (_enableDebugLog)
                {
                    Debug.Log($"アイテム {item.name} がコンテナ外です");
                }
                break;
            }
        }

        // クリア状態が変化した場合のみイベントを発行
        if (allItemsContained && !_wasCleared)
        {
            _wasCleared = true;
            if (_enableDebugLog)
            {
                Debug.Log("★ ステージクリア！");
            }
            _onClearSubject.OnNext(Unit.Default);
        }
        else if (!allItemsContained && _wasCleared)
        {
            _wasCleared = false;
            if (_enableDebugLog)
            {
                Debug.Log("クリア状態が解除されました");
            }
        }

        return allItemsContained;
    }

    /// <summary>
    /// クリア状態をリセットする
    /// </summary>
    public void ResetJudgment()
    {
        _wasCleared = false;
        if (_enableDebugLog)
        {
            Debug.Log("判定状態をリセットしました");
        }
    }

    private void OnDrawGizmos()
    {
        if (_container == null) return;

        // 許容範囲を可視化（黄色半透明）
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Bounds containerBounds = _container.GetWorldBounds();
        Vector3 expandedSize = containerBounds.size + Vector3.one * _tolerance * 2f;
        Gizmos.DrawCube(containerBounds.center, expandedSize);

        // 許容範囲の枠線（黄色）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(containerBounds.center, expandedSize);
    }

    private void OnDestroy()
    {
        _onClearSubject?.Dispose();
    }
}
