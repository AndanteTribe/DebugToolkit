using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float minRange;
    public float maxRange;

    void Start()
    {
        InvokeRepeating("PrefabSpawn",1,3);
    }

    void PrefabSpawn()
    {
        GameObject obj=Instantiate(prefab);
        obj.transform.position = new Vector3(Random.Range(minRange,maxRange),5,0);
    }
}
