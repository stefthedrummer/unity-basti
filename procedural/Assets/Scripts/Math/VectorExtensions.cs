using UnityEngine;
using Unity.Mathematics;

public static class VectorExtensions {

    public static Vector3 ToXZ(this float2 v) => new Vector3(v.x, 0, v.y);
    public static float2 XZ(this Vector3 v) => new float2(v.x, v.z);

}
