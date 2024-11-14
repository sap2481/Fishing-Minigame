using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //==== FIELDS ====
    [SerializeField] GameObject menuPrefab;
    public GameObject menuInstance;
    bool escKeyPressedLastFrame;
    bool escKeyPressedThisFrame;
    bool menuActive;

    float maxSpeedStorage;

    GameObject player;
    Outpost outpost;

    [SerializeField] Sprite tempFishSprite; //This is temporary functionality until each fish has their own sprite
    [SerializeField] Collisions collisions;
    [SerializeField] Sprite blankIcon;
    Vector2 mousePosition;
    
    //==== START ====
    void Start()
    {
        escKeyPressedLastFrame = false;
        escKeyPressedThisFrame = false;
        menuActive = false;

        player = GameObject.FindGameObjectWithTag("Player");
        outpost = GameObject.FindObjectOfType<Outpost>();
    }

    //==== UPDATE ====
    void Update()
    {
        escKeyPressedThisFrame = Keyboard.current.escapeKey.isPressed;
        //Get Mouse Position
        mousePosition = Mouse.current.position.ReadValue();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (!escKeyPressedThisFrame && escKeyPressedLastFrame) //If the Escape key was just pressed...
        {
            if (!menuActive && !outpost.outpostActive) //If the menu is not active, activate it
            {
                menuActive = true;
                menuInstance = Instantiate(menuPrefab);
                menuInstance.transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(ResetClicked);

                maxSpeedStorage = player.GetComponent<Player>().MaxSpeed;
                player.GetComponent<Player>().MaxSpeed = 0;

                //player.GetComponent<Fishing>().Range = 0;
                //Debug.Log("Range = " + player.GetComponent<Fishing>().Range);
            }
            else if (menuActive) //If the menu is active, deactivate it
            {
                menuActive = false;
                Destroy(menuInstance.gameObject);
                menuInstance = null;

                player.GetComponent<Player>().MaxSpeed = maxSpeedStorage;
                //player.GetComponent<Fishing>().Range = player.GetComponent<Fishing>().RangeStorage;
                //Debug.Log("Range = " + player.GetComponent<Fishing>().Range);
            }
        }

        if (menuActive) { player.GetComponent<Fishing>().Range = 0; }
        else { player.GetComponent<Fishing>().Range = player.GetComponent<Fishing>().RangeStorage; }

        if (menuInstance != null)
        {
            //Fill Out Ship Information
            menuInstance.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Boat Speed: " + maxSpeedStorage + " Knots";
            menuInstance.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "Line Range: " + player.GetComponent<Fishing>().RangeStorage + " Clicks";
            menuInstance.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = "Hull: " + player.GetComponent<Player>().Hull + " / " + player.GetComponent<Player>().MaxHull;
            menuInstance.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetComponent<TMP_Text>().text = "Money: $" + player.GetComponent<Player>().Money;

            for (int i = 0; i < 6; i++) //Fill in cargo hold with fish
            {
                if (i < player.GetComponent<Fishing>().FishList.Count)
                {
                    menuInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(i).GetComponent<Image>().sprite = player.GetComponent<Fishing>().FishList[i].Sprite;
                    menuInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = "Name: " + player.GetComponent<Fishing>().FishList[i].Name;
                    menuInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = "Value: $" + player.GetComponent<Fishing>().FishList[i].Value;
                }
                else
                {
                    menuInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(i).GetComponent<Image>().sprite = blankIcon;
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
        player.GetComponent<Player>().ResetTiles = true;
        player.GetComponent<Player>().Position = new Vector3(0, 0, 0);
        player.GetComponent<Player>().Hull = 100;
        player.GetComponent<Player>().Money = 0;
        player.GetComponent<Fishing>().FishCaught = 0;
        player.GetComponent<Fishing>().FishList.Clear();

        menuActive = false;
        Destroy(menuInstance.gameObject);
        menuInstance = null;

        player.GetComponent<Player>().MaxSpeed = maxSpeedStorage;
        player.GetComponent<Fishing>().Range = player.GetComponent<Fishing>().RangeStorage;

        SceneManager.LoadScene("TutorialScene");

        Debug.Log("Game Reset.");
    }
}
