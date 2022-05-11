using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fuludestroy : MonoBehaviour
{
    AudioSource _audioSource;
    public AudioClip burnSound;

    public bool isFire;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("FULU"))
        {
            Destroy(other.gameObject);
            _audioSource.PlayOneShot(burnSound);

            //isFire = true;
            
        }
        // if(other.gameObject.tag.Equals("Bullet"))
        // {
        //     Destroy(gameObject);
        // }

    }

    private void Update()
    {
        if (isFire)
        {
            _audioSource.PlayOneShot(burnSound);
        }
    }

}
