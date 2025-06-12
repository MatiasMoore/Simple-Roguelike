using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour
{
    public UnityAction<AbstractUpgrade> OnUpgradePickedUp;

    [SerializeField]
    private int _spawnCount = 3;

    [SerializeField]
    private List<GameObject> _upgrades;

    private List<GameObject> _spawnedUpgrades;

    private void Awake()
    {
        _spawnedUpgrades = new List<GameObject>();
    }

    public void SpawnUpgrages()
    {
        Vector2 coordinates = GameObject.FindGameObjectWithTag("Player").transform.position - new Vector3(0, 2, 0);
        
        for (int i = 0; i < _spawnCount; i++)
        {
            var randomIndex = Random.Range(0, _upgrades.Count);
            Debug.Log("Spawning Index: " + randomIndex);

            AbstractUpgrade prefabUpgrade = _upgrades[randomIndex].GetComponent<AbstractUpgrade>();
            GameObject spawnedUpgrade = prefabUpgrade.SpawnUpgrade(coordinates);
            _spawnedUpgrades.Add(spawnedUpgrade);
            spawnedUpgrade.GetComponent<AbstractUpgrade>().OnUpgradePickedUp += UpgradePickUp;
        }
    }

    private void UpgradePickUp(AbstractUpgrade upgrade)
    {
        foreach (var spawnedUpgrade in _spawnedUpgrades)
        {
            Destroy(spawnedUpgrade);
        }
        OnUpgradePickedUp?.Invoke(upgrade);
    }
}
