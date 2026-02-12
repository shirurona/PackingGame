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
    [SerializeField] ItemPlacer _placer;
    [SerializeField] float _clearDelay = 2f;

    readonly ReactiveProperty<GameState> _state = new(GameState.Title);
    public ReadOnlyReactiveProperty<GameState> State => _state;

    readonly IStageGenerator _generator = new RecursiveSplitStageGenerator();

    StagePlayManager _playManager;
    TimeManager _timeManager;

    public ReadOnlyReactiveProperty<float> RemainingTime => _timeManager.RemainingTime;

    void Awake()
    {
        _timeManager = new TimeManager();
        _timeManager.OnTimeUp.Subscribe(_ => OnTimeUpAsync().Forget()).AddTo(this);
    }

    void Start()
    {
        _placer.ItemPlaced += OnItemPlaced;
        _placer.ItemRemoved += OnItemRemoved;

        // 起動時は即ゲーム開始（タイトルUIができたらStartGame()をボタンから呼ぶ）
        StartGame();
    }

    void OnDestroy()
    {
        _timeManager?.Dispose();
    }

    void OnItemPlaced(ItemData data, Vector3 position, RotationState rotation)
    {
        _playManager.PlaceItem(data, position, rotation);
        CheckClear();
    }

    void OnItemRemoved(ItemData data)
    {
        var placed = _playManager.FindByData(data);
        if (placed != null)
            _playManager.RemoveItem(placed);
    }

    /// <summary>
    /// タイトルからゲーム開始。ステージを生成して Playing に遷移。
    /// </summary>
    public void StartGame()
    {
        var stage = _generator.Generate(_settings);
        _playManager = new StagePlayManager(stage);
        _spawner.SpawnStage(stage);
        _placer.SetPlayContext(stage.BoxSize, _playManager.PlacedItems);
        _state.Value = GameState.Playing;

        // 制限時間カウントダウン開始
        _timeManager.StartCountdown(stage.TimeLimit);

        Debug.Log($"ステージ開始: {stage}");
    }

    /// <summary>
    /// クリア判定を実行し、クリアなら次ステージへ遷移する。
    /// アイテム配置後に呼び出すこと。
    /// </summary>
    void CheckClear()
    {
        if (_playManager.IsClear())
            OnClearedAsync().Forget();
    }

    async UniTaskVoid OnClearedAsync()
    {
        _state.Value = GameState.Cleared;
        _timeManager.StopCountdown();
        Debug.Log("ステージクリア！");

        await UniTask.Delay((int)(_clearDelay * 1000), cancellationToken: destroyCancellationToken);

        StartGame();
    }

    async UniTaskVoid OnTimeUpAsync()
    {
        if (_state.Value != GameState.Playing) return;

        _state.Value = GameState.GameOver;
        _placer.enabled = false; // プレイヤー操作を無効化
        Debug.Log("タイムアップ！ゲームオーバー");

        await UniTask.Delay((int)(_clearDelay * 1000), cancellationToken: destroyCancellationToken);

        // ゲームオーバー後は再スタート
        _placer.enabled = true;
        StartGame();
    }
}