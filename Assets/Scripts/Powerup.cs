using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]    private float _speed = 3.0f;
    [SerializeField]    private int _powerupID;     //0 = Tripleshot, 1 = Speed, 2 = Shield
    [SerializeField]    private AudioClip _PowerupAudio;

    // Start is called before the first frame update
    void Start()
    {
        if (_PowerupAudio == null)
        {
            Debug.LogError("Powerup sound is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -5.4f)
        {
            Destroy(this.gameObject);
        }        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if tripshot powerup
        //enable tripleshot powerup on player
        //destroy this object
        if (other.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(_PowerupAudio, transform.position);

            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0: 
                        player.ActivateTripleShotPowerup();
                        break;
                    case 1:
                        player.ActivateSpeedPowerup();
                        break;
                    case 2:
                        player.ActivateShieldPowerup();
                        break;
                    case 3:
                        player.IncreaseAmmoCount(15);
                        break;
                    case 4: 
                        player.LivesCollected();
                        break;
                    default:
                        Debug.LogError("Invalid Powerup ID assigned");
                        break;
                }
            }
            
            Destroy(this.gameObject);
        }

    }
}
