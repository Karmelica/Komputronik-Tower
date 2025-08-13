using UnityEngine;
using System.Collections.Generic;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance;
    public List<Pool<MonoBehaviour>> pools = new();

    private Dictionary<string, Pool<MonoBehaviour>> poolDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var pool in pools)
        {
            pool.CreatePool();
            poolDict[pool.poolKey] = pool;
        }
    }
    
    public T Get<T>(string key) where T : MonoBehaviour
    {
        if (poolDict.TryGetValue(key, out var pool))
        {
            return pool.Get() as T;
        }
        
        Debug.LogError($"Pool Key: {key} not found");
        return null;
    }

    public void Return(string key, MonoBehaviour obj)
    {
        if (poolDict.TryGetValue(key, out var pool))
        {
            pool.ReturnToPool(obj);
        }
    }
}

[System.Serializable]
public class Pool<T> where T : MonoBehaviour
{
    public string poolKey;
    public T poolObject;
    public int initialSize = 5;
    public Transform parent;
    
    private Queue<T> objects = new();
    
    public void CreatePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            T obj = GameObject.Instantiate(poolObject);

            if (parent != null)
            {
                obj.transform.parent = parent.transform;
            }
            
            obj.gameObject.SetActive(false);
            objects.Enqueue(obj);
        }
    }
    
    public T Get()
    {
        if (objects.Count == 0)
        {
            AddObject();
        }
        
        T obj = objects.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }
    
    public void ReturnToPool(T obj)
    {
        objects.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }

    private void AddObject()
    {
        T obj = GameObject.Instantiate(poolObject);
        obj.gameObject.SetActive(false);
        objects.Enqueue(obj);
    }
}


