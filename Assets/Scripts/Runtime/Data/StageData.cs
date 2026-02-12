using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージの定義データ。箱サイズ、アイテム一覧、制限時間を保持する。
/// </summary>
public class StageData
{
    public Vector3 BoxSize;
    public List<ItemData> Items;
    public float TimeLimit;

    public StageData(Vector3 boxSize, List<ItemData> items, float timeLimit)
    {
        BoxSize = boxSize;
        Items = items;
        TimeLimit = timeLimit;
    }

    public override string ToString()
    {
        return $"Stage: Box={BoxSize}, Items={Items.Count}, TimeLimit={TimeLimit}s";
    }
}
