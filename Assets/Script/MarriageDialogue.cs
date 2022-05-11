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
            if (OVRInput.Get(OVRInput.Button.One)) //press A button
            {
                Destroy(textBox1);
                textBoxA.SetActive(true);
                _audioSource.PlayOneShot(dialogueSound);

                if (OVRInput.Get(OVRInput.Button.One)) //press A button
                {
                    Destroy(textBoxA);
                    textBoxB.SetActive(true);
                    _audioSource.PlayOneShot(dialogueSound);
                }

                else if (OVRInput.Get(OVRInput.Button.Two)) //press B button
                {
                    Destroy(textBoxA);
                }
            }

            else if (OVRInput.Get(OVRInput.Button.Two)) //press B button
            {
                Destroy(textBox1);
                textBoxB.SetActive(true);
                _audioSource.PlayOneShot(dialogueSound);

                if (OVRInput.Get(OVRInput.Button.One)) //press A button
                {
                    Destroy(textBoxB);
                    textBoxA.SetActive(true);
                    _audioSource.PlayOneShot(dialogueSound);

                }
                else if (OVRInput.Get(OVRInput.Button.Two)) //press B button
                {
                    Destroy(textBoxB);
                }
            }
        }
    }
}
