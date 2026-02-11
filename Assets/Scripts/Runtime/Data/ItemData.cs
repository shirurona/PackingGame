using UnityEngine;

/// <summary>
/// アイテムの定義データ。ステージ生成時に作られ、変更されない。
/// </summary>
public class ItemData
{
    public int Id;
    public Vector3 Size;
    public Color Color;
    public RotationState InitialRotation;

    public ItemData(int id, Vector3 size, Color color, RotationState initialRotation)
    {
        Id = id;
        Size = size;
        Color = color;
        InitialRotation = initialRotation;
    }

    public override string ToString() => $"Item#{Id} Size={Size} {InitialRotation}";
}
