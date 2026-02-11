using UnityEngine;

/// <summary>
/// ドラッグ中アイテムの回転入力を処理する。キーボードとUIボタン両対応。
/// </summary>
public class ItemRotator : MonoBehaviour
{
    private ItemView _target;

    public void SetTarget(ItemView target) => _target = target;
    public void ClearTarget() => _target = null;

    void Update()
    {
        if (_target == null) return;

        if (Input.GetKeyDown(KeyCode.W)) RotateX();
        if (Input.GetKeyDown(KeyCode.S)) RotateXReverse();
        if (Input.GetKeyDown(KeyCode.Q)) RotateY();
        if (Input.GetKeyDown(KeyCode.E)) RotateYReverse();
        if (Input.GetKeyDown(KeyCode.A)) RotateZ();
        if (Input.GetKeyDown(KeyCode.D)) RotateZReverse();
    }

    // UIボタン用publicメソッド
    // 絶対後で直す
    public void RotateX() => Rotate(r => r.RotateX());
    public void RotateXReverse() => Rotate(r => r.RotateXReverse());
    public void RotateY() => Rotate(r => r.RotateY());
    public void RotateYReverse() => Rotate(r => r.RotateYReverse());
    public void RotateZ() => Rotate(r => r.RotateZ());
    public void RotateZReverse() => Rotate(r => r.RotateZReverse());

    private void Rotate(System.Func<RotationState, RotationState> rotate)
    {
        if (_target == null) return;
        _target.ApplyRotation(rotate(_target.CurrentRotation));
    }
}