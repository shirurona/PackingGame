using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ中のアイテム配置状態を管理し、クリア判定を行う。
/// </summary>
public class StagePlayManager
{
    readonly StageData _stage;
    readonly List<PlacedItem> _placedItems = new();

    public StagePlayManager(StageData stage)
    {
        _stage = stage;
    }

    public IReadOnlyList<PlacedItem> PlacedItems => _placedItems;

    public void PlaceItem(ItemData data, Vector3 position, RotationState rotation)
    {
        _placedItems.Add(new PlacedItem(data, position, rotation));
    }

    public PlacedItem FindByData(ItemData data)
    {
        foreach (var placed in _placedItems)
        {
            if (placed.Data == data)
                return placed;
        }
        return null;
    }

    public void RemoveItem(PlacedItem placed)
    {
        _placedItems.Remove(placed);
    }

    public bool IsClear()
    {
        return ClearChecker.IsStageClear(_stage.BoxSize, _placedItems, _stage.Items.Count);
    }
}