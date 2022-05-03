using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending1 : MonoBehaviour
{
    public GameObject windowEscape;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            windowEscape.SetActive(true);
        }

        if (OVRInput.Get(OVRInput.Button.Two))
        {
            SceneManager.LoadScene("Ending1");
        }
    }
}
