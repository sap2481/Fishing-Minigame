using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpostPointer : MonoBehaviour
{
    //==== FIELDS ====
    //Camera
    Camera cam;
    float camHeight;
    float camWidth;

    GameObject player;
    Vector3 playerPosition;

    Outpost outpost;
    Vector3 outpostPosition;

    [SerializeField] GameObject pointer;
    GameObject pointerObj;

    Vector3 distance;

    //==== START ====
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPosition = player.transform.position;

        outpost = GameObject.FindObjectOfType<Outpost>();
        outpostPosition = outpost.transform.position;

        //Set Camera
        cam = Camera.main;
        camHeight = cam.orthographicSize * 2.0f;
        camWidth = cam.orthographicSize * cam.aspect;

        pointerObj = null;
    }

    //==== UPDATE ====
    void Update()
    {
        playerPosition = player.transform.position;

        if (outpost.gameObject.GetComponent<SpriteRenderer>().isVisible == false && pointerObj == null) //If the outpost isn't visible in the camera, instantiate the pointer
        {
            pointerObj = Instantiate(pointer);
            //Debug.Log("Should've instantiated the pointer.");
        }
        else //If it is visible in the camera, destroy the pointer
        {
            if (pointerObj != null) { Destroy(pointerObj.gameObject); }
            pointerObj = null;
        }

        if (pointerObj != null) //If there's a pointer...
        {
            Vector3 pos = camWidth * Vector3.Normalize(outpostPosition - playerPosition) + playerPosition;
            pointerObj.transform.position = pos;
        }
    }
}
