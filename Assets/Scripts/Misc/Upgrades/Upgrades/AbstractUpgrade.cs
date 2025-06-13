using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct UpgradeData
{
    public GameObject Player;
}

public abstract class AbstractUpgrade : MonoBehaviour
{
    public UnityAction<AbstractUpgrade> OnUpgradePickedUp;

    [SerializeField]
    private GameObject _upgradePrefab;
    [SerializeField]
    private int _upgradeCost = 0;

    public abstract bool UpplyUpgrade(UpgradeData data);


    public virtual GameObject SpawnUpgrade(Vector2 coordinates)
    {
        return Instantiate(_upgradePrefab, coordinates, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UpgradeData data = new UpgradeData
            {
                Player = collision.gameObject
            };

            CoinPickupCounter playerWallet = data.Player.GetComponent<CoinPickupCounter>();
            if (playerWallet.Spend(_upgradeCost) == false)
            {
                Debug.Log("Not enough money to pick up upgrade: " + gameObject.name);
                return;
            }

            if (UpplyUpgrade(data))
            {
                OnUpgradePickedUp?.Invoke(this);
                Debug.Log("Upgrade picked up: " + gameObject.name);
            }
        }
    }
}
