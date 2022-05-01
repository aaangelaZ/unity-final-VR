using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fuludestroy : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("fire"))
        {
            Destroy(gameObject);
        }
        // if(other.gameObject.tag.Equals("Bullet"))
        // {
        //     Destroy(gameObject);
        // }

    }

}
