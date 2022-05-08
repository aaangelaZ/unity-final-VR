using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{
    public GameObject testCube;

 
    void Update()
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 pos = new Vector3(Random.Range(8.27f, 8.73f), Random.Range(1.17f, 1.49f), Random.Range(4.11f, 5.11f));

            
            Instantiate(testCube, pos, Random.rotation);
        }
    }
}
