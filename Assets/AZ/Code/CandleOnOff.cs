using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleOnOff : MonoBehaviour
{
    public bool onSwitch;
    public bool lightStatus;
    public GameObject theLight;
    public GameObject theFlame;
    public GameObject uiCandle;
    public AudioSource myFx;
    public AudioClip matchStrike;
    public AudioClip candlePutout;

    void Start()
    {
        //theLight.SetActive(false);
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
                    myFx.PlayOneShot(matchStrike);
                } 
            }
            
            if (onSwitch)
            {
                if (lightStatus)
                {
                   if (OVRInput.Get(OVRInput.Button.One))
                   {
                    theFlame.SetActive(false);
                    theLight.SetActive(false);
                    myFx.PlayOneShot(candlePutout);
                   }
                }
            }
        }

    }

}
