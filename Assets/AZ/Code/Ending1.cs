using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending1 : MonoBehaviour
{
    public GameObject windowEscape;
    public GameObject windowGhost1;
    public GameObject windowGhost2;
    public GameObject windowGhost3;

    AudioSource _audioSource;

    public AudioClip thunderSound;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        StartCoroutine(NextLevelAfterWait());
    }

    IEnumerator NextLevelAfterWait()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("Ending1");
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            windowEscape.SetActive(true);

            
        }
        
        if (OVRInput.Get(OVRInput.Button.Two))
        {
                windowGhost1.SetActive(true);
                windowGhost2.SetActive(true);
                windowGhost3.SetActive(true);
                _audioSource.PlayOneShot(thunderSound);
                StartCoroutine(NextLevelAfterWait());
         }            
    }
}
