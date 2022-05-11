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
    public bool choiceA;
    public bool choiceB;

    AudioSource _audioSource;
    public AudioClip dialogueSound;

    void Start()
    {
        //textBox1.SetActive(false);
        //textBoxA.SetActive(false);
        //textBoxB.SetActive(false);

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
        if (dialogueOn) //开启第一个选择
        {
            if (OVRInput.Get(OVRInput.Button.One)) //press A button - WHAT IS IT
            {
                textBox1.SetActive(false);
                //textBoxA.SetActive(true); //介绍婚帖
                //_audioSource.PlayOneShot(dialogueSound);
                choiceA = true;
            }

            else if (OVRInput.Get(OVRInput.Button.Two)) //press B button - WHO ARE YOU
            {
                textBox1.SetActive(false);
                //textBoxB.SetActive(true);//come find me
                //_audioSource.PlayOneShot(dialogueSound);
                choiceB = true;
            }
            }



        if (choiceA){ //介绍婚帖

            textBoxA.SetActive(true); //介绍婚帖
            choiceB = false;
            dialogueOn = false;

            if (OVRInput.Get(OVRInput.Button.One)) //press A button - who are you
            {
                textBoxA.SetActive(false);
                choiceB = true;
                _audioSource.PlayOneShot(dialogueSound);

            }

            else if (OVRInput.Get(OVRInput.Button.Two)) //press B button - Exit
            {
                choiceA = false;
                choiceB = false;
                textBoxA.SetActive(false);
                textBoxB.SetActive(false);
            }
        }


        if (choiceB)//come find me
        {
            textBoxB.SetActive(true); //come find me
            choiceA = false;
            dialogueOn = false;

            if (OVRInput.Get(OVRInput.Button.Three)) //press X button - what is it
            {
                textBoxB.SetActive(false);
                choiceA = true; //What is this
                _audioSource.PlayOneShot(dialogueSound);


            }
            else if (OVRInput.Get(OVRInput.Button.Four)) //press Y button - Exit 
            {
                choiceA = false;
                choiceB = false;
                textBoxA.SetActive(false);
                textBoxB.SetActive(false);
            }
        }
    }
    }

