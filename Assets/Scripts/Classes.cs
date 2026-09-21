using UnityEngine;

[System.Serializable]
public class DifficultySettings
{
    public Difficulty difficulty;
    public SpawnObj[] spawnObjs;
    public float spawnTime;
}
[System.Serializable]
public class SpawnObj
{
    public int[] spawnPointIndices;
}

[System.Serializable]
public class ScreenResolution
{
    public int Index;
    public int Width;
    public int Height;
}
[System.Serializable]
public class TargetFrameRate
{
    public int Index;
    public int FrameRate;
}
[System.Serializable]
public class Config
{
    public Difficulty Difficulty = Difficulty.Normal;
    public bool IsFullScreen = false;
    public int CurResolutionIndex = 0;
    public bool IsVsync = false;
    public int CurFrameRateIndex = 1;
    public bool[] Sounds;
    public float MouseSensitivity = 0.5f;
    public int MouseSensitivityIndex = 0;
    public float MouseSensitivityCamera = 0.5f;
    public int MouseSensitivityIndexCamera = 0;

    public Config() { }
    public Config(Config source) //コピーコンストラクタ
    {
        CopyFrom(source);
    }
    public void CopyFrom(Config source)
    {
        Difficulty = source.Difficulty;
        IsFullScreen = source.IsFullScreen;
        CurResolutionIndex = source.CurResolutionIndex;
        IsVsync = source.IsVsync;
        CurFrameRateIndex = source.CurFrameRateIndex;
        Sounds = (bool[])source.Sounds.Clone();
        MouseSensitivity = source.MouseSensitivity;
        MouseSensitivityIndex = source.MouseSensitivityIndex;
        MouseSensitivityCamera = source.MouseSensitivityCamera;
        MouseSensitivityIndexCamera = source.MouseSensitivityIndexCamera;
    }
}

[System.Serializable]
public class DamageInfo
{
    public int AttackPower;
    public int ShockPower;
    public Vector3 DamageHitNormal;
    public DamageType DamageType;
    public bool CanAnimOverride = true;
    public int CurShockPower;
}
