using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarriageDialogue : MonoBehaviour
{
    public GameObject textBox;

    AudioSource _audioSource;
    public AudioClip dialogueSound;

    void Start()
    {
        textBox.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
    }

    
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textBox.SetActive(true);
            _audioSource.PlayOneShot(dialogueSound);

            Destroy(textBox, 5f);
        }
    }
}
