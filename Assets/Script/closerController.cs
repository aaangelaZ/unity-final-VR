using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class closerController : MonoBehaviour
{
    public GameObject vampire;
    AudioSource _audioSource;
    public AudioClip glassBreak;
    public AudioClip scaryLaugh;

    public GameObject closerText;
    public GameObject mirror;


    public GameObject clue;
    public GameObject spotLight;
    public GameObject mirrorCollider;

    public GameObject screen;
    

    void Start()
    {
        vampire.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
        clue.SetActive(false);
        spotLight.SetActive(false);
        screen.SetActive(true);
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
            _audioSource.PlayOneShot(scaryLaugh);

            closerText.SetActive(false);
            spotLight.SetActive(true);

            Destroy(mirror);
            clue.SetActive(true);

            Destroy(vampire, 8f);

            Destroy(mirrorCollider);

            Destroy(spotLight,3f);

            screen.SetActive(false);
        }
    }
}
