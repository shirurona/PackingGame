using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// アイテム1個の視覚表現とドラッグイベント。Cube primitiveにアタッチして使う。
/// </summary>
[RequireComponent(typeof(MeshRenderer), typeof(BoxCollider))]
public class ItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public ItemData Data { get; private set; }
    public RotationState CurrentRotation { get; private set; }

    public event Action<ItemView, PointerEventData> DragBegun;
    public event Action<ItemView, PointerEventData> Dragged;
    public event Action<ItemView, PointerEventData> DragEnded;
    public event Action<ItemView, PointerEventData> Clicked;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private MeshRenderer _renderer;
    private Color _baseColor;

    public void Initialize(ItemData data)
    {
        Data = data;
        CurrentRotation = data.InitialRotation;
        _baseColor = data.Color;
        _renderer = GetComponent<MeshRenderer>();

        transform.localScale = data.Size;
        transform.rotation = CurrentRotation.ToQuaternion();

        var block = new MaterialPropertyBlock();
        block.SetColor(BaseColorId, _baseColor);
        _renderer.SetPropertyBlock(block);
    }

    public void SetTransparent(bool transparent)
    {
        var color = _baseColor;
        color.a = transparent ? 0.5f : 1f;
        var block = new MaterialPropertyBlock();
        block.SetColor(BaseColorId, color);
        _renderer.SetPropertyBlock(block);
    }

    /// <summary>回転適用後の実サイズ</summary>
    public Vector3 EffectiveSize => CurrentRotation.Apply(Data.Size);

    public void OnBeginDrag(PointerEventData eventData) => DragBegun?.Invoke(this, eventData);
    public void OnDrag(PointerEventData eventData) => Dragged?.Invoke(this, eventData);
    public void OnEndDrag(PointerEventData eventData) => DragEnded?.Invoke(this, eventData);
    public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke(this, eventData);
}
