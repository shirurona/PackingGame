/// <summary>
/// ステージ生成のインターフェース。
/// 別の生成アルゴリズムを追加する場合はこれを実装する。
/// </summary>
public interface IStageGenerator
{
    StageData Generate(StageGenerationSettings settings);
}
