using UnityEngine;
using UnityEngine.Serialization;

public class RandomSpawner : MonoBehaviour
{
    public GameObject _prefab;
    public float _minRange;
    public float _maxRange;
    private float _initialDelay = 1f;
    private float _repeatRate = 3f;

    void Start()
    {
        InvokeRepeating(nameof(PrefabSpawn),_initialDelay,_repeatRate);
    }

    void PrefabSpawn()
    {
        GameObject obj=Instantiate(_prefab);
        obj.transform.position = new Vector3(Random.Range(_minRange,_maxRange),5,0);
    }
}
