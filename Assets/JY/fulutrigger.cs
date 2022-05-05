using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fulutrigger : MonoBehaviour
{
    public GameObject fuluPrefab;

    IEnumerator papershow()
    {
    yield return new WaitForSeconds(1);

    for (int i=0; i<20; i++ )
    {
        Vector3 pos = new Vector3 (Random.Range(-4.87f,-3.8f),Random.Range(1.22f,1.42f), Random.Range(10.64f,10.77f));
        Instantiate(fuluPrefab, pos, Quaternion.identity);
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
