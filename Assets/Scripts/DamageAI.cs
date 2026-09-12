using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class DamageAI : DamageController
{    
    Enemy enemy;
    MoveAI moveAI;

    CancellationToken destroyToken;
    CancellationTokenSource flashCts;

    [SerializeField] float sqrDistThreshold = 50f;


    public override void Initialize(Enemy enemy, MoveAI moveAI, GroundMoter groundMoter, Anim anim)
    {
        base.Initialize(null, null, groundMoter, null);
        this.enemy = enemy;
        this.moveAI = moveAI;
    }    
    protected override void Awake()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();
    }
    protected override void OnEnable()
    {

    }
    protected override void OnDisable()
    {
        flashCts?.Cancel();
    }
    protected override void OnDestroy()
    {
        flashCts?.Cancel();
        flashCts?.Dispose();
    }

    public override void Damage(DamageInfo damageInfo)
    {
        base.Damage(damageInfo);

        if (moveAI != null)
        {
            moveAI.IsGreater = true;
            foreach (var e in enemy.PoolManager.AllEnemies0)
            {
                if (e.gameObject == this.gameObject) continue;
                if (!e.gameObject.activeInHierarchy) continue;
                float sqrDist = (e.transform.position - transform.position).sqrMagnitude;
                if (sqrDist < sqrDistThreshold) e.IsGreater = true;
            }
        }

        if (statusController.IsHpZero)
        {
            statusController.Die(DeadType.HpZero);
            return;
        }        

        flashCts?.Cancel();
        flashCts?.Dispose();

        flashCts = new CancellationTokenSource();

        var linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(
                destroyToken, flashCts.Token);

        flashController.FlashAsync(linkedCts).Forget(
            ex => Debug.LogError($"FlashAsyncÇ≈ó·äOÉGÉâÅ[: {ex}"));
    }
}
