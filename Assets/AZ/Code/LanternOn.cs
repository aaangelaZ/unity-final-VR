using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternOn : MonoBehaviour
{
    public GameObject LanLight1;
    public GameObject LanLight2;
    public GameObject Lanlight3;
    public GameObject Lanlight4;

    //for canvas dialogue
    AudioSource _audioSource;
    //public AudioClip brideTalk;
    //public GameObject brideText;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        //brideText.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            LanLight1.SetActive(true);
            LanLight2.SetActive(true);
            Lanlight3.SetActive(true);
            Lanlight4.SetActive(true);

            //_audioSource.PlayOneShot(brideTalk);
            //brideText.SetActive(true);
            //Destroy(brideText, 5f);
            //Destroy(brideTalk, 5f);
        }
    }
}
