using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]    private float _speed = 8.0f;
    [SerializeField]    private float _LaserDestructionMaxHeight = 7.1f;
    [SerializeField]    private float _LaserDestructionMinHeight = -5.4f;
    [SerializeField]    private AudioClip _laserAudio;

    private bool _isEnemyLaser = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_laserAudio == null)
        {
            Debug.LogError("Laser Audio clip is NULL");
        }
        else
        {
            AudioSource.PlayClipAtPoint(_laserAudio, transform.position, 1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isEnemyLaser == false)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
        
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y >= _LaserDestructionMaxHeight)
        {
            //Destroy the parent if exits
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < _LaserDestructionMinHeight)
        {
            //Destroy the parent if exits
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            Destroy(this.transform.parent.gameObject);
            this.transform.parent = null;
            Destroy(this.gameObject);
        }
    }

    public bool IsEnemyLaser()
    {
        return _isEnemyLaser;
    }
}
