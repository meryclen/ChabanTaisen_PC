using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PoolManager : MonoBehaviour
{
    [SerializeField] GameObject beam0GO; //ÉvÉåÉCÉÑÅ[íe
    [SerializeField] GameObject beam1GO; //ìGí èÌíe
    [SerializeField] GameObject grenade0GO; //ìGÉOÉåÉlÅ[Éhíe
    [SerializeField] GameObject enemy0GO; //à⁄ìÆÉ^ÉCÉvìG
    [SerializeField] GameObject enemy1GO; //ëÂå^ñCë‰
    [SerializeField] GameObject enemy2GO; //è¨å^ñCë‰
    [SerializeField] GameObject explode0GO; //ÉvÉåÉCÉÑÅ[íeÇÃîöî≠
    [SerializeField] GameObject explode1GO; //ìGÉOÉåÉlÅ[ÉhíeÇÃîöî≠
    [SerializeField] GameObject explode2GO; //ìGí èÌíeÇÃîöî≠
    [SerializeField] GameObject explode3GO; //enemy1ÇÃîöî≠
    [SerializeField] GameObject explode4GO; //enemy0ÅAenemy2ÇÃîöî≠

    [SerializeField] DamagePlayer damagePlayer;

    [SerializeField] int beam0_size = 32;
    [SerializeField] int beam1_size = 64;
    [SerializeField] int grenade0_size = 32;
    [SerializeField] int explode0_size = 32;
    [SerializeField] int explode1_size = 32;
    [SerializeField] int explode2_size = 64;
    [SerializeField] int explode3_size = 16;
    [SerializeField] int explode4_size = 16;
    [SerializeField] int enemy0_size = 16;
    [SerializeField] int enemy1_size = 16;
    [SerializeField] int enemy2_size = 16;

    [SerializeField] List<PoolEntry<Projectile>> projectileEntries = new();
    [SerializeField] List<PoolEntry<Enemy>> enemyEntries = new();
    [SerializeField] List<PoolEntry<Explode>> explodeEntries = new();

    Dictionary<Projectile, ObjectPool<Projectile>> projectilePools = new();
    Dictionary<Enemy, ObjectPool<Enemy>> enemyPools = new();
    Dictionary<Explode, ObjectPool<Explode>> explodePools = new();


    public Projectile Get(Projectile prefab)
    {
        return projectilePools[prefab].Get();
    }
    public Enemy Get(Enemy prefab)
    {
        return enemyPools[prefab].Get();
    }
    public Explode Get(Explode prefab)
    {
        return explodePools[prefab].Get();
    }


    public int Enemy0_size => enemy0_size;

    [SerializeField] List<Enemy0> allEnemies0 = new();
    public IReadOnlyList<Enemy0> AllEnemies0 => allEnemies0;
    [SerializeField] List<Enemy2> allEnemies1 = new();
    public IReadOnlyList<Enemy2> AllEnemies1 => allEnemies1;
    [SerializeField] List<Enemy2> allEnemies2 = new();
    public IReadOnlyList<Enemy2> AllEnemies2 => allEnemies2;

    [SerializeField] List<GameObject> allEnemies0GO = new();
    public IReadOnlyList<GameObject> AllEnemies0GO => allEnemies0GO;
    Dictionary<GameObject, DamageController> goToDamageControllerEnemy0 = new();
    public Dictionary<GameObject, DamageController> GoToDamageControllerEnemy0 =>
        goToDamageControllerEnemy0;

    [SerializeField] List<GameObject> allEnemies1GO = new();
    public IReadOnlyList<GameObject> AllEnemies1GO => allEnemies1GO;
    Dictionary<GameObject, DamageController> goToDamageControllerEnemy1 = new();
    public Dictionary<GameObject, DamageController> GoToDamageControllerEnemy1 =>
        goToDamageControllerEnemy1;

    [SerializeField] List<GameObject> allEnemies2GO = new();
    public IReadOnlyList<GameObject> AllEnemies2GO => allEnemies2GO;
    Dictionary<GameObject, DamageController> goToDamageControllerEnemy2 = new();
    public Dictionary<GameObject, DamageController> GoToDamageControllerEnemy2 =>
        goToDamageControllerEnemy2;


    void Awake()
    {
        SetEntry<Projectile>(beam0GO, beam0_size, projectileEntries, projectilePools);
        SetEntry<Projectile>(beam1GO, beam1_size, projectileEntries, projectilePools);
        SetEntry<Projectile>(grenade0GO, grenade0_size, projectileEntries, projectilePools);
        SetEntry<Enemy>(enemy0GO, enemy0_size, enemyEntries, enemyPools);
        SetEntry<Enemy>(enemy1GO, enemy1_size, enemyEntries, enemyPools);
        SetEntry<Enemy>(enemy2GO, enemy2_size, enemyEntries, enemyPools);
        SetEntry<Explode>(explode0GO, explode0_size, explodeEntries, explodePools);
        SetEntry<Explode>(explode1GO, explode1_size, explodeEntries, explodePools);
        SetEntry<Explode>(explode2GO, explode2_size, explodeEntries, explodePools);
        SetEntry<Explode>(explode3GO, explode3_size, explodeEntries, explodePools);
        SetEntry<Explode>(explode4GO, explode4_size, explodeEntries, explodePools);
    }

    void SetEntry<T>(
        GameObject g,
        int size,
        List<PoolEntry<T>> poolEntries,
        Dictionary<T, ObjectPool<T>> pools
        )
        where T : MonoBehaviour, IPoolable
    {
        var t = g.GetComponent<T>();
        var poolEntry = new PoolEntry<T>();
        poolEntry.prefab = t;
        poolEntry.size = size;
        poolEntries.Add(poolEntry);
        var objectPoolT = new ObjectPool<T>(poolEntry.prefab, poolEntry.size);

        if (g == enemy0GO)
        {
            allEnemies0.AddRange(objectPoolT.AllObj.Cast<Enemy0>());
            allEnemies0GO.AddRange(objectPoolT.AllGO);
            for (int i=0; i<allEnemies0.Count; i++)
            {
                goToDamageControllerEnemy0.Add(
                    allEnemies0GO[i], allEnemies0GO[i].GetComponent<DamageController>());
            }
        }
        if (g == enemy1GO)
        {
            allEnemies1.AddRange(objectPoolT.AllObj.Cast<Enemy2>());
            allEnemies1GO.AddRange(objectPoolT.AllGO);
            for (int i=0; i<allEnemies1.Count; i++)
            {
                goToDamageControllerEnemy1.Add(
                    allEnemies1GO[i], allEnemies1GO[i].GetComponent<DamageController>());
            }
        }
        if (g == enemy2GO)
        {
            allEnemies2.AddRange(objectPoolT.AllObj.Cast<Enemy2>());
            allEnemies2GO.AddRange(objectPoolT.AllGO);
            for (int i = 0; i < allEnemies2.Count; i++)
            {
                goToDamageControllerEnemy2.Add(
                    allEnemies2GO[i], allEnemies2GO[i].GetComponent<DamageController>());
            }
        }
        if (g == beam1GO)
        {
            List<Beam0> allBeam0 = new();
            allBeam0.AddRange(objectPoolT.AllObj.Cast<Beam0>());
            for (int i=0; i<allBeam0.Count; i++)
            {
                allBeam0[i].DamagePlayer = this.damagePlayer;
            }
        }
        if (g == grenade0GO)
        {
            List<Grenade0> allGrenade0 = new();
            allGrenade0.AddRange(objectPoolT.AllObj.Cast<Grenade0>());
            for (int i=0; i<allGrenade0.Count; i++)
            {
                allGrenade0[i].DamagePlayer = this.damagePlayer;
            }
        }

        pools.Add(poolEntry.prefab, objectPoolT);
    }
}

[System.Serializable]
class PoolEntry<T>
{
    public T prefab;
    public int size;
}
