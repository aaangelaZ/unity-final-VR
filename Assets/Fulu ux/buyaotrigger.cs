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
            SceneManager.LoadScene("Ending2");
        }

        // if(other.gameObject.tag.Equals("Bullet"))
        // {
        //     Destroy(gameObject);
        // }

    }
}
