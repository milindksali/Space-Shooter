using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]    private GameObject _enemyPrefab;
    [SerializeField]    private GameObject _enemyContainer;
    
    private bool _stopSpawning = false;
    private bool _lowAmmo = false;

    [SerializeField]    private GameObject[] _powerups;
    [SerializeField]    private GameObject _powerupContainer;

    public void SetLowAmmo(bool value)
    {
        _lowAmmo = value;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
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

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
