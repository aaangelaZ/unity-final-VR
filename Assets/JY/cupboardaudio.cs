using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cupboardaudio : MonoBehaviour
{

    [SerializeField] private AudioSource cupboard;

    public GameObject spotLight;

    private void Start()
    {
        spotLight.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            cupboard.Play();
            spotLight.SetActive(true);
        }
        
    }
}
