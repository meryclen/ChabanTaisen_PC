using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class StatusAI : StatusController
{
    [SerializeField] EnemyStatus curStatus;
    public EnemyStatus CurStatus { get => curStatus; set => curStatus = value; }
    [SerializeField] Enemy enemy;
    [SerializeField] MoveAI moveAI;

    CancellationTokenSource cts;
    CancellationToken destroyToken;
    CancellationTokenSource linkedCts;
    CancellationToken linkedToken;

    [SerializeField] int maxHp = 100;
    [SerializeField] int hp;
    public override int Hp { get => hp; set => hp = value; }
    public override bool IsHpZero => hp <= 0;
    [SerializeField] Explode explodePrefab;


    public void Initialize(Enemy enemy, MoveAI moveAI)
    {
        this.enemy = enemy;
        this.moveAI = moveAI;
    }
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        destroyToken = this.GetCancellationTokenOnDestroy();
        linkedCts = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, cts.Token);
        linkedToken = linkedCts.Token;

        hp = maxHp;

        if (moveAI != null)
        {
            CurStatus = EnemyStatus.Patrol;
            StatusAsync(linkedToken).Forget(
                ex => Debug.LogError($"StatusAsyncで例外エラー: {ex}"));
        }
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

    async UniTask StatusAsync(CancellationToken ct)
    {
        try
        {
            ct.ThrowIfCancellationRequested();
            while (true)
            {
                int rnd = UnityEngine.Random.Range(5, 10);
                await UniTask.Delay(TimeSpan.FromSeconds(rnd), cancellationToken: ct);

                rnd = UnityEngine.Random.Range(0, 5);
                if (rnd > 1)
                {
                    CurStatus = EnemyStatus.Patrol;
                    rnd = UnityEngine.Random.Range(0, moveAI.TargetIndexNum);
                    moveAI.TargetIndex = rnd;                    
                }
                else
                {
                    CurStatus = EnemyStatus.Idle;
                }
            }
        }
        catch (OperationCanceledException)
        {
            Log.D("StatusAsyncがキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public override void Die(DeadType deadType)
    {
        var explode = enemy.PoolManager.Get(explodePrefab);
        explode.transform.position = transform.position;
        if (explode is Explode1 e1)
        {
            e1.Explode();
        }
        enemy.Pool.Return(enemy);
    }
}
