using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Stage1 : Stage
{
    Enemy enemy0;
    Enemy enemy1;
    Enemy enemy2;

    PoolManager poolManager;

    [SerializeField] Transform[] spawnPointRoots;
    public override Transform SpawnPointsRoot0 => spawnPointRoots[0];
    [SerializeField] Transform[] spawnPoints0;
    public override Transform[] SpawnPoints0 => spawnPoints0;
    [SerializeField] int spawnIndex0;
    public override int SpawnIndex0 { get => spawnIndex0; set => spawnIndex0 = value; }
    [SerializeField] int spawnPoints0Length;
    public override int SpawnPoints0Length
    {
        get => spawnPoints0Length;
        set => spawnPoints0Length = value;
    }

    [SerializeField] StageSettings settings;    


    public override void Initialize(Enemy enemy0, Enemy enemy1, Enemy enemy2, PoolManager poolManager)
    {
        this.enemy0 = enemy0;
        this.enemy1 = enemy1;
        this.enemy2 = enemy2;
        this.poolManager = poolManager;
    }

    void Start()
    {
        //enemy0はrespawn有り        
        spawnPoints0 = spawnPointRoots[0].transform
            .Cast<Transform>()
            .ToArray();
        spawnPoints0Length = spawnPoints0.Length;

        //enemy1配置　respawn無し
        SetSpawnPoints(
            spawnPointRoots[1],
            enemy1,
            settings.difficulties[(int)GameSystem.Config.Difficulty].spawnObjs[1].spawnPointIndices);

        //enemy2配置　respawn無し
        SetSpawnPoints(
            spawnPointRoots[2],
            enemy2,
            settings.difficulties[(int)GameSystem.Config.Difficulty].spawnObjs[2].spawnPointIndices);
        
        void SetSpawnPoints(Transform spawnPointsRoot, Enemy enemy, int[] spawnPointIndices)
        {
            var spawnPoints =
                spawnPointsRoot.transform
                .GetComponentsInChildren<Transform>()
                .Where(t => t != spawnPointsRoot.transform)
                .Where((_, index) =>
                {
                    var check = false;
                    foreach (var item in spawnPointIndices)
                    {
                        if (item == index)
                        {
                            check = true;
                            break;
                        }
                    }
                    return check;
                })
                .ToArray();

            foreach (var item in spawnPoints)
            {
                var instance = poolManager.Get(enemy);
                instance.transform.SetPositionAndRotation(item.position, item.rotation);
            }
        }
    }

    public async override UniTask DefaultSpawnEnemy0(CancellationToken ct)
    {
        await UniTask.Yield(cancellationToken: ct);
        for (int i = 0; i < spawnPoints0.Length; i++)
        {
            await UniTask.Yield(cancellationToken: ct);
            var instance = poolManager.Get(enemy0);
        }
    }

    public override async UniTask StageAsync(CancellationToken ct)
    {
        try
        {
            while (true)
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(
                        settings.difficulties[(int)GameSystem.Config.Difficulty].spawnTime),
                    cancellationToken: ct);

                var count = 0;
                foreach (var e in poolManager.AllEnemies0)
                {
                    if (e.gameObject.activeInHierarchy) count++;
                }
                if (count < poolManager.Enemy0_size) //キャラオーバーチェック
                {
                    var instance = poolManager.Get(enemy0);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Log.D("StageAsyncがキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }    
}
