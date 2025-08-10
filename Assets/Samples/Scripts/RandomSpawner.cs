using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float minRange;
    public float maxRange;
    private float _initialDelay = 1f;
    private float _repeatRate = 3f;

    void Start()
    {
        InvokeRepeating(nameof(PrefabSpawn),_initialDelay,_repeatRate);
    }

    void PrefabSpawn()
    {
        GameObject obj=Instantiate(prefab);
        obj.transform.position = new Vector3(Random.Range(minRange,maxRange),5,0);
    }
}
