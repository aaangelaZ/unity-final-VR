using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cupboardaudio : MonoBehaviour
{

    //[SerializeField] private AudioSource cupboard;
    public AudioClip handlerSound;
    AudioSource _audioSource;


    public GameObject spotLight;

    private void Start()
    {
        spotLight.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            //cupboard.Play();
            _audioSource.PlayOneShot(handlerSound);
            spotLight.SetActive(true);
        }
        
    }
}
