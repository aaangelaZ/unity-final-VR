using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending1 : MonoBehaviour
{
    public GameObject windowEscape; //textMP
    public GameObject Kirby;

    AudioSource _audioSource;
    public AudioClip thunderSound;

    ////set delay timer
    //[SerializeField] private float loadSceneDelay = 2f;
    //private float timeElapsed;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        Kirby.SetActive(false);
        windowEscape.SetActive(false);

        //StartCoroutine(NextLevelAfterWait());
    }

    //IEnumerator NextLevelAfterWait()
    //{
    //    yield return new WaitForSeconds(2.0f);
    //    SceneManager.LoadScene("Ending1");
    //}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            windowEscape.SetActive(true);
            Kirby.SetActive(true);

            //timeElapsed += Time.deltaTime; //set timer

            if (OVRInput.Get(OVRInput.Button.Two))
            {
                
                _audioSource.PlayOneShot(thunderSound);
                SceneManager.LoadScene("Ending1");

                //if (timeElapsed > loadSceneDelay)
                //{
                //    SceneManager.LoadScene("Ending1");
                //}
                
                //StartCoroutine(NextLevelAfterWait());
            }
        }
        
        
        
        
        //if (OVRInput.Get(OVRInput.Button.Two))
        //{
        //        windowGhost1.SetActive(true);
        //        windowGhost2.SetActive(true);
        //        windowGhost3.SetActive(true);
        //        _audioSource.PlayOneShot(thunderSound);
        //        //StartCoroutine(NextLevelAfterWait());
        // }            
    }
}
