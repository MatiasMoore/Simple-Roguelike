using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour
{
    public UnityAction<AbstractUpgrade> OnUpgradePickedUp;

    [SerializeField]
    private int _spawnCount = 3;
    [SerializeField]
    private float _spawnMargin = 1f;
    [SerializeField] 
    private float _wallMargin = 0.5f;

    [SerializeField]
    private List<GameObject> _randomUpgrades;
    [SerializeField]
    private List<GameObject> _garanteedUpgrades;

    private List<GameObject> _spawnedUpgrades;

    private void Awake()
    {
        _spawnedUpgrades = new List<GameObject>();
    }

    public void SpawnUpgrages(Vector2 upperLeft, Vector2 lowerRight)
    {
        Vector2 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        var coordinates = CalculateSpawnPositions(upperLeft, lowerRight, playerPosition);
        if (coordinates.Count < _spawnCount)
            return;
        int i;
        for (i = 0; i < _garanteedUpgrades.Count && i < coordinates.Count; i++)
        {
             AddUpgradeOnScene(coordinates[i], _garanteedUpgrades[i].GetComponent<AbstractUpgrade>());
        }

        List<GameObject> upgrades = GetRandomisedUpgrades(_spawnCount - i, _randomUpgrades);

        for (; i < _spawnCount && i < coordinates.Count; i++)
        {
            AddUpgradeOnScene(coordinates[i], upgrades[i - _garanteedUpgrades.Count].GetComponent<AbstractUpgrade>());
        }
    }

    private List<GameObject> GetRandomisedUpgrades(int count, List<GameObject> upgradesList)
    {
        var randomisedUpgrades = new List<GameObject>();
        var availableUpgrades = new List<GameObject>(upgradesList);
        for (int i = 0; i < count && availableUpgrades.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableUpgrades.Count);
            randomisedUpgrades.Add(availableUpgrades[randomIndex]);
            availableUpgrades.RemoveAt(randomIndex);
        }
        return randomisedUpgrades;
    }

    private void AddUpgradeOnScene(Vector2 position, AbstractUpgrade upgradePrefab)
    {
        AbstractUpgrade prefabUpgrade = upgradePrefab.GetComponent<AbstractUpgrade>();
        GameObject spawnedUpgrade = prefabUpgrade.SpawnUpgrade(position);
        _spawnedUpgrades.Add(spawnedUpgrade);
        spawnedUpgrade.GetComponent<AbstractUpgrade>().OnUpgradePickedUp += UpgradePickUp;
    }

    private List<Vector2> CalculateSpawnPositions(Vector2 upperLeft, Vector2 lowerRight, Vector2 playerPosition)
    {
        var result = new List<Vector2>();
        var takenPositions = new List<Vector2>();

        float minX = upperLeft.x + _wallMargin;
        float maxX = lowerRight.x - _wallMargin;
        float minY = lowerRight.y + _wallMargin;
        float maxY = upperLeft.y - _wallMargin;

        int attempts = 0;
        int maxAttempts = _spawnCount * 20; // ограничение на случай невозможной генерации

        while (result.Count < _spawnCount && attempts < maxAttempts)
        {
            attempts++;

            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            Vector2 candidate = new Vector2(x, y);

            bool tooCloseToPlayer = Vector2.Distance(candidate, playerPosition) < _spawnMargin;
            bool tooCloseToOthers = takenPositions.Any(p => Vector2.Distance(p, candidate) < _spawnMargin);

            if (!tooCloseToPlayer && !tooCloseToOthers)
            {
                result.Add(candidate);
                takenPositions.Add(candidate);
            }
        }

        if (result.Count < _spawnCount)
        {
            Debug.LogWarning("Could not find enough valid spawn positions.");
        }

        return result;
    }

    private void UpgradePickUp(AbstractUpgrade upgrade)
    {
        if (upgrade is NewWeaponUpgrade)
        {
            if (_randomUpgrades.Contains(upgrade.UpgradePrefab))
            {
                _randomUpgrades.Remove(upgrade.UpgradePrefab);
            }

            if (_garanteedUpgrades.Contains(upgrade.UpgradePrefab))
            {
                _garanteedUpgrades.Remove(upgrade.UpgradePrefab);
            }
        }
        foreach (var spawnedUpgrade in _spawnedUpgrades)
        {
            Destroy(spawnedUpgrade);
        }
        OnUpgradePickedUp?.Invoke(upgrade);
    }
}
