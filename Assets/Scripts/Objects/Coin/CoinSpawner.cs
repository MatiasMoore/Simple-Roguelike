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
                var obj = Instantiate(coin._coinObj, this.transform.position, Quaternion.identity, null);
                var rb = obj.GetComponent<Rigidbody2D>();
                if (rb  != null)
                {
                    var r = new System.Random();
                    var force = new Vector2(1, 1);
                    force *= (float)r.NextDouble();
                    rb.AddForce(force);
                }
            }
        }
    }


}
