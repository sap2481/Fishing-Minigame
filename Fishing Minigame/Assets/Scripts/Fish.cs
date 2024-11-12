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

    /*//Name Arrays (these will not be used until later in development)
    string[] twoRingNames =  { "Sardine", "Clownfish", "Koi" };
    string[] threeRingNames = { "Pufferfish", "Sting Ray", "Squid" };
    string[] fourRingNames = { "Moray Eel", "Hammerhead Shark", "Great White Shark" };*/

    //==== PROPERTIES ====
    public float DifficultyLevel { get { return difficultyLevel; } }
    public float Value { get {  return value; } }
    public string Name { get { return name; } }
    public Sprite Sprite { get { return sprite; } }

    //==== CONSTRUCTOR ====
    public Fish(int ringLevel)
    {
        this.ringLevel = ringLevel;

        switch (ringLevel)
        {
            case 1:
                difficultyLevel = Random.Range(40, 70);
                name = "Clownfish";
                sprite = allfish.clownfish;
                break;

            case 2:
                difficultyLevel = Random.Range(65, 95);
                name = "Pufferfish";
                sprite = allfish.pufferfish;
                break;

            case 3:
                difficultyLevel = Random.Range(90, 120);
                name = "Hammerhead Shark";
                sprite = allfish.hammerhead;
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
