using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCollider : MonoBehaviour
{
    AudioSource _audioSource;
    public AudioClip windSound;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

 
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player entered");
            _audioSource.PlayOneShot(windSound);

        }
    }
}
