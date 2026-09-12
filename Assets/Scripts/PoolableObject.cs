using UnityEngine;

public abstract class PoolableObject : MonoBehaviour, IPoolable
{
    IPool pool;
    public IPool Pool => pool;
    PoolManager poolManager;
    public PoolManager PoolManager => poolManager;
    
    protected virtual void Awake()
    {
        poolManager = FindFirstObjectByType<PoolManager>();
    }
    
    public virtual void SetPool(IPool pool)
    {
        this.pool = pool;
    }
}

public abstract class Projectile : PoolableObject
{

}

public abstract class Enemy : PoolableObject
{
    public virtual bool IsGreater { get; set; }
}

public abstract class Explode : PoolableObject
{

}
