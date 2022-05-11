using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuluClue : MonoBehaviour
{
    public GameObject playerOsText;

    public void Start()
    {
        playerOsText.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerOsText.SetActive(true);

        }

    }
}
