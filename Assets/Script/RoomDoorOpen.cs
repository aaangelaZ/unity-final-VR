using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomDoorOpen : MonoBehaviour
{
    public GameObject roomDoor;
    public GameObject ipadOS;
    public GameObject fuluClueCollider;

    AudioSource _audioSource;
    public AudioClip doorOpenSound;

    void Start()
    {
        roomDoor.SetActive(true);
        //ipadOS.SetActive(false);
        fuluClueCollider.SetActive(true);

        _audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            roomDoor.SetActive(false);
            ipadOS.SetActive(true);
            fuluClueCollider.SetActive(false);

            _audioSource.PlayOneShot(doorOpenSound);
            Destroy(doorOpenSound, 5f);
            Destroy(ipadOS, 8f);
            Destroy(roomDoor, 5f);
            Destroy(fuluClueCollider);
        }

    }
}
