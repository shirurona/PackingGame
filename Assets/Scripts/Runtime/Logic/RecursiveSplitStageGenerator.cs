using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 再帰分割方式のステージ生成。
/// 箱の内部空間を再帰的に切断してアイテムを生成する。
/// 必ず解が存在することが保証される。
/// </summary>
public class RecursiveSplitStageGenerator : IStageGenerator
{
    public StageData Generate(StageGenerationSettings settings)
    {
        var boxSize = new Vector3(
            Random.Range(settings.BoxSizeMin.x, settings.BoxSizeMax.x),
            Random.Range(settings.BoxSizeMin.y, settings.BoxSizeMax.y),
            Random.Range(settings.BoxSizeMin.z, settings.BoxSizeMax.z)
        );

        int targetCount = Random.Range(settings.ItemCountMin, settings.ItemCountMax + 1);

        // 箱全体を初期領域として開始
        var regions = new List<Bounds>
        {
            new Bounds(boxSize * 0.5f, boxSize)
        };

        // 目標アイテム数まで分割
        while (regions.Count < targetCount)
        {
            int regionIndex = SelectRegion(regions, settings.MinEdgeSize);
            if (regionIndex < 0) break; // 分割可能な領域がない

            var region = regions[regionIndex];
            var splitResult = TrySplit(region, settings.MinEdgeSize);
            if (splitResult == null) break;

            regions.RemoveAt(regionIndex);
            regions.Add(splitResult.Value.a);
            regions.Add(splitResult.Value.b);
        }

        // 領域をアイテムに変換
        var items = new List<ItemData>();
        for (int i = 0; i < regions.Count; i++)
        {
            var r = regions[i];
            items.Add(new ItemData(
                id: i,
                size: r.size,
                color: GenerateRandomColor(),
                initialRotation: RotationState.Random()
            ));
        }

        return new StageData(boxSize, items);
    }

    private static int SelectRegion(List<Bounds> regions, float minEdge)
    {
        var splittable = new List<int>();
        for (int i = 0; i < regions.Count; i++)
        {
            if (HasSplittableAxis(regions[i].size, minEdge))
                splittable.Add(i);
        }

        if (splittable.Count == 0) return -1;

        return splittable[Random.Range(0, splittable.Count)];
    }

    private static bool HasSplittableAxis(Vector3 size, float minEdge)
    {
        float threshold = minEdge * 2f;
        return size.x >= threshold || size.y >= threshold || size.z >= threshold;
    }

    private static (Bounds a, Bounds b)? TrySplit(Bounds region, float minEdge)
    {
        // ランダムな軸から順に分割可能な軸を探す
        float threshold = minEdge * 2f;
        int start = Random.Range(0, 3);
        int axis = -1;
        for (int i = 0; i < 3; i++)
        {
            int candidate = (start + i) % 3;
            if (region.size[candidate] >= threshold) { axis = candidate; break; }
        }

        if (axis < 0) return null;

        float splitPoint = region.min[axis] + Random.Range(minEdge, region.size[axis] - minEdge);

        var aMax = region.max;
        aMax[axis] = splitPoint;

        var bMin = region.min;
        bMin[axis] = splitPoint;

        var a = new Bounds();
        var b = new Bounds();
        a.SetMinMax(region.min, aMax);
        b.SetMinMax(bMin, region.max);

        return (a, b);
    }

    static Color GenerateRandomColor()
    {
        return Color.HSVToRGB(
            Random.Range(0f, 1f),
            Random.Range(0.4f, 0.8f),
            Random.Range(0.6f, 0.9f)
        );
    }
}
