using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class candlefire : MonoBehaviour
{
    AudioSource _audioSource;
    public AudioClip burnSound;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("FULU"))
        {
           
            _audioSource.PlayOneShot(burnSound);
        }
        // if(other.gameObject.tag.Equals("Bullet"))
        // {
        //     Destroy(gameObject);
        // }

    }

}
