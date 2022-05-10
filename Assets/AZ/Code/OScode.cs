using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OScode : MonoBehaviour
{
    public GameObject playerOsText;
    public GameObject endingOsText;
    
    public void Start()
    {
        playerOsText.SetActive(false);
        endingOsText.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerOsText.SetActive(true);
            endingOsText.SetActive(true);

            Destroy(playerOsText, 5f);
        }
        else
        {
            endingOsText.SetActive(false);
        }
    }
}
