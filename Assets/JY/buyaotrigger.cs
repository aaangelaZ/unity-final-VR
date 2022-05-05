using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buyaotrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("buyao"))
        {
            SceneManager.LoadScene("StartScene");
        }

        // if(other.gameObject.tag.Equals("Bullet"))
        // {
        //     Destroy(gameObject);
        // }

    }
}
