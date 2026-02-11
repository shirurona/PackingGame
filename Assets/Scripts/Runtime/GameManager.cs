using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

/// <summary>
/// ゲーム全体のフロー制御。タイトル→パズル→クリア→次ステージのループを管理する。
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] StageGenerationSettings _settings;
    [SerializeField] StageSpawner _spawner;
    [SerializeField] float _clearDelay = 2f;

    readonly ReactiveProperty<GameState> _state = new(GameState.Title);
    public ReadOnlyReactiveProperty<GameState> State => _state;

    readonly IStageGenerator _generator = new RecursiveSplitStageGenerator();

    StagePlayManager _playManager;
    public StagePlayManager PlayManager => _playManager;

    void Start()
    {
        // 起動時は即ゲーム開始（タイトルUIができたらStartGame()をボタンから呼ぶ）
        StartGame();
    }

    /// <summary>
    /// タイトルからゲーム開始。ステージを生成して Playing に遷移。
    /// </summary>
    public void StartGame()
    {
        var stage = _generator.Generate(_settings);
        _playManager = new StagePlayManager(stage);
        _spawner.SpawnStage(stage);
        _state.Value = GameState.Playing;

        Debug.Log($"ステージ開始: {stage}");
    }

    /// <summary>
    /// クリア判定を実行し、クリアなら次ステージへ遷移する。
    /// アイテム配置後に呼び出すこと。
    /// </summary>
    public void CheckClear()
    {
        if (_playManager.IsClear())
            OnClearedAsync().Forget();
    }

    async UniTaskVoid OnClearedAsync()
    {
        _state.Value = GameState.Cleared;
        Debug.Log("ステージクリア！");

        await UniTask.Delay((int)(_clearDelay * 1000), cancellationToken: destroyCancellationToken);

        StartGame();
    }
}