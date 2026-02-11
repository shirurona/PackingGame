using UnityEngine;

/// <summary>
/// ステージ生成の動作確認用。Phase 1完了後に削除予定。
/// </summary>
public class StageGeneratorTest : MonoBehaviour
{
    [SerializeField] StageGenerationSettings _settings;
    [SerializeField] StageSpawner _spawner;

    void Start()
    {
        if (_settings == null)
        {
            Debug.LogError("StageGenerationSettingsがアサインされていません");
            return;
        }

        IStageGenerator generator = new RecursiveSplitStageGenerator();
        var stage = generator.Generate(_settings);

        Debug.Log($"=== ステージ生成結果 ===");
        Debug.Log($"箱サイズ: {stage.BoxSize}");
        Debug.Log($"箱体積: {stage.BoxSize.x * stage.BoxSize.y * stage.BoxSize.z:F3}");
        Debug.Log($"アイテム数: {stage.Items.Count}");

        float totalVolume = 0f;
        foreach (var item in stage.Items)
        {
            Debug.Log($"  {item} -> 回転後サイズ: {item.InitialRotation.Apply(item.Size)}");
            totalVolume += item.Size.x * item.Size.y * item.Size.z;

            // 最小辺サイズチェック
            if (item.Size.x < _settings.MinEdgeSize - 0.001f ||
                item.Size.y < _settings.MinEdgeSize - 0.001f ||
                item.Size.z < _settings.MinEdgeSize - 0.001f)
            {
                Debug.LogWarning($"  [警告] 最小辺サイズ違反: {item.Size}");
            }
        }

        Debug.Log($"アイテム体積合計: {totalVolume:F3}");
        float boxVolume = stage.BoxSize.x * stage.BoxSize.y * stage.BoxSize.z;
        Debug.Log($"体積差分: {Mathf.Abs(totalVolume - boxVolume):F6}");

        if (Mathf.Abs(totalVolume - boxVolume) > 0.01f)
            Debug.LogError("体積が一致しません！");
        else
            Debug.Log("体積チェック OK");

        // 3Dオブジェクトを生成
        if (_spawner != null)
            _spawner.SpawnStage(stage);
        else
            Debug.LogWarning("StageSpawnerがアサインされていません");
    }
}
