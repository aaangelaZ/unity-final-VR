using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cupboardaudio : MonoBehaviour
{

    [SerializeField] private AudioSource cupboard;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            cupboard.Play();
        }
        
    }
}
