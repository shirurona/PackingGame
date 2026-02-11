using UnityEngine;

/// <summary>
/// アイテム1個の視覚表現。Cube primitiveにアタッチして使う。
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class ItemView : MonoBehaviour
{
    public ItemData Data { get; private set; }

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    public void Initialize(ItemData data)
    {
        Data = data;

        transform.localScale = data.Size;
        transform.rotation = data.InitialRotation.ToQuaternion();

        var renderer = GetComponent<MeshRenderer>();
        var block = new MaterialPropertyBlock();
        block.SetColor(BaseColorId, data.Color);
        renderer.SetPropertyBlock(block);
    }
}
