using UnityEngine;

/// <summary>
/// 90度刻みの回転状態。各軸の回転数(0-3)を保持する。
/// </summary>
public class RotationState
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }

    public RotationState(int x = 0, int y = 0, int z = 0)
    {
        X = ((x % 4) + 4) % 4;
        Y = ((y % 4) + 4) % 4;
        Z = ((z % 4) + 4) % 4;
    }

    /// <summary>
    /// 回転後のサイズを返す。90度刻みなので軸の入れ替えで計算。
    /// </summary>
    public Vector3 Apply(Vector3 size)
    {
        var q = ToQuaternion();
        // 各基底ベクトルを回転し、どの元軸に対応するかを求める
        var rotX = q * Vector3.right;
        var rotY = q * Vector3.up;
        var rotZ = q * Vector3.forward;

        return new Vector3(
            Mathf.Abs(rotX.x) * size.x + Mathf.Abs(rotX.y) * size.y + Mathf.Abs(rotX.z) * size.z,
            Mathf.Abs(rotY.x) * size.x + Mathf.Abs(rotY.y) * size.y + Mathf.Abs(rotY.z) * size.z,
            Mathf.Abs(rotZ.x) * size.x + Mathf.Abs(rotZ.y) * size.y + Mathf.Abs(rotZ.z) * size.z
        );
    }

    public Quaternion ToQuaternion()
    {
        return Quaternion.Euler(X * 90f, Y * 90f, Z * 90f);
    }

    public RotationState RotateX() => new RotationState(X + 1, Y, Z);
    public RotationState RotateY() => new RotationState(X, Y + 1, Z);
    public RotationState RotateZ() => new RotationState(X, Y, Z + 1);

    public static RotationState Random()
    {
        return new RotationState(
            UnityEngine.Random.Range(0, 4),
            UnityEngine.Random.Range(0, 4),
            UnityEngine.Random.Range(0, 4)
        );
    }

    public static RotationState Identity => new RotationState(0, 0, 0);

    public override string ToString() => $"Rot({X * 90}, {Y * 90}, {Z * 90})";
}
