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

        catchTracker_Text.text = "Fish Caught: 0";
    }

    //==== UPDATE ====
    void Update()
    {
        catchTracker_Text.text = "Fish Caught: " + fishingManager.FishCaught;

        if (prevFishCaught != fishingManager.FishCaught) //If the player just caught a fish, turn the panel green.
        {
            catchTracker_Panel.GetComponent<Image>().color = Color.green;
        }

        if (fishingManager.FishFail) //If the player just failed to catch a fish, turn the panel red
        {
            catchTracker_Panel.GetComponent<Image>().color = Color.red;
            fishingManager.FishFail = false;
        }

        //Restore Panel Color
        if (catchTracker_Panel.GetComponent<Image>().color.g > 0 || catchTracker_Panel.GetComponent<Image>().color.r > 0)
        {
            catchTracker_Panel.GetComponent<Image>().color = new Color(catchTracker_Panel.GetComponent<Image>().color.r - (1f * Time.deltaTime), catchTracker_Panel.GetComponent<Image>().color.g - (1f * Time.deltaTime), 0f);
        }

        prevFishCaught = fishingManager.FishCaught;
    }
}
