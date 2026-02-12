using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

/// <summary>
/// 制限時間のカウントダウンを管理する。
/// R3のReactivePropertyで残り時間を保持し、UniTaskでカウントダウン処理を実行する。
/// </summary>
public class TimeManager : IDisposable
{
    readonly ReactiveProperty<float> _remainingTime = new(0f);
    public ReadOnlyReactiveProperty<float> RemainingTime => _remainingTime;

    readonly Subject<Unit> _onTimeUp = new();
    public Observable<Unit> OnTimeUp => _onTimeUp;

    bool _isRunning;
    CancellationTokenSource _cts;

    /// <summary>
    /// 制限時間を設定してカウントダウンを開始する。
    /// </summary>
    public void StartCountdown(float timeLimit)
    {
        StopCountdown();

        _remainingTime.Value = timeLimit;
        _isRunning = true;
        _cts = new CancellationTokenSource();

        CountdownAsync(_cts.Token).Forget();
    }

    /// <summary>
    /// カウントダウンを停止する。
    /// </summary>
    public void StopCountdown()
    {
        _isRunning = false;
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    async UniTaskVoid CountdownAsync(CancellationToken cancellationToken)
    {
        try
        {
            var previousTime = Time.unscaledTime;

            while (_isRunning && _remainingTime.Value > 0f)
            {
                await UniTask.Delay(100, cancellationToken: cancellationToken); // 約0.1秒ごとに更新

                var currentTime = Time.unscaledTime;
                var delta = currentTime - previousTime;
                previousTime = currentTime;

                _remainingTime.Value = Mathf.Max(0f, _remainingTime.Value - delta);
            }

            if (_remainingTime.Value <= 0f && _isRunning)
            {
                _onTimeUp.OnNext(Unit.Default);
                _isRunning = false;
            }
        }
        catch (OperationCanceledException)
        {
            // キャンセル時は何もしない
        }
    }

    public void Dispose()
    {
        StopCountdown();
        _remainingTime?.Dispose();
        _onTimeUp?.Dispose();
    }
}
