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

    [SerializeField] GameObject dialogueBoxPrefab;
    GameObject dialogueBox;
    TMP_Text textBox;

    bool canClick;
    bool mouseLeftThisFrame;
    bool mouseLeftLastFrame;

    GameObject player;
    Fishing playerFishing;

    bool checkIfFishing;
    
    //==== START ====
    void Start()
    {
        dialogueBox = Instantiate(dialogueBoxPrefab);
        textBox = dialogueBox.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        increment = 0;

        canClick = true;
        mouseLeftLastFrame = false;
        mouseLeftThisFrame = false;

        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Fishing>().enabled = false;
        checkIfFishing = false;
    }

    //==== UPDATE ====
    void Update()
    {
        mouseLeftThisFrame = Mouse.current.leftButton.IsPressed();
        if (canClick && !mouseLeftThisFrame && mouseLeftLastFrame) //If a single click was just completed, change the dialogue
        {
            if (increment == 12) { increment = 6; }
            else if (increment == 2 || increment == 4 || increment == 10 || increment == 11) { StartCoroutine(WaitToCheck(0.1f)); } //Delays a check by 0.1 seconds to allow the fishing mechanic to catch up to the tutorial
            else { increment++; }
            StartCoroutine(WaitToClick(0.5f));
        }
        
        switch (increment)
        {
            case 0:
                textBox.text = "You must be the new angler! I'm Fork. Fork the Fisherman. Don't ask.";
                break;

            case 1:
                textBox.text = "Before you go gallivanting off by yourself, you need to learn to fish. This is a fishing game, after all, and I was made explicitly to teach you. It's a dull existence.";
                break;

            case 2:
                textBox.text = "See that crosshair? It turns green when you're close enough to cast your line, and red when you're too far out. It's ideal to fish at whirlpools, but click anywhere your crosshair is green for now to cast your line.";
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
                break;

            case 6:
                if (player.GetComponent<Fishing>().crosshair.activeSelf == true) { increment = 10; } //Jump to increment 10 if the player screws up getting a fish on the line
                textBox.text = "Good! Now, You see that green area there? Try to click when the moving bar is overlapping it, or as close as you can get. A lot of clicking, I know.";
                break;

            case 7:
                textBox.text = "Now try it again...";
                break;

            case 8:
                if (player.GetComponent<Fishing>().FishFail) { increment = 11; } //if the player fails to catch a fish, jump to increment 11.
                textBox.text = "Good job! You caught a fish! That's pretty much the entire game. Use WASD or Arrow keys to move, don't bump into things, and click on the outpost (my lovely abode) to buy, sell, or quest. Good luck, have fun, and let the developer know if you have any issues, 'cause I sure can't do anything about them.";
                break;

            case 9:
                SceneManager.LoadScene("StarterScene");
                break;

            case 10:
                textBox.text = "Did you miss the notification? Or did you click before it even came up? Don't do either of those things. Try again. And be sure it's on the whirlpool, or you're gonna have to wait a lot longer.";
                break;

            case 11:
                textBox.text = "You didn't catch it. Okay, that's fine, let's try again. Cast your line - yes, please, at the whirlpool.";
                break;

            case 12:
                //This plays after either case 10 or 11
                textBox.text = "This dialogue is gonna keep looping, you know. You're not unlocking anything new by failing. Except this, I suppose. Whoop-dee-doo.";
                player.GetComponent<Fishing>().FishFail = false;
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

    private IEnumerator WaitToCheck(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (player.GetComponent<Fishing>().crosshair.activeSelf == true) { checkIfFishing = false; Debug.Log("Player is NOT fishing"); } 
        else 
        { 
            checkIfFishing = true; 
            Debug.Log("Player IS Fishing");
            if (increment == 10) { increment = 12; }
            else { increment++; }
            Debug.Log(player.GetComponent<Fishing>().waitingTime);
            if (increment != 3) { StartCoroutine(player.GetComponent<Fishing>().WaitForFish(player.GetComponent<Fishing>().waitingTime, player.GetComponent<Fishing>().numberOfCasts)); }
        }
    }
}
