public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    DifficultyNum
}
public enum SoundIndex
{
    PlayerVoice,
    PlayerAttack,
    PlayerFootSteps,
    EnemyAttack,
    SoundIndexNum
}

public enum PlayerStatus
{
    Fine,
    FallDown,
    Recover,
    Dead
}
public enum EnemyStatus
{
    Idle,
    Patrol,
    Attack,
    Dead
}
public enum DamageType
{
    None,
    Small,
    Large,
    FallDown,
    Falling,
    Gravity
}
public enum DeadType
{
    Alive,
    HpZero,
    FallClash,
    DropOut,
    TimeOver
}
public enum GameState
{
    Title,
    GamePlay,
    Pause,
    Dead,
    SceneChange
}
public enum ParticleType
{
    Fire,
    Smoke
}

public enum SurfaceType
{
    Undefined,
    Flesh,
    Metal,
    Wood,
    Soil,
    Stone,
    Grass,
    Glass,
    Water
}
