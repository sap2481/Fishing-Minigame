using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    //==== FIELDS ====
    int ringLevel; //This will range from 2 to 4, and determine how many rings is required to catch a fish
    float difficultyLevel; //This will determine how much rotational difference the player must remain under to successfully catch this fish
    float value; //How much money the fish is worth
    string name; //What is the fish called?
    Sprite sprite; //The relevant fish sprite (this will not come into play until Lydia gives me art)

    //Sprites
    AllFish allfish = GameObject.FindObjectOfType<AllFish>();

    //==== PROPERTIES ====
    public float DifficultyLevel { get { return difficultyLevel; } }
    public float Value { get {  return value; } }
    public string Name { get { return name; } }
    public Sprite Sprite { get { return sprite; } }

    //==== CONSTRUCTOR ====
    public Fish(int ringLevel)
    {
        this.ringLevel = ringLevel;
        int fishDecider = Random.Range(1, 4);

        switch (ringLevel)
        {
            case 1:
                difficultyLevel = Random.Range(40, 70);
                if (fishDecider == 1)
                {
                    name = "Clownfish";
                    sprite = allfish.clownfish;
                }
                else if (fishDecider == 2)
                {
                    name = "Koi";
                    sprite = allfish.koi;
                }
                else if (fishDecider == 3)
                {
                    name = "Sardine";
                    sprite = allfish.sardine;
                }
                break;

            case 2:
                difficultyLevel = Random.Range(65, 95);
                if (fishDecider == 1)
                {
                    name = "Pufferfish";
                    sprite = allfish.pufferfish;
                }
                else if (fishDecider == 2)
                {
                    name = "Stingray";
                    sprite = allfish.stingray;
                }
                else if (fishDecider == 3)
                {
                    name = "Eel";
                    sprite = allfish.eel;
                }
                break;

            case 3:
                difficultyLevel = Random.Range(90, 120);
                if (fishDecider == 1)
                {
                    name = "Hammerhead Shark";
                    sprite = allfish.hammerhead;
                }
                else if (fishDecider == 2)
                {
                    name = "Squid";
                    sprite = allfish.squid;
                }
                else if (fishDecider == 3)
                {
                    name = "Great White Shark";
                    sprite = allfish.greatwhite;
                }
                break;
        }

        value = difficultyLevel;
    }
    
    //==== START ====
    void Start()
    {
        
    }

    //==== UPDATE ====
    void Update()
    {
        
    }
}
