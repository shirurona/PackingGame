using UnityEngine;

/// <summary>
/// ステージ生成パラメータ。エディタで調整可能。
/// </summary>
[CreateAssetMenu(fileName = "StageGenerationSettings", menuName = "PackingGame/Stage Generation Settings")]
public class StageGenerationSettings : ScriptableObject
{
    [Header("箱サイズ")]
    public Vector3 BoxSizeMin = new Vector3(3f, 3f, 3f);
    public Vector3 BoxSizeMax = new Vector3(8f, 8f, 8f);

    [Header("アイテム数")]
    public int ItemCountMin = 5;
    public int ItemCountMax = 10;

    [Header("分割制約")]
    [Tooltip("アイテムの最小辺サイズ。これ未満の辺は生成されない")]
    public float MinEdgeSize = 0.5f;
}
