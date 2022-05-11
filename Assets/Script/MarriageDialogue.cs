using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarriageDialogue : MonoBehaviour
{
    public GameObject textBox1;
    public GameObject textBoxA;
    public GameObject textBoxB;

    public bool dialogueOn;
    //public bool choiceA;
    //public bool choiceB;

    AudioSource _audioSource;
    public AudioClip dialogueSound;

    void Start()
    {
        textBox1.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textBox1.SetActive(true);
            dialogueOn = true; //open up dialogue system
            _audioSource.PlayOneShot(dialogueSound);
        }
    }



    void Update()
    {
        if (dialogueOn)
        {
            if (OVRInput.Get(OVRInput.Button.One)) //press A button - WHAT IS IT
            {
                Destroy(textBox1);
                textBoxA.SetActive(true);
                _audioSource.PlayOneShot(dialogueSound);

                if (OVRInput.Get(OVRInput.Button.Three)) //press X button
                {
                    Destroy(textBoxA);
                    textBoxB.SetActive(true); //Who are you
                    _audioSource.PlayOneShot(dialogueSound);
                }

                else if (OVRInput.Get(OVRInput.Button.Two)) 
                {
                    Destroy(textBoxA); //exit dialogue
                }
            }

            else if (OVRInput.Get(OVRInput.Button.Two)) //press B button - WHO ARE YOU
            {
                Destroy(textBox1);
                textBoxB.SetActive(true);
                _audioSource.PlayOneShot(dialogueSound);

                if (OVRInput.Get(OVRInput.Button.Four)) //press Y button
                {
                    Destroy(textBoxB);
                    textBoxA.SetActive(true); //What is this
                    _audioSource.PlayOneShot(dialogueSound);

                }
                else if (OVRInput.Get(OVRInput.Button.Two)) //press B button
                {
                    Destroy(textBoxB); //exit dialogue
                }
            }
            }
        }
    }

