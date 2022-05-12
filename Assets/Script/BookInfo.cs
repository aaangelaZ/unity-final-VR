using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookInfo : MonoBehaviour
{
    public GameObject bookInfo;
    public bool openBook;
    public GameObject lookText;

    void Start()
    {
        bookInfo.SetActive(false);
        openBook = false;
        lookText.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            //bookInfo.SetActive(true);
            openBook = true;
            lookText.SetActive(false);

            //Destroy(bookInfo, 10f);
        }
    }

    void Update(){
        if(openBook){

            bookInfo.SetActive(true);

            if (OVRInput.Get(OVRInput.Button.Three)) //press X button 
            {
                bookInfo.SetActive(false);
            }
}
}
}
