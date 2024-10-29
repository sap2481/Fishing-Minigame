using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class Outpost : MonoBehaviour
{
    //==== FIELDS ====
    [SerializeField] GameObject opMenuPrefab;
    GameObject opMenuInstance;
    bool outpostActive;

    GameObject player;
    float maxSpeedStorage;

    //Mouse Fields
    Vector2 mousePosition;
    bool mouseLeftThisFrame;
    bool mouseLeftLastFrame;

    [SerializeField] Collisions collisions;

    //==== START ====
    void Start()
    {
        //Find Player
        player = GameObject.FindGameObjectWithTag("Player");

        mouseLeftLastFrame = false;
        outpostActive = false;
    }

    //==== UPDATE ====
    void Update()
    {
        //Get Mouse Position & State
        mousePosition = Mouse.current.position.ReadValue();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mouseLeftThisFrame = Mouse.current.leftButton.IsPressed();

        if (collisions.CheckMouseOverlap(mousePosition, this.gameObject) || outpostActive) //If the mouse is overlapping with the outpost
        {
            //Set range to zero so player can't cast on the outpost
            player.GetComponent<Fishing>().Range = 0;

            if (mouseLeftLastFrame && !mouseLeftThisFrame && !outpostActive) //If the player clicked on the outpost, open the outpost menu
            {
                outpostActive = true;
                opMenuInstance = Instantiate(opMenuPrefab);
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(xOut);


            }
        }
        else
        {
            //Reset fishing range
            player.GetComponent<Fishing>().Range = player.GetComponent<Fishing>().RangeStorage;
        }

        mouseLeftLastFrame = mouseLeftThisFrame;
    }

    //==== METHODS & COROUTINES ====
    public void xOut()
    {
        outpostActive = false;
        Destroy(opMenuInstance.gameObject);
        opMenuInstance = null;
    }
}
