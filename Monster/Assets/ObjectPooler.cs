using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [System.Serializable]
    public class PoolObj
    {
        public List<GameObject> poolDict;
        public GameObject prefab;
    }

    #region Singleton
    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public List<Pool> pools;
    private Dictionary<string, PoolObj> poolDictionary;
    
    void Start()
    {
        poolDictionary = new Dictionary<string, PoolObj>();

        foreach ( Pool pool in pools)
        {
            PoolObj newPoolObj = new PoolObj();
            newPoolObj.poolDict = new List<GameObject>();

            newPoolObj.prefab = pool.prefab;
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.SetParent(gameObject.transform);
                obj.SetActive(false);

                newPoolObj.poolDict.Add(obj);
            }
            poolDictionary.Add(pool.tag, newPoolObj);
        }
    }


    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("pool with tag" + tag + "doesn't exist");
            return null;
        }

        GameObject ret = null;
        foreach (var go in poolDictionary[tag].poolDict)
        {
            if (!go.activeInHierarchy)
            {
                ret = go;
                break;
            }
        }

        if (ret == null)
        {
            GameObject newObj = Instantiate(poolDictionary[tag].prefab);
            newObj.transform.SetParent(gameObject.transform);
            newObj.SetActive(false);
            poolDictionary[tag].poolDict.Add(newObj);
            ret = newObj;
        }

        ret.SetActive(true);
        ret.transform.position = position;
        ret.transform.rotation = rotation;


        return ret;
    }
}
