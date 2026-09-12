using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public abstract class GroundMoter : MonoBehaviour, ITickable
{
    protected CancellationTokenSource cts;
    protected CancellationToken destroyToken;
    protected CancellationTokenSource linkedCts;
    protected CancellationToken linkedToken;

    [SerializeField] protected GroundSettings settings;

    Vector3 move;
    public Vector3 Move { get => move; set => move = value; }

    protected float blowDist;
    [SerializeField] protected float blowDistMaxDelta = 0.1f;

    protected Vector3 desiredV;
    protected Vector3 blowV;
    protected bool isBlow;
    protected float gravityVelocity;

    public bool IsGrounded;

    public DamageInfo gravityDamageInfo = new();


    protected abstract void Awake();
    
    void OnEnable()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();
        blowDist = blowDistMaxDelta;
        gravityDamageInfo.DamageType = DamageType.None;
    }
    void OnDisable()
    {
        cts?.Cancel();
        linkedCts?.Dispose();
        linkedCts = null;
        cts?.Dispose();
        cts = null;
    }
    void OnDestroy()
    {
        cts?.Dispose();
        linkedCts?.Dispose();
    }
    public abstract void Tick();
    public void LateTick() { }

    protected abstract UniTask BlowAsync(CancellationToken ct);
}

[System.Serializable]
public class GroundHit
{
    public bool hit;
    public Vector3 point;
    public Vector3 normal;
}
