using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternOn : MonoBehaviour
{
    public GameObject LanLight1;
    public GameObject LanLight2;
    public GameObject Lanlight3;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            LanLight1.SetActive(true);
            LanLight2.SetActive(true);
            Lanlight3.SetActive(true);
        }
    }
}
