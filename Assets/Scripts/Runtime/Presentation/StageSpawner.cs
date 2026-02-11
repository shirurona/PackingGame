using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// StageDataから箱とアイテムのGameObjectを生成する。
/// </summary>
public class StageSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _boxPrefab;
    [SerializeField] private ItemView _itemPrefab;

    private GameObject _boxInstance;
    private readonly List<ItemView> _itemInstances = new();

    public void SpawnStage(StageData stageData)
    {
        ClearStage();

        // 箱を生成
        _boxInstance = Instantiate(_boxPrefab, transform);
        _boxInstance.transform.localScale = stageData.BoxSize;

        // アイテムを生成（箱の手前に仮配置）
        for (int i = 0; i < stageData.Items.Count; i++)
        {
            var itemData = stageData.Items[i];
            var itemView = Instantiate(_itemPrefab, transform);
            itemView.Initialize(itemData);

            var rotatedSize = itemData.InitialRotation.Apply(itemData.Size);
            itemView.transform.position = new Vector3(
                i * 2f,
                rotatedSize.y / 2f,
                -2f
            );

            _itemInstances.Add(itemView);
        }
    }

    private void ClearStage()
    {
        if (_boxInstance != null)
        {
            Destroy(_boxInstance);
            _boxInstance = null;
        }

        foreach (var item in _itemInstances)
            Destroy(item.gameObject);
        _itemInstances.Clear();
    }
}