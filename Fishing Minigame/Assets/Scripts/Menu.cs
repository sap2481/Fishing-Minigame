using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    //==== FIELDS ====
    [SerializeField] GameObject menuPrefab;
    GameObject menuInstance;
    bool escKeyPressedLastFrame;
    bool escKeyPressedThisFrame;
    bool menuActive;
    bool fishSelected;

    float maxSpeedStorage;
    float rangeStorage;

    GameObject player;

    [SerializeField] Sprite tempFishSprite; //This is temporary functionality until each fish has their own sprite
    
    //==== START ====
    void Start()
    {
        escKeyPressedLastFrame = false;
        escKeyPressedThisFrame = false;
        menuActive = false;
        fishSelected = false;

        player = GameObject.FindGameObjectWithTag("Player");
    }

    //==== UPDATE ====
    void Update()
    {
        escKeyPressedThisFrame = Keyboard.current.escapeKey.isPressed;

        if (!escKeyPressedThisFrame && escKeyPressedLastFrame) //If the Escape key was just pressed...
        {
            if (!menuActive) //If the menu is not active, activate it
            {
                menuActive = true;
                menuInstance = Instantiate(menuPrefab);
                menuInstance.transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(ResetClicked);

                maxSpeedStorage = player.GetComponent<Player>().MaxSpeed;
                player.GetComponent<Player>().MaxSpeed = 0;

                rangeStorage = player.GetComponent<Fishing>().Range;
                player.GetComponent<Fishing>().Range = 0;
            }
            else //If the menu is active, deactivate it
            {
                menuActive = false;
                Destroy(menuInstance.gameObject);
                menuInstance = null;

                player.GetComponent<Player>().MaxSpeed = maxSpeedStorage;
                player.GetComponent<Fishing>().Range = rangeStorage;
            }
        }

        if (menuInstance != null)
        {
            //Fill Out Ship Information
            menuInstance.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Boat Speed: " + maxSpeedStorage + " Knots";
            menuInstance.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "Line Range: " + rangeStorage + " Clicks";

            for (int i = 0; i < 6; i++) //Fill in cargo hold with fish
            {
                if (i < player.GetComponent<Fishing>().FishList.Count)
                {
                    menuInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(i).GetComponent<Image>().sprite = tempFishSprite;
                    menuInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = "Name: " + player.GetComponent<Fishing>().FishList[i].Name;
                    menuInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = "Value: $" + player.GetComponent<Fishing>().FishList[i].Value;
                }
                else
                {
                    menuInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(i).GetComponent<Image>().sprite = null;
                    menuInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = "Name: ---";
                    menuInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = "Value: ---";
                }
            }
        }

        escKeyPressedLastFrame = escKeyPressedThisFrame;
    }

    //==== METHODS & COROUTINES ====
    void ResetClicked()
    {
        Debug.Log("Game Reset.");

        player.GetComponent<Player>().ResetTiles = true;
        player.GetComponent<Player>().Position = new Vector3(0, 0, 0);
        player.GetComponent<Player>().Hull = 100;
        player.GetComponent<Fishing>().FishCaught = 0;
        player.GetComponent<Fishing>().FishList.Clear();

        menuActive = false;
        Destroy(menuInstance.gameObject);
        menuInstance = null;

        player.GetComponent<Player>().MaxSpeed = maxSpeedStorage;
        player.GetComponent<Fishing>().Range = rangeStorage;
    }
}
