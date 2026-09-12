#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Diagnostics;

public static class Log
{
    [Conditional("ENABLE_LOG")]
    public static void D(string msg)
    {
        UnityEngine.Debug.Log(msg);
    }
    [Conditional("ENABLE_LOG")]
    public static void R(UnityEngine.Vector3 start, UnityEngine.Vector3 dir)
    {
        UnityEngine.Debug.DrawRay(start, dir);
    }
    [Conditional("ENABLE_LOG")]
    public static void R(UnityEngine.Vector3 start, UnityEngine.Vector3 dir, UnityEngine.Color color)
    {
        UnityEngine.Debug.DrawRay(start, dir, color);
    }
}
