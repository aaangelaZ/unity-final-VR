using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCollider : MonoBehaviour
{
    AudioSource _audioSource;
    public AudioClip windSound;
    public GameObject windOStext;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        windOStext.SetActive(false);
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
            windOStext.SetActive(true);

            Destroy(windOStext, 5f);

        }
    }
}
