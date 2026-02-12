using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// アイテムのドラッグ移動と配置を制御する。
/// </summary>
public class ItemPlacer : MonoBehaviour
{
    [SerializeField] StageSpawner _spawner;
    [SerializeField] Camera _camera;
    [SerializeField] ItemRotator _rotator;
    [SerializeField] float _snapThreshold = 0.3f;
    [SerializeField, Tooltip("箱内判定の許容誤差")]
    float _boxTolerance = 0.01f;

    private ItemView _dragging;
    private Vector3 _boxSize;
    private IReadOnlyList<PlacedItem> _placedItems;

    /// <summary>アイテム配置時に発火（Data, Position, Rotation）</summary>
    public event Action<ItemData, Vector3, RotationState> ItemPlaced;

    /// <summary>アイテム取り出し時に発火</summary>
    public event Action<ItemData> ItemRemoved;

    /// <summary>ステージ開始時に箱サイズと配置済みアイテム参照をセットする。</summary>
    public void SetPlayContext(Vector3 boxSize, IReadOnlyList<PlacedItem> placedItems)
    {
        _boxSize = boxSize;
        _placedItems = placedItems;
    }

    void OnEnable()
    {
        _spawner.ItemSpawned += OnItemSpawned;
    }

    void OnDisable()
    {
        _spawner.ItemSpawned -= OnItemSpawned;
    }

    void OnItemSpawned(ItemView item)
    {
        item.DragBegun += OnBeginDrag;
        item.Dragged += OnDrag;
        item.DragEnded += OnEndDrag;
        item.Clicked += OnClick;
    }

    void OnBeginDrag(ItemView item, PointerEventData _)
    {
        ItemRemoved?.Invoke(item.Data);

        _dragging = item;
        _dragging.SetTransparent(true);
        _rotator.SetTarget(_dragging);
    }

    void OnDrag(ItemView item, PointerEventData eventData)
    {
        if (_dragging == null) return;

        // マウス位置をワールド座標に変換（カメラのY高さをZ深度として使用）
        var screenPos = eventData.position;
        var worldPos = _camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _camera.transform.position.y));

        var effectiveSize = _dragging.EffectiveSize;
        var pos = new Vector3(worldPos.x, CalcDropY(worldPos, effectiveSize), worldPos.z);

        //if (_placedItems != null)
        //    pos = ItemSnapper.Snap(pos, effectiveSize, _boxSize, _placedItems, _snapThreshold);

        _dragging.transform.position = pos;
    }

    void OnEndDrag(ItemView item, PointerEventData _)
    {
        if (_dragging == null) return;

        _rotator.ClearTarget();
        _dragging.SetTransparent(false);

        var effectiveSize = _dragging.EffectiveSize;
        var center = _dragging.transform.position;

        var cornerPosition = center - effectiveSize * 0.5f;
        ItemPlaced?.Invoke(_dragging.Data, cornerPosition, _dragging.CurrentRotation);

        _dragging = null;
    }

    /// <summary>XZ位置での設置済みアイテムとの重なりから、ドロップ先Y座標を算出する。</summary>
    float CalcDropY(Vector3 center, Vector3 itemSize)
    {
        var halfItem = itemSize * 0.5f;
        float maxTop = 0f;

        if (_placedItems != null)
        {
            for (int i = 0; i < _placedItems.Count; i++)
            {
                var bounds = _placedItems[i].GetBounds();

                bool overlapX = (center.x + halfItem.x > bounds.min.x) && (center.x - halfItem.x < bounds.max.x);
                bool overlapZ = (center.z + halfItem.z > bounds.min.z) && (center.z - halfItem.z < bounds.max.z);

                if (overlapX && overlapZ)
                {
                    float otherTop = bounds.max.y;
                    if (otherTop > maxTop) maxTop = otherTop;
                }
            }
        }

        return maxTop + halfItem.y;
    }

    void OnClick(ItemView item, PointerEventData _)
    {
        ItemRemoved?.Invoke(item.Data);
    }
}