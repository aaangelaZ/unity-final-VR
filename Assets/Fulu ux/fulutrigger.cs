using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fulutrigger : MonoBehaviour
{
    public GameObject fuluPrefab;

    IEnumerator papershow()
    {
    yield return new WaitForSeconds(1);

    for (int i=0; i<15; i++ )
    {
        Vector3 pos = new Vector3 (Random.Range(8.19f, 8.67f),Random.Range(0.70f,0.99f), Random.Range(5.11f, 6.19f));
            
        //Instantiate(fuluPrefab, pos, Quaternion.identity);
        Instantiate(fuluPrefab, pos, Random.rotation);
        yield return new WaitForSeconds(0.1f);
    }
    }

    // public void SpawnSphere()
    // {
    //     StartCoroutine(papershow());
    // }

    private void OnTriggerEnter(Collider other) 
    {

        if(other.gameObject.CompareTag("Player"))
        {   
            StartCoroutine(papershow());
            // Destroy(gameObject);
        }

    }
    

    

}
