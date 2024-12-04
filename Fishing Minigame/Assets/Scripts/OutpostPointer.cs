using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    Vector3 pointerPosition;

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

        Vector3 viewPos = cam.WorldToViewportPoint(outpostPosition); //Get outpost position relative to the viewport
        if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1) //If the outpost is inside the viewport...
        {
            if (pointerObj != null) //If it hasn't been destroyed, destroy the pointer
            {
                Destroy(pointerObj.gameObject);
                pointerObj = null;
            }
        }
        else //If the outpost is NOT inside the viewport
        {
            if (pointerObj == null) { pointerObj = Instantiate(pointer); } //If there is no pointerObj, instantiate one
            
            pointerPosition = outpostPosition;

            //Adjust pointer position to hover at the edge of the viewport
            if (pointerPosition.x < playerPosition.x - camWidth) { pointerPosition.x = playerPosition.x - camWidth + 0.75f; }
            else if (pointerPosition.x > playerPosition.x + camWidth) { pointerPosition.x = playerPosition.x + camWidth - 0.75f; }

            if (pointerPosition.y < playerPosition.y - (camHeight / 2)) { pointerPosition.y = playerPosition.y - (camHeight / 2) + 1f; }
            else if (pointerPosition.y > playerPosition.y + (camHeight / 2)) { pointerPosition.y = playerPosition.y + (camHeight / 2) - 0.75f; }

            pointerObj.transform.position = pointerPosition; //Set pointer transform position
        }

        if (pointerObj != null) { pointerObj.transform.GetChild(1).GetComponent<TMP_Text>().text = Mathf.Round(Vector3.Distance(player.transform.position, outpost.transform.position)) + " Clicks"; }
    }
}
