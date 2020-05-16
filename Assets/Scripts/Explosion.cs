using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]    private AudioClip _explosionSound;

    // Start is called before the first frame update
    void Start()
    {
        if (_explosionSound == null)
        {
            Debug.LogError("Explosion Sound is NULL");
        }
        else
        {
            AudioSource.PlayClipAtPoint(_explosionSound, transform.position, 1.0f);
        }
        Destroy(this.gameObject, 2.5f);
    }
}
