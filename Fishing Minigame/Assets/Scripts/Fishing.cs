using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//As of now, this script is planned to be attached to the player.

public class Fishing : MonoBehaviour
{
    //==== FIELDS ====
    //Mouse fields
    bool mouseLeftThisFrame;
    bool mouseLeftLastFrame;
    //bool mouseRightThisFrame; //See below
    //bool mouseRightLastFrame; //Not sure if I'm gonna need these - as far as I know, fishing will be done primarily through left-click. Good to have for now tho, just in case.
    Vector2 mousePosition;

    //Prefabs & Related GameObjects
    [SerializeField] GameObject crosshairPrefab;
    GameObject crosshair;
    GameObject bobber; //Crosshair but a different color
    [SerializeField] GameObject notificationPrefab;
    GameObject notification;

    //Other Values
    bool lineCast;
    int fishCaught;
    int numberOfCasts;

    //==== START ====
    void Start()
    {
        mouseLeftLastFrame = false;

        //Instantiate Crosshair
        crosshair = Instantiate(crosshairPrefab);
        crosshair.transform.position = mousePosition;

        //Set Bobber & Notification to Null
        bobber = null;
        notification = null;

        lineCast = false;
        fishCaught = 0;
        numberOfCasts = 0;
    }

    //==== UPDATE ====
    void Update()
    {
        mouseLeftThisFrame = Mouse.current.leftButton.IsPressed(); //Get current mouse-press state
        crosshair.transform.position = mousePosition; //Set crosshair to mouse position

        if (lineCast) //if the line is cast, don't have the crosshair moving around with the mouse (set it invisible)
        {
            crosshair.SetActive(false);
        }
        else
        {
            crosshair.SetActive(true);
        }

        //Get Mouse Position
        mousePosition = Mouse.current.position.ReadValue();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (!mouseLeftThisFrame && mouseLeftLastFrame) //If the mouse was just clicked...
        {
            if (!lineCast && bobber == null) //If the line was just cast...
            {
                bobber = Instantiate(crosshairPrefab);
                bobber.transform.position = mousePosition;
                bobber.GetComponent<SpriteRenderer>().color = Color.red;
                numberOfCasts++;
                StartCoroutine(WaitForFish(Random.Range(3.0f, 5.0f), numberOfCasts));
            }
            else if (lineCast && bobber != null && notification != null) //If the line is cast, and the player is catching a fish...
            {
                fishCaught++; //Catch a fish, & destroy relevant fish-catching prefabs
                Destroy(notification.gameObject);
                notification = null;
                Destroy(bobber.gameObject);
                bobber = null;
            }
            else if (lineCast && bobber != null) //If the line was just reeled in without a catch...
            {
                Destroy(bobber.gameObject);
                bobber = null;
            }
            lineCast = !lineCast;
        }

        mouseLeftLastFrame = mouseLeftThisFrame;
    }

    //==== METHODS & COROUTINES ====
    private IEnumerator WaitForFish(float waitTime, int currentCastNum)
    {
        yield return new WaitForSeconds(waitTime);
        if (lineCast && currentCastNum == numberOfCasts) //If the line is still cast after the wait time has passed, and is still on the current cast...
        {
            notification = Instantiate(notificationPrefab);
            notification.transform.position = new Vector3(bobber.transform.position.x, bobber.transform.position.y + 1.5f, bobber.transform.position.z);
            yield return new WaitForSeconds(1.0f);
            if (notification != null) //If the notification hasn't been destroyed yet (aka if the player hasn't caught the fish), destroy the notification.
            {
                Destroy(notification.gameObject);
                notification = null;
            }
        }
    }
}
