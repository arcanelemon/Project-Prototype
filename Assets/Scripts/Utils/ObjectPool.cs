using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    //
    private bool isReady;

    #region Singleton 

    //
    public static ObjectPool Instance;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        Instance = this;

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject newObject = Instantiate(pool.prefab);
                newObject.SetActive(false);
                objectPool.Enqueue(newObject);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }

        isReady = true;
    }

    #endregion

    //
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    //
    public List<Pool> pools;

    //
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsReady()
    {
        return isReady;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject SpawnFromPool(string tag)
    {
        if (!poolDictionary.ContainsKey(tag) && tag == "None")
        {
            return null;
        } else if (!poolDictionary.ContainsKey(tag)) 
        {
            Debug.LogError("Error: Pool with Tag " + tag + " does not exist.");
            return null;
        }

        GameObject spawningObject = poolDictionary[tag].Dequeue();

        PooledObject pooledObject = spawningObject.GetComponent<PooledObject>();

        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(spawningObject);

        spawningObject.SetActive(true);

        return spawningObject;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!poolDictionary.ContainsKey(tag) && tag == "None")
        {
            return null;
        }
        else if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError("Error: Pool with Tag " + tag + " does not exist.");
            return null;
        }

        GameObject spawningObject = poolDictionary[tag].Dequeue();
        spawningObject.transform.position = position;
        spawningObject.transform.rotation = rotation;

        if (parent != null)
        {
            spawningObject.transform.parent = parent;
        }

        PooledObject pooledObject = spawningObject.GetComponent<PooledObject>();

        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(spawningObject);

        spawningObject.SetActive(true);

        return spawningObject;
    }
}
