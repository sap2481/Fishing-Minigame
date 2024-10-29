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

        if (collisions.CheckMouseOverlap(mousePosition, this.gameObject) || outpostActive) //If the mouse is overlapping with the outpost...
        {
            //Set range to zero so player can't cast on the outpost
            player.GetComponent<Fishing>().Range = 0;

            if (mouseLeftLastFrame && !mouseLeftThisFrame && !outpostActive) //If the player clicked on the outpost, open the outpost menu
            {
                outpostActive = true;
                opMenuInstance = Instantiate(opMenuPrefab);
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(xOut);
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(BuyButton);
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(SellButton);
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(QuestButton);
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
    public void xOut() //Exit the outpost menu when the X button is clicked
    {
        outpostActive = false;
        Destroy(opMenuInstance.gameObject);
        opMenuInstance = null;
    }

    public void BuyButton() //Change the menu to the Buy screen
    {
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).gameObject.SetActive(false); //Set Sell panel to inactive
    }
    public void SellButton() //Change the menu to the Sell screen
    {
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).gameObject.SetActive(true); //Set Sell panel to active
        for (int i = 0; i < 6; i++)
        {
            if (i < player.GetComponent<Fishing>().FishList.Count) //If there's a fish to go in the sell slot, set it to active & fill it with relevant info
            {
                Fish thisFish = player.GetComponent<Fishing>().FishList[i];

                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).gameObject.SetActive(true);
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = thisFish.Name;
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = "$" + thisFish.Value;
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).GetChild(2).GetComponent<Image>().sprite = thisFish.Sprite;
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { SellTheFish(thisFish); });
            }
            else //If there is not a fish to go in the slot, set the slot to inactive
            {
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    public void QuestButton()
    {
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).gameObject.SetActive(false); //Set Sell panel to inactive
    }

    public void SellTheFish(Fish fish)
    {
        player.GetComponent<Player>().Money += fish.Value;
        Debug.Log("Player Money: $" + player.GetComponent<Player>().Money);
        player.GetComponent<Fishing>().FishList.Remove(fish);
        SellButton();
    }
}