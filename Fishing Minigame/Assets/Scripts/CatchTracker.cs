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

        if (prevFishCaught != fishingManager.FishCaught) //Turn panel green when a player catches a fish
        {
            catchTracker_Panel.GetComponent<Image>().color = Color.green;
        }

        //Restore Panel Color
        if (catchTracker_Panel.GetComponent<Image>().color.g > 0)
        {
            catchTracker_Panel.GetComponent<Image>().color = new Color(0f, catchTracker_Panel.GetComponent<Image>().color.g - 0.001f, 0f);
        }

        prevFishCaught = fishingManager.FishCaught;
    }
}