using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public abstract class Stage : MonoBehaviour
{
    public abstract Transform SpawnPointsRoot0 { get; }
    public abstract Transform[] SpawnPoints0 { get; }
    public abstract int SpawnPoints0Length { get; set; }
    public abstract int SpawnIndex0 { get; set; }
    public abstract void Initialize(Enemy enemy0, Enemy enemy1, Enemy enemy2, PoolManager poolManager);
    public virtual async UniTask DefaultSpawnEnemy0(CancellationToken ct) { await UniTask.Yield(cancellationToken: ct); }
    public virtual async UniTask StageAsync(CancellationToken ct) { await UniTask.Yield(cancellationToken: ct); }
}
