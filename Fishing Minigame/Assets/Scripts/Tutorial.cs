using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    //==== FIELDS ====
    public int increment; //Increment to determine dialogue within the dialogue box
    int lastIncrement; //What was the last dialogue that played?

    [SerializeField] GameObject dialogueBoxPrefab;
    GameObject dialogueBox;
    TMP_Text textBox;

    bool canClick;
    bool mouseLeftThisFrame;
    bool mouseLeftLastFrame;

    GameObject player;
    Fishing playerFishing;

    bool waitForFishStarted;
    bool numCastsIncremented;
    
    //==== START ====
    void Start()
    {
        dialogueBox = Instantiate(dialogueBoxPrefab);
        textBox = dialogueBox.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        increment = 0;
        lastIncrement = 0;

        canClick = true;
        mouseLeftLastFrame = false;
        mouseLeftThisFrame = false;

        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Fishing>().enabled = false;

        waitForFishStarted = false;
        numCastsIncremented = false;
    }

    //==== UPDATE ====
    void Update()
    {
        mouseLeftThisFrame = Mouse.current.leftButton.IsPressed();
        if (canClick && !mouseLeftThisFrame && mouseLeftLastFrame) //If a single click was just completed, change the dialogue
        {
            if (increment == 10) { increment = 12; }
            else if (increment == 12) { increment = 6; }
            else { increment++; }
            StartCoroutine(WaitToClick(0.5f));
        }
        
        switch (increment)
        {
            case 0:
                textBox.text = "You must be the new fisherman! I'm Fork. Fork the Fisherman. Yeah, I know.";
                break;

            case 1:
                textBox.text = "Before you go gallivanting off by yourself, you need to learn to fish. This is a fishing game, after all, and I was made explicitly to teach you. It's a dull existence.";
                break;

            case 2:
                textBox.text = "See that crosshair? It turns green when you're close enough to cast your line, and red when you're too far out. It's ideal to fish at whirlpools, but click anywhere for now to cast your line.";
                player.GetComponent<Fishing>().enabled = true;
                break;

            case 3:
                textBox.text = "Good job! When the crosshair is yellow, it means your line is cast, and it's time to wait until you see a notification appear. When it does, you have a fish on your line, and need to click quickly. Click now to retract your line.";
                break;

            case 4:
                textBox.text = "You ready to try it? Remember, wait for the notification to appear. Now, click to cast your line... preferably on the whirlpool, so we're not here all day. Be patient, but vigilant!";
                break;

            case 5:
                textBox.text = "Why am I a fork, you ask? That's a damn good question. Could you ask the developer and see what he says? He's the one who made me this way.";
                /*if (!numCastsIncremented)
                {
                    player.GetComponent<Fishing>().numberOfCasts++;
                    Debug.Log(player.GetComponent<Fishing>().numberOfCasts);
                    numCastsIncremented = true;
                }*/
                //if (!waitForFishStarted) { StartCoroutine(player.GetComponent<Fishing>().WaitForFish(Random.Range(3.0f, 5.0f), player.GetComponent<Fishing>().numberOfCasts)); waitForFishStarted = true; }
                break;

            case 6:
                waitForFishStarted = false;
                numCastsIncremented = false;
                if (player.GetComponent<Fishing>().crosshair.active == true) { increment = 10; } //Jump to increment 10 if the player screws up getting a fish on the line
                textBox.text = "Good! Now, You see that blue area there? Try to click when the moving white bar is overlapping with that blue one. A lot of clicking, I know.";
                break;

            case 7:
                textBox.text = "Now try it again...";
                break;

            case 8:
                if (player.GetComponent<Fishing>().FishFail) { increment = 11; } //if the player fails to catch a fish, jump to increment 11.
                textBox.text = "Good job! You caught a fish! That's pretty much the entire game. Good luck, have fun, come see me if you need anything. It's not like I'm physically capable of going anywhere.";
                break;

            case 9:
                SceneManager.LoadScene("StarterScene");
                break;

            case 10:
                waitForFishStarted = false;
                numCastsIncremented = false;
                textBox.text = "Did you miss the notification? Or did you click before it even came up? Don't do either of those things. Try again. And be sure it's on the whirlpool, or you're gonna have to wait a lot longer.";
                break;

            case 11:
                waitForFishStarted = false;
                numCastsIncremented = false;
                textBox.text = "You didn't catch it. Okay, that's fine, let's try again. Cast your line - yes, please, at the whirlpool.";
                break;

            case 12:
                //This plays after either case 10 or 11
                textBox.text = "This dialogue is gonna keep looping, you know. You're not unlocking anything new by failing. Except this, I suppose. Whoop-dee-doo.";
                /*if (!numCastsIncremented) { 
                    player.GetComponent<Fishing>().numberOfCasts++; 
                    Debug.Log(player.GetComponent<Fishing>().numberOfCasts);  
                    numCastsIncremented = true; 
                }
                if (!waitForFishStarted) { StartCoroutine(player.GetComponent<Fishing>().WaitForFish(Random.Range(3.0f, 5.0f), player.GetComponent<Fishing>().numberOfCasts)); waitForFishStarted = true; }*/
                break;

            default:
                textBox.text = "Oops! Something's fucking broken cuz the switch statement went to default. You should fix that, Sam!";
                break;
        }

        mouseLeftLastFrame = mouseLeftThisFrame;
    }

    //==== METHODS & COROUTINES ====
    private IEnumerator WaitToClick(float waitTime)
    {
        canClick = false;
        yield return new WaitForSeconds(waitTime);
        canClick = true;
    }
}
