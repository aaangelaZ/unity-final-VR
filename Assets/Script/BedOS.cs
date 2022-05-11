using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedOS : MonoBehaviour
{
    public GameObject brideCopy;
    public GameObject bedOS;
    public GameObject brideOriginal;
    public GameObject spotLight;

    AudioSource _audioSource;
    public AudioClip dialogueSound;

    void Start()
    {
        bedOS.SetActive(false);
        brideCopy.SetActive(false);
        spotLight.SetActive(false);

        brideOriginal.SetActive(true);

        _audioSource = GetComponent<AudioSource>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            bedOS.SetActive(true);
            brideCopy.SetActive(true);
            spotLight.SetActive(true);
            brideOriginal.SetActive(false);

            _audioSource.PlayOneShot(dialogueSound);

            Destroy(bedOS, 5f);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            bedOS.SetActive(false);
            brideCopy.SetActive(false);
            spotLight.SetActive(false);
            brideOriginal.SetActive(true);
        }

    }
}
