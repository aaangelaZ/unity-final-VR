using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleOnOff : MonoBehaviour
{
    AudioSource _audioSource;
    public bool onSwitch;
    public bool lightStatus;
    public GameObject theLight;
    public GameObject theFlame;
    public GameObject uiCandle;

    public AudioClip matchStrike;
    public AudioClip candlePutout;

    public GameObject particles;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            uiCandle.SetActive(true);
            onSwitch = true;
        }
        else
        {
            uiCandle.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            uiCandle.SetActive(true);
            onSwitch = false;
        }
        else
        {
            uiCandle.SetActive(false);
        }
    }

    void Update()
    {
        if(theLight.active)
        {
            lightStatus = true;
        }

        if (!theLight.active)
        {
            lightStatus = false;
        }

        if (onSwitch)
        {
            if (!lightStatus)
            {
                if (OVRInput.Get(OVRInput.Button.One))
                {
                    theFlame.SetActive(true);
                    theLight.SetActive(true);
                    _audioSource.PlayOneShot(matchStrike);
                    Destroy(GameObject.FindWithTag("particles"));
                } 
            }
            
            if (onSwitch)
            {
                if (lightStatus)
                {
                   if (OVRInput.GetDown(OVRInput.Button.One))
                   {
                    theFlame.SetActive(false);
                    theLight.SetActive(false);
                    _audioSource.PlayOneShot(candlePutout);
                    }
                }
            }
        }

    }

}
