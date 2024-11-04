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
    public bool outpostActive;

    GameObject player;
    float maxSpeedStorage;

    //Mouse Fields
    Vector2 mousePosition;
    bool mouseLeftThisFrame;
    bool mouseLeftLastFrame;

    [SerializeField] Collisions collisions;
    Menu menu;

    float speedUpgradeCost;
    float originalSpeed;
    float rangeUpgradeCost;
    float originalRange;
    float hullUpgradeCost;
    float originalHull;

    bool canClick; //A bool to ensure that one button at a time is ever in progress

    //==== START ====
    void Start()
    {
        //Find Player
        player = GameObject.FindGameObjectWithTag("Player");
        menu = GameObject.FindObjectOfType<Menu>();

        mouseLeftLastFrame = false;
        outpostActive = false;

        originalSpeed = 5;
        originalRange = 4;
        originalHull = 100;

        canClick = true;
    }

    //==== UPDATE ====
    void Update()
    {
        //Get Mouse Position & State
        mousePosition = Mouse.current.position.ReadValue();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mouseLeftThisFrame = Mouse.current.leftButton.IsPressed();

        //Check if the player is colliding with the outpost
        if (collisions.CheckColliderCollision(this.gameObject, player))
        {
            player.GetComponent<Player>().StartBounceback(this.gameObject);
        }

        if (collisions.CheckMouseOverlap(mousePosition, this.gameObject) || outpostActive) //If the mouse is overlapping with the outpost...
        {
            //Set range to zero so player can't cast on the outpost
            player.GetComponent<Fishing>().Range = 0;

            if (mouseLeftLastFrame && !mouseLeftThisFrame && !outpostActive && !menu.menuInstance) //If the player clicked on the outpost, open the outpost menu
            {
                outpostActive = true;
                player.GetComponent<Player>().Hull = 100; //Because the player docked at an outpost, reset health to full
                opMenuInstance = Instantiate(opMenuPrefab);
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(xOut);
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(BuyButton);
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(SellButton);
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(QuestButton);
                SellButton();
            }
        }
        else
        {
            //Reset fishing range
            player.GetComponent<Fishing>().Range = player.GetComponent<Fishing>().RangeStorage;
        }

        //Update Money the Player Has
        if (outpostActive) { opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(9).GetComponent<TMP_Text>().text = "Money: $" + player.GetComponent<Player>().Money; }

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
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).gameObject.SetActive(true); //Set Buy panel to active <--
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(6).gameObject.SetActive(false); //Set Quest panel to inactive

        //Calculate Upgrade Costs
        speedUpgradeCost = 200 + ((player.GetComponent<Player>().MaxSpeed - originalSpeed) * 200);
        rangeUpgradeCost = 300 + ((player.GetComponent<Fishing>().RangeStorage - originalRange) * 250);
        hullUpgradeCost = 400 + ((player.GetComponent<Player>().Hull - originalHull) * 6);

        //Fill Out Costs
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = "Cost: $" + speedUpgradeCost;
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = "Cost: $" + rangeUpgradeCost;
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(3).GetChild(1).GetComponent<TMP_Text>().text = "Cost: $" + hullUpgradeCost;

        //If the player has enough money for each upgrade, add a listener to the respective upgrade's button
        if (player.GetComponent<Player>().Money >= speedUpgradeCost)
        {
            opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(1).GetChild(2).GetComponent<Image>().color = Color.green;
            opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { UpgradeSpeed(speedUpgradeCost); });
        }
        else
        {
            opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(1).GetChild(2).GetComponent<Image>().color = Color.red;
            opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(1).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        }

        if (player.GetComponent<Player>().Money >= rangeUpgradeCost)
        {
            opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(2).GetChild(2).GetComponent<Image>().color = Color.green;
            opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(2).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { UpgradeRange(rangeUpgradeCost); });
        }
        else
        {
            opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(2).GetChild(2).GetComponent<Image>().color = Color.red;
            opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(2).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        }

        if (player.GetComponent<Player>().Money >= hullUpgradeCost)
        {
            opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(3).GetChild(2).GetComponent<Image>().color = Color.green;
            opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(3).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { UpgradeHull(hullUpgradeCost); });
        }
        else
        {
            opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(3).GetChild(2).GetComponent<Image>().color = Color.red;
            opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetChild(3).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
    public void SellButton() //Change the menu to the Sell screen
    {
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).gameObject.SetActive(true); //Set Sell panel to active <--
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).gameObject.SetActive(false); //Set Buy panel to inactive
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(6).gameObject.SetActive(false); //Set Quest panel to inactive

        for (int i = 0; i < 6; i++)
        {
            if (player.GetComponent<Fishing>().FishList.Count == 0) //If there are no fish to sell, display the Empty Cargo Hold message
            {
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).gameObject.SetActive(false);
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(6).gameObject.SetActive(true);
            }
            else
            {
                if (i < player.GetComponent<Fishing>().FishList.Count) //If there's a fish to go in the sell slot, set it to active & fill it with relevant info
                {
                    Fish thisFish = player.GetComponent<Fishing>().FishList[i];

                    opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).gameObject.SetActive(true);
                    opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = thisFish.Name;
                    opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = "$" + thisFish.Value;
                    opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).GetChild(2).GetComponent<Image>().sprite = thisFish.Sprite;
                    opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).GetChild(3).GetComponent<Button>().onClick.AddListener
                        (delegate { SellTheFish(player.GetComponent<Fishing>().FishList, player.GetComponent<Fishing>().FishList.FindIndex(a => a.Value == thisFish.Value)); });
                }
                else //If there is not a fish to go in the slot, set the slot to inactive
                {
                    opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).gameObject.SetActive(false);
                    opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(i).GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();
                }
                opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).GetChild(6).gameObject.SetActive(false); //Set Empty Cargo Hold message to inactive
            }
        }
    }
    public void QuestButton()
    {
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(4).gameObject.SetActive(false); //Set Sell panel to inactive
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(5).gameObject.SetActive(false); //Set Buy panel to inactive
        opMenuInstance.transform.GetChild(0).GetChild(0).GetChild(6).gameObject.SetActive(true); //Set Quest panel to active <--
    }

    public void SellTheFish(List<Fish> fishList, int index)
    {
        if (canClick)
        {
            if (index == -1) { index = 0; }
            player.GetComponent<Player>().Money += fishList[index].Value;
            Debug.Log("I just sold " + fishList[index].Name + " for " + fishList[index].Value);
            player.GetComponent<Fishing>().FishList.RemoveAt(index);
            SellButton();
            StartCoroutine(WaitForButtonClick(0.25f));
        }
    }

    public void UpgradeSpeed(float cost)
    {
        if (canClick)
        {
            player.GetComponent<Player>().MaxSpeed++;
            player.GetComponent<Player>().Money -= cost;
            BuyButton();
            StartCoroutine(WaitForButtonClick(0.25f));
        }
    }
    public void UpgradeRange(float cost)
    {
        if (canClick)
        {
            player.GetComponent<Fishing>().RangeStorage++;
            player.GetComponent<Player>().Money -= cost;
            BuyButton();
            StartCoroutine(WaitForButtonClick(0.25f));
        }
    }
    public void UpgradeHull(float cost)
    {
        if (canClick)
        {
            player.GetComponent<Player>().Hull += 50;
            player.GetComponent<Player>().MaxHull += 50;
            player.GetComponent<Player>().Money -= cost;
            BuyButton();
            StartCoroutine(WaitForButtonClick(0.25f));
        }
    }
    private IEnumerator WaitForButtonClick(float waitTime)
    {
        canClick = false;
        yield return new WaitForSeconds(waitTime);
        canClick = true;
    } 
}
