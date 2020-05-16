using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]    private float _speed = 2.0f;
    [SerializeField]    private float _topVerticalStartPos = 6f;
    [SerializeField]    private int _enemyKillPoints = 10;
    [SerializeField]    private AudioClip _explosionSound;
    [SerializeField]    private GameObject _enemyLaserPrefab;
    private float _fireRate = 3.0f;
    private float _canFire = -1.0f;
    private bool _isEnemyDestroyed = false;

    private Player _player;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: Check correct player instance for Co-Op Mode
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        _animator = this.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL");
        }

        if (_explosionSound == null)
        {
            Debug.LogError("Enemy explosion sound is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire && _isEnemyDestroyed == false)
        {
            StartCoroutine(FireEnemyLaser());
        }
        
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.4f)
        {
            transform.position = new Vector3(Random.Range(-8f, 8f), _topVerticalStartPos, 0);
        }
    }

    IEnumerator FireEnemyLaser()
    {
        yield return new WaitForEndOfFrame();

        _fireRate = Random.Range(3.0f, 7.0f);
        _canFire = Time.time + _fireRate;

        GameObject enemyLaser = Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (_player != null)
            {
                _player.Damage();
            }

            _speed = 0f;
            Destroy(GetComponent<Collider2D>());

            _animator.SetTrigger("OnEnemyDeath");
            AudioSource.PlayClipAtPoint(_explosionSound, transform.position, 1.0f);

            Destroy(this.gameObject, 2.8f);
        }

        if (other.tag == "Laser")
        {
            Laser laser = other.GetComponent<Laser>();
            if (laser != null)
            {
                if (laser.IsEnemyLaser() == true)
                {
                    //Debug.Log("This is Enemy Laser, Do not Kill Enemy");
                    return;
                }
                
                _isEnemyDestroyed = true;   //Enemy should not be able to fire now
                Destroy(GetComponent<Collider2D>());
                _animator.SetTrigger("OnEnemyDeath");
                _speed = 0f;
                AudioSource.PlayClipAtPoint(_explosionSound, transform.position, 1.0f);
                Destroy(other.gameObject);

                if (_player != null)
                {
                    _player.UpdateScore(_enemyKillPoints);
                }

                Destroy(this.gameObject, 2.8f);
            }
        }
    }
}
