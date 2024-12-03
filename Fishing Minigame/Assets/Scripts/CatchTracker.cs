using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatchTracker : MonoBehaviour
{
    //==== FIELDS ====
    [SerializeField] GameObject catchTracker_Panel;
    [SerializeField] TMP_Text catchTracker_Text;

    Fishing fishingManager;

    int prevFishCaught;
    
    //==== START ====
    void Start()
    {
        fishingManager = GameObject.FindGameObjectWithTag("Player").GetComponent<Fishing>();

        catchTracker_Text.text = "Total Fish Caught: 0\nFish in Cargo Hold: 0";
    }

    //==== UPDATE ====
    void Update()
    {
        catchTracker_Text.text = "Total Fish Caught: " + fishingManager.FishCaught + "\nFish In Cargo Hold: " + fishingManager.FishList.Count;

        if (prevFishCaught != fishingManager.FishCaught) //If the player just caught a fish, turn the panel text green.
        {
            catchTracker_Text.color = Color.green;
        }

        if (fishingManager.FishFail) //If the player just failed to catch a fish, turn the panel text red
        {
            catchTracker_Text.color = Color.red;
            fishingManager.FishFail = false;
        }

        //Restore Panel Text Color
        if (catchTracker_Text.color.g < 255 || catchTracker_Text.color.r < 255 || catchTracker_Text.color.b < 255)
        {
            catchTracker_Text.color = new Color(catchTracker_Text.color.r + (1f * Time.deltaTime), catchTracker_Text.color.g + (1f * Time.deltaTime), catchTracker_Text.color.b + (1f * Time.deltaTime));
        }

        prevFishCaught = fishingManager.FishCaught;
    }
}
