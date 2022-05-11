using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomDoorOpen : MonoBehaviour
{
    public GameObject roomDoor;
    public GameObject ipadOS;

    AudioSource _audioSource;
    public AudioClip doorOpenSound;

    void Start()
    {
        roomDoor.SetActive(true);
        ipadOS.SetActive(false);

        _audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            roomDoor.SetActive(false);
            ipadOS.SetActive(true);
            Destroy(ipadOS, 3.5f);

            _audioSource.PlayOneShot(doorOpenSound);
        }

    }
}