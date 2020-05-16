using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 20.0f;
    [SerializeField]
    private GameObject _explosionPrefab;

    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        if (_explosionPrefab == null)
        { 
            Debug.LogError("Explosion is NULL");
        }
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate asteroid
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            _spawnManager.StartSpawning();

            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            Destroy(this.gameObject, 0.25f);
        }
    }

}
