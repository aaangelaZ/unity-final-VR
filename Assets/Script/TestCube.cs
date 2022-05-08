using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{
    public GameObject testCube;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 pos = new Vector3(Random.Range(8.19f, 8.67f), Random.Range(0.70f, 0.99f), Random.Range(5.11f, 6.19f));

            //Instantiate(fuluPrefab, pos, Quaternion.identity);
            Instantiate(testCube, pos, Random.rotation);
        }
}
