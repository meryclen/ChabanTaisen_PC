using UnityEngine;
using System.Collections.Generic;

public class ObjectPool<T> : IPool
    where T : MonoBehaviour, IPoolable
{
    Queue<T> pool = new();
    T prefab;
    readonly List<T> allObj = new();
    public IReadOnlyList<T> AllObj => allObj;
    readonly List<GameObject> allGO = new();
    public IReadOnlyList<GameObject> AllGO => allGO;

    public ObjectPool(T prefab, int size)
    {
        this.prefab = prefab;
        for (int i = 0; i < size; i++)
        {
            T obj = GameObject.Instantiate(prefab);
            obj.SetPool(this);
            allGO.Add(obj.gameObject);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
            allObj.Add(obj);
        }
    }

    public T Get()
    {
        T obj;
        obj = pool.Dequeue();        
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }

    public void Return(IPoolable obj)
    {
        Return((T)obj);
    }
}
