using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージの定義データ。箱サイズとアイテム一覧を保持する。
/// </summary>
public class StageData
{
    public Vector3 BoxSize;
    public List<ItemData> Items;

    public StageData(Vector3 boxSize, List<ItemData> items)
    {
        BoxSize = boxSize;
        Items = items;
    }

    public override string ToString()
    {
        return $"Stage: Box={BoxSize}, Items={Items.Count}";
    }
}
