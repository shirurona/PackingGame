using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// アイテムのドラッグ移動と配置を制御する。
/// </summary>
public class ItemPlacer : MonoBehaviour
{
    [SerializeField] StageSpawner _spawner;
    [SerializeField] Camera _camera;

    private ItemView _dragging;
    private Plane _dragPlane;

    /// <summary>アイテム配置時に発火（Data, Position, Rotation）</summary>
    public event Action<ItemData, Vector3, RotationState> ItemPlaced;

    /// <summary>アイテム取り出し時に発火</summary>
    public event Action<ItemData> ItemRemoved;

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
        _dragPlane = new Plane(Vector3.up, new Vector3(0, item.transform.position.y, 0));
    }

    void OnDrag(ItemView item, PointerEventData eventData)
    {
        if (_dragging == null) return;

        var ray = _camera.ScreenPointToRay(eventData.position);
        if (_dragPlane.Raycast(ray, out float distance))
        {
            var point = ray.GetPoint(distance);
            var pos = _dragging.transform.position;
            pos.x = point.x;
            pos.z = point.z;
            _dragging.transform.position = pos;
        }
    }

    void OnEndDrag(ItemView item, PointerEventData _)
    {
        if (_dragging == null) return;

        _dragging.SetTransparent(false);

        var effectiveSize = _dragging.EffectiveSize;
        var center = _dragging.transform.position;
        var cornerPosition = center - effectiveSize * 0.5f;
        ItemPlaced?.Invoke(_dragging.Data, cornerPosition, _dragging.CurrentRotation);

        _dragging = null;
    }

    void OnClick(ItemView item, PointerEventData _)
    {
        ItemRemoved?.Invoke(item.Data);
    }
}