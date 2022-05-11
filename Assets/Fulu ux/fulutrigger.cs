using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fulutrigger : MonoBehaviour
{
    public GameObject fuluPrefab;
    //public GameObject fuluClueCollider;

    IEnumerator papershow()
    {
    // yield return new WaitForSeconds(0.5f);

        for (int i=0; i<25; i++ )
        {
            Vector3 pos = new Vector3 (Random.Range(8.34f, 8.45f),Random.Range(1.32f,1.36f), Random.Range(4.11f, 5.11f));
                
            // Instantiate(fuluPrefab, pos, Quaternion.identity);
            Instantiate(fuluPrefab, pos, Random.rotation);
            yield return new WaitForSeconds(0.1f);
        }
    }

    // public void SpawnSphere()
    // {
    //     StartCoroutine(papershow());
    // }

    //private void Start()
    //{
    //    fuluClueCollider.SetActive(false);
    //}

    private void OnTriggerEnter(Collider other) 
    {

        if(other.gameObject.CompareTag("Player"))
        {   
            StartCoroutine(papershow());
            //fuluClueCollider.SetActive(true);
    // Destroy(gameObject);
}

    }
    

    

}
