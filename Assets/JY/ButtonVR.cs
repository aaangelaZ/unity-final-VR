using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    public GameObject fuluPrefab;
    GameObject presser;
    AudioSource sound;
    bool isPressed;

    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPressed = false;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(!isPressed)
        {

            button.transform.localPosition = new Vector3(0,0.003f,0);
            presser = other.gameObject;
            onPress.Invoke();
            sound.Play();
            isPressed = true;

        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject == presser)
        {
            button.transform.localPosition = new Vector3(0,0.015f,0);
            onRelease.Invoke();
            isPressed = false;
        }
    }


    public IEnumerator papershow()
    {
        yield return new WaitForSeconds(1);

        for (int i=0; i<20; i++ )
        {
            Vector3 pos = new Vector3 (Random.Range(3f,5f),Random.Range(1f,3f), Random.Range(6f,8f));
            Instantiate(fuluPrefab, pos, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void SpawnSphere()
    {
        // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // sphere.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
        // sphere.transform.localPosition = new Vector3(3,0.5f,7);
        // sphere.AddComponent<Rigidbody>();

    }

}
