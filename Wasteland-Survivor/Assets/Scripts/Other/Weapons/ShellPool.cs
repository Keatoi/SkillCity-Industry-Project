using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellPool : MonoBehaviour
{
    public GameObject casingPrefab;
    private GameObject CasingPool;
    public int poolSize = 15;
    private Queue<GameObject> objPool = new Queue<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        CasingPool = new GameObject("Casings pool");
      //Init pool
      for (int i = 0; i < poolSize; i++)
        {
            GameObject go = Instantiate(casingPrefab);
            go.transform.parent = CasingPool.transform;
            go.SetActive(false);
            objPool.Enqueue(go);
        }
    }
    public GameObject GetPoolObject()
    {
        if(objPool.Count > 0)
        {
            GameObject pool = objPool.Dequeue();
            pool.SetActive(true);
            return pool;
        }
        return null;
    }
    public void ReturnToPool(GameObject go)
    {
        go.SetActive(false);
        objPool.Enqueue(go);
    }
}
