using R3;
using TMPro;
using UnityEngine;

/// <summary>
/// 制限時間の残り時間をUIに表示する。
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class TimerView : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;

    TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        if (_gameManager == null)
        {
            Debug.LogError($"{nameof(TimerView)}: {nameof(_gameManager)} is not assigned.", this);
            enabled = false;
            return;
        }

        // GameManagerの残り時間を購読して表示
        _gameManager.RemainingTime?.Subscribe(UpdateTimerText).AddTo(this);
    }

    void UpdateTimerText(float remainingTime)
    {
        int minutes = (int)(remainingTime / 60f);
        int seconds = (int)(remainingTime % 60f);
        _text.text = $"残り時間: {minutes:00}:{seconds:00}";
    }
}
