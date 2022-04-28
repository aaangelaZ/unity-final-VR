using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class papershowup : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject fuluPrefab;

    IEnumerator papershow()

    {
        yield return new WaitForSeconds(1);

        for (int i=0; i<20; i++ )
        {
            Vector3 pos = new Vector3 (Random.Range(3f,5f),Random.Range(1f,3f), Random.Range(6f,8f));
            Instantiate(fuluPrefab, pos, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }

    }

    
    // IEnumerator Start()
    // {
    //     GameObject[] fulus = GameObject.FindGameObjectWithTag("FULU");

    //     foreach (GameObject fulu in fulus)
    //     {
    //         fulu.SetActive(false);
    //         yield return new WaitForSeconds(0.2f);
    //     }
                
    //     yield return new WaitForSeconds(0.5f);

    //     foreach (GameObject fulu in fulus)
    //     {
    //         fulu.SetActive(true);
    //         yield return new WaitForSeconds(0.2f);
    //     }


    // } 

}
