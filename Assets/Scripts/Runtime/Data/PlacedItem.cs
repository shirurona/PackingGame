using UnityEngine;

/// <summary>
/// 箱に配置されたアイテムの状態。
/// </summary>
public class PlacedItem
{
    public ItemData Data;
    public Vector3 Position;
    public RotationState Rotation;

    public PlacedItem(ItemData data, Vector3 position, RotationState rotation)
    {
        Data = data;
        Position = position;
        Rotation = rotation;
    }

    /// <summary>
    /// 回転適用後の実サイズ
    /// </summary>
    public Vector3 EffectiveSize => Rotation.Apply(Data.Size);

    /// <summary>
    /// ワールド空間でのBoundsを返す。Positionはアイテムの左下奥の角。
    /// </summary>
    public Bounds GetBounds()
    {
        var size = EffectiveSize;
        var center = Position + size * 0.5f;
        return new Bounds(center, size);
    }

    public override string ToString() => $"Placed Item#{Data.Id} at {Position} {Rotation}";
}
