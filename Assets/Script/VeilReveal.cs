using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VeilReveal : MonoBehaviour
{
    public GameObject spotLight;
    public GameObject dialogueSystem;

    void Start()
    {
        spotLight.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            spotLight.SetActive(true);
            Destroy(dialogueSystem);
        }
    }
}
