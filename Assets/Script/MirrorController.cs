using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorController: MonoBehaviour
{

    AudioSource _audioSource;

    public AudioClip ghostTalk;
 
    public GameObject tipText;

    public GameObject closerCollider;

    void Start()
    {

        _audioSource = GetComponent<AudioSource>();
     
        tipText.SetActive(false);
        closerCollider.SetActive(false);
    }

  
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tipText.SetActive(true);
            closerCollider.SetActive(true);

            _audioSource.PlayOneShot(ghostTalk);
        
        }
    }
}
