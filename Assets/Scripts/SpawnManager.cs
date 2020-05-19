using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;

    private bool _stopSpawning = false;
    private bool _lowAmmo = false;
    private bool _lowHealth = false;

    [SerializeField] private GameObject _powerupContainer;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private GameObject[] _rarePowerups;
    [SerializeField] private float _rarePowerupPercentage = 5.0f;

    private Coroutine _healthPowerupCoroutine = null;

    public void SetLowHealth(bool value)
    {
        _lowHealth = value;

        //Stop any running Coroutine for health generation independent of low health value
        //If low health is true start the coroutine again.
        if (_healthPowerupCoroutine != null)
            StopCoroutine(_healthPowerupCoroutine);

        if (_lowHealth)
        {
            _healthPowerupCoroutine = StartCoroutine(SpawnHealthPowerupRoutine());
        }
    }

    public void SetLowAmmo(bool value)
    {
        _lowAmmo = value;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnRarePowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(Random.Range(-8f, 8f), 6f, 0), Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 7.0f));
            int powerupID;
            if (_lowAmmo)
            {
                powerupID = 3;
            }
            else
            {
                powerupID = Random.Range(0, 3);
            }
            GameObject newPowerup = Instantiate(_powerups[powerupID], new Vector3(Random.Range(-8.0f, 8.0f), 6f, 0), Quaternion.identity);
            newPowerup.transform.parent = _powerupContainer.transform;
        }
    }

    IEnumerator SpawnHealthPowerupRoutine()
    {
        yield return new WaitForSeconds(Random.Range(10.0f, 17.0f));
        while (_stopSpawning == false && _lowHealth == true)
        {
            GameObject newPowerup = Instantiate(_powerups[4], new Vector3(Random.Range(-8.0f, 8.0f), 6f, 0), Quaternion.identity);
            newPowerup.transform.parent = _powerupContainer.transform;
            yield return new WaitForSeconds(Random.Range(10.0f, 15.0f));
        }
    }

    IEnumerator SpawnRarePowerupRoutine()
    {
        yield return new WaitForSeconds(Random.Range(10.0f, 17.0f));
        while (_stopSpawning == false)
        {
            float randomValue = Random.value;
            if (randomValue < (_rarePowerupPercentage/100))
            {
                int powerupID = Random.Range(0, _rarePowerups.Length);
                GameObject newPowerup = Instantiate(_rarePowerups[powerupID], new Vector3(Random.Range(-8.0f, 8.0f), 6f, 0), Quaternion.identity);
                newPowerup.transform.parent = _powerupContainer.transform;
                yield return new WaitForSeconds(Random.Range(30.0f, 45.0f));
            }
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
