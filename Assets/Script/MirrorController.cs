using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorController: MonoBehaviour
{
    public GameObject vampire;
    AudioSource _audioSource;
    public AudioClip glassBreak;
    public GameObject mirror;

    void Start()
    {
        vampire.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
        //mirror.SetActive(true);
    }

  
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            vampire.SetActive(true);
            _audioSource.PlayOneShot(glassBreak);
            //mirror.SetActive(false);
            Destroy(mirror, 3);
        }
    }
}
