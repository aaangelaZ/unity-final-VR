using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MirrorEnding : MonoBehaviour
{
    public GameObject windowGhost;

    AudioSource _audioSource;
    public AudioClip thunderSound;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        windowGhost.SetActive(false);

    }

    void Update()
    {
        
    }

    private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Ending1");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            windowGhost.SetActive(true);
            _audioSource.PlayOneShot(thunderSound);
            StartCoroutine(WaitForSceneLoad());
        }
    }
}
