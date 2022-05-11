using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookInfo : MonoBehaviour
{
    public GameObject bookInfo;

    void Start()
    {
        bookInfo.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            bookInfo.SetActive(true);
            Destroy(bookInfo, 10f);
        }
    }
}
