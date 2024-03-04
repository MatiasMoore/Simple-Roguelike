using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField]
    private Pedestrian _ped;

    [Serializable]
    public struct CoinToSpawn
    {
        [SerializeField]
        public GameObject _coinObj;

        [SerializeField] 
        public int _count;
    }

    [SerializeField]
    private float _spawnRadius = 1;

    [SerializeField]
    private List<CoinToSpawn> _coints = new List<CoinToSpawn>();

    private void Awake()
    {
        _ped.OnDeath += SpawnCoins;
    }

    private void SpawnCoins()
    {
        foreach (var coin in _coints)
        {
            for (int i = 0; i < coin._count; i++)
            {
                var spawnPos = _spawnRadius * UnityEngine.Random.insideUnitSphere;
                var obj = Instantiate(coin._coinObj, transform.position + spawnPos, Quaternion.identity, transform.parent);
                var rb = obj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(UnityEngine.Random.insideUnitSphere);
                }
            }
        }
    }


}
