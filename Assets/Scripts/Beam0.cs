using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class Beam0 : Projectile
{
    [SerializeField] float speed = 10f;
    [SerializeField] float sqrShotDistanceLimit = 200f;

    Vector3 startPos;
    bool iscur = false;
        
    Vector3 prevPos;
    [SerializeField] float radius = 0.1f;
    [SerializeField] LayerMask hitLayerMask;
    [SerializeField] LayerMask damageLayerMask;

    CancellationTokenSource cts;
    CancellationTokenSource linkedCts;
    CancellationToken destroyToken;
    CancellationToken linkedToken;

    [SerializeField] float curR;
    [SerializeField] float curG;
    [SerializeField] float curB;
    [SerializeField] float curA;

    [SerializeField] float initEmissionStrength;
    [SerializeField] float curEmissionStrength;

    [SerializeField] Explode explodePrefab;
    Explode0 explode0;

    Coloring coloring;

    [SerializeField] DamageInfo beam0DamageInfo = new();
    [SerializeField] SoundManager soundManager;

    DamagePlayer damagePlayer;
    public DamagePlayer DamagePlayer { get => damagePlayer; set => damagePlayer = value; }


    protected override void Awake()
    {
        base.Awake();
        
        if (explodePrefab is Explode0 e0) explode0 = e0;
        MeshRenderer mr = GetComponent<MeshRenderer>();

        coloring = new Coloring(mr);
    }
    void OnEnable()
    {
        iscur = false;
        cts = new CancellationTokenSource();
        destroyToken = this.GetCancellationTokenOnDestroy();
        linkedCts = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, cts.Token);
        linkedToken = linkedCts.Token;
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
                prevPos, radius, curDir, out RaycastHit hit, dist, hitLayerMask);
            prevPos = curPos;

            if (hitCheck)
            {
                beam0DamageInfo.DamageHitNormal = hit.normal;
                Collider col = hit.collider;
                DamageController d = null;
                DamageController enemyDamageController = null;

                if ((LayerMask.GetMask("Player") & (1 << col.gameObject.layer)) != 0)
                {
                    damagePlayer.Damage(beam0DamageInfo);
                }
                else if ((LayerMask.GetMask("Enemy0") & (1 << col.gameObject.layer)) != 0)
                {
                    enemyDamageController =
                        PoolManager.GoToDamageControllerEnemy0[col.transform.root.gameObject];
                    enemyDamageController.Damage(beam0DamageInfo);
                }
                else if ((LayerMask.GetMask("Enemy1") & (1 << col.gameObject.layer)) != 0)
                {
                    enemyDamageController =
                        PoolManager.GoToDamageControllerEnemy1[col.transform.root.gameObject];
                    enemyDamageController.Damage(beam0DamageInfo);
                }
                else if ((LayerMask.GetMask("Enemy2") & (1 << col.gameObject.layer)) != 0)
                {
                    enemyDamageController =
                        PoolManager.GoToDamageControllerEnemy2[col.transform.root.gameObject];
                    enemyDamageController.Damage(beam0DamageInfo);
                }                
                else { }

                var explode = PoolManager.Get(explodePrefab);
                explode.transform.position = hit.point;
                if (explode is Explode0 e0)
                {
                    if (d != null) e0.Explode(d.SurfaceType);
                    if (enemyDamageController != null)
                    {
                        e0.Explode(enemyDamageController.SurfaceType);
                    }
                }
                Pool.Return(this);
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
