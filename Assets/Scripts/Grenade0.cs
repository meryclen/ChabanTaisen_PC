using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class Grenade0 : Projectile
{
    [SerializeField] float speed = 10f;
    [SerializeField] float sqrShotDistanceLimit = 200f;
    
    Vector3 startPos;
    bool iscur = false;
    
    Vector3 prevPos;
    [SerializeField] float sphereRadius = 0.1f;
    [SerializeField] float overlapSphereRadius = 5f;
    [SerializeField] LayerMask hitLayerMask;
    [SerializeField] LayerMask damageLayerMask;
    LayerMask playerLayer;

    CancellationTokenSource cts;
    CancellationTokenSource linkedCts;
    CancellationToken destroyToken;
    CancellationToken linkedToken;
    
    [SerializeField] float curR = 10f;
    [SerializeField] float curG = 10f;
    [SerializeField] float curB = 1f;
    [SerializeField] float curA = 0.5f;

    [SerializeField] float initEmissionStrength = 10f;
    [SerializeField] float curEmissionStrength = 0.5f;
    
    [SerializeField] Explode explodePrefab;

    Coloring coloring;

    [SerializeField] DamageInfo grenade0DamageInfo = new();

    [SerializeField] Collider[] overlapSphereResults;
    int overlapSphereResultsLength = 16;

    [SerializeField] SoundManager soundManager;
    DamagePlayer damagePlayer;
    public DamagePlayer DamagePlayer { get => damagePlayer; set => damagePlayer = value; }


    protected override void Awake()
    {
        base.Awake();

        MeshRenderer mr = GetComponent<MeshRenderer>();
        coloring = new Coloring(mr);
        overlapSphereResults = new Collider[overlapSphereResultsLength];

        playerLayer = LayerMask.GetMask("Player");
    }
    void OnEnable()
    {
        iscur = false;

        cts = new CancellationTokenSource();
        destroyToken = this.GetCancellationTokenOnDestroy();
        linkedCts = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, cts.Token);
        linkedToken = linkedCts.Token;

        Array.Clear(overlapSphereResults, 0, overlapSphereResultsLength);

        grenade0DamageInfo.ShockPower = 500;
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
    
    void Update()
    {
        Vector3 curPos = transform.position;

        if (!iscur)
        {
            iscur = true;
            startPos = transform.position;
            coloring.ColorSetting(curR, curG, curB, curA, initEmissionStrength);
            FireStartAsync(linkedToken).Forget(
                ex => Debug.LogError($"FireStartAsyncで例外エラー: {ex}"));            
            prevPos = curPos;
        }

        transform.position += transform.forward * speed * Time.deltaTime;
        Vector3 dir = curPos - startPos;

        if (dir.sqrMagnitude > sqrShotDistanceLimit) //一定距離以上飛来すると消える
        {
            Pool.Return(this);
        }
        else
        {
            Vector3 curDir = (curPos - prevPos).normalized;
            float dist = curDir.magnitude;
            bool hitCheck = Physics.SphereCast(
                prevPos, sphereRadius, curDir, out RaycastHit hit, dist, hitLayerMask);
            prevPos = curPos;

            if (hitCheck)
            {
                Collider col = hit.collider;

                if ((playerLayer & (1 << col.gameObject.layer)) != 0)
                {
                    damagePlayer.Damage(grenade0DamageInfo);
                }
                /*
                    if ((damageLayerMask & (1 << col.gameObject.layer)) != 0)
                {
                    grenade0DamageInfo.DamageHitNormal = hit.normal;
                    grenade0DamageInfo.AttackPower = 75;
                    grenade0DamageInfo.ShockPower = 500;
                    var d = col.GetComponentInParent<DamageController>();
                    if (d != null) d.Damage(grenade0DamageInfo);
                }
                */
                var explode = PoolManager.Get(explodePrefab);
                explode.transform.position = hit.point;
                if (explode is Explode1 e1) e1.Explode();

                Pool.Return(this);

                if ((playerLayer & (1 << col.gameObject.layer)) != 0) return;

                int count = Physics.OverlapSphereNonAlloc(
                    hit.point, overlapSphereRadius, overlapSphereResults);

                for (int i = 0; i < count; i++)
                {
                    if ((playerLayer & (1 << overlapSphereResults[i].gameObject.layer)) != 0)
                    {
                        var overlapSphereDir =
                            hit.point - overlapSphereResults[i].transform.position;
                        var overlapSphereDirDist = overlapSphereDir.magnitude;
                        grenade0DamageInfo.DamageHitNormal = overlapSphereDir.normalized;

                        int tmpAttackPower;
                        int tmpShockPower;
                        if (overlapSphereDirDist < 1.5f)
                        {
                            tmpAttackPower = 75;
                            tmpShockPower = 500;
                            grenade0DamageInfo.DamageType = DamageType.FallDown;
                        }
                        else if (overlapSphereDirDist < 2f)
                        {
                            tmpAttackPower = 25;
                            tmpShockPower = 300;
                            grenade0DamageInfo.DamageType = DamageType.FallDown;
                        }
                        else
                        {
                            tmpAttackPower = 10;
                            tmpShockPower = 100;
                        }
                        grenade0DamageInfo.AttackPower = tmpAttackPower;
                        grenade0DamageInfo.ShockPower = tmpShockPower;
                        damagePlayer.Damage(grenade0DamageInfo);
                        break;
                    }
                }
            }
        }
    }

    async UniTask FireStartAsync(CancellationToken ct)
    {
        try
        {
            soundManager.Raise();
            await UniTask.Delay(100, cancellationToken: ct);            
            coloring.ColorSetting(curR, curG, curB, curA, curEmissionStrength);
        }
        catch (OperationCanceledException)
        {
            Log.D("FireStartAsyncがキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
