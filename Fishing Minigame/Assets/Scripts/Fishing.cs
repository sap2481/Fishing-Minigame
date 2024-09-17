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
    [SerializeField] GameObject circlePrefab;
    GameObject targetCircle;
    GameObject scalingCircle;
    List<GameObject> circles;

    //Other Values
    bool lineCast;
    int fishCaught;
    int numberOfCasts;
    bool fishOnTheLine;
    float circleScale;
    float scaleSpeed;
    int ringTotal;
    int ringCount;
    float scaleDifference;

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

        //Set Other Values
        lineCast = false;
        fishCaught = 0;
        numberOfCasts = 0;
        fishOnTheLine = false;
        circleScale = 0;
        scaleSpeed = 0;
        ringTotal = 0;
        ringCount = 0;
        scaleDifference = 0;
        circles = new List<GameObject>();
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
            else if (lineCast && bobber != null && notification != null) //If the line is cast, and the player can catch a fish...
            {
                fishOnTheLine = true;
                ringTotal = Random.Range(1, 3);
                ringCount = 1;

                //fishCaught++; //Catch a fish, & destroy relevant fish-catching prefabs

                //Instantiate First Target Circle
                circles.Add(Instantiate(circlePrefab));
                targetCircle = circles[0];
                circleScale = Random.Range(1.0f, 1.5f);
                circles[0].transform.position = bobber.transform.position;

                Color tempColor = new Color(230, 0, 255);
                circles[0].GetComponent<SpriteRenderer>().color = tempColor;

                circles[0].transform.localScale = new Vector3(circleScale, circleScale);

                //Destroy notification & bobber
                Destroy(notification.gameObject);
                notification = null;
                Destroy(bobber.gameObject);
                bobber = null;

                //Instantiate First Scaling Circle
                circles.Add(Instantiate(circlePrefab));
                scalingCircle = circles[1];
                scaleSpeed = Random.Range(0.1f, 0.5f);
                circles[1].transform.position = circles[0].transform.position;
                circles[1].transform.localScale = new Vector3(0.01f, 0.01f);

            }
            else if (fishOnTheLine && ringCount <= ringTotal) //If the player is in the process of catching the fish, and has not completed all rings
            {
                ringCount++;

                //Measure difference in scales between targetCircle and scalingCircle


                targetCircle.SetActive(false); //Turn target circle invisible before initializing a new target circle
                GameObject lastTargetCircle = targetCircle;
                GameObject lastScalingCircle = scalingCircle;

                //Instantiate Next Target Circle
                circles.Add(Instantiate(circlePrefab));
                targetCircle = circles[0 + (ringCount * 2)]; //PROBLEM: Got an Index Out Of Bounds exception here. Will need to re-math this part.
                circleScale = Random.Range(1.0f, 1.5f);
                targetCircle.transform.localScale = new Vector3(lastTargetCircle.transform.localScale.x + circleScale, lastTargetCircle.transform.localScale.y + circleScale);
                targetCircle.transform.position = lastTargetCircle.transform.position;
                targetCircle.GetComponent<SpriteRenderer>().color = new Color(230, 0, 255);
                targetCircle.layer = lastTargetCircle.layer - 1;

                //Instantiate Next Scaling Circle
                circles.Add(Instantiate(circlePrefab));
                scalingCircle = circles[1 + (ringCount * 2)];
                scaleSpeed = Random.Range(0.1f, 0.5f);
                scalingCircle.transform.position = lastScalingCircle.transform.position;
                scalingCircle.transform.localScale = lastScalingCircle.transform.localScale;
                Color tempcolor = scalingCircle.GetComponent<SpriteRenderer>().color;
                tempcolor.g += 100;
                scalingCircle.GetComponent<SpriteRenderer>().color = tempcolor;
                scalingCircle.layer = lastScalingCircle.layer - 1;
            }
            else if (fishOnTheLine && ringCount > ringTotal) //If the player has completed their progression through the rings...
            {
                foreach(GameObject circle in circles) //Destroy all circles & clear them from the list
                {
                    Destroy(circle.gameObject);
                }
                circles.Clear();
            }
            else if (lineCast && bobber != null) //If the line was just reeled in without a catch...
            {
                Destroy(bobber.gameObject);
                bobber = null;
            }
            lineCast = !lineCast;
        }

        if (scalingCircle != null) //If a scalingCircle exists, scale it up
        {
            scalingCircle.transform.localScale += new Vector3(scaleSpeed * Time.deltaTime, scaleSpeed * Time.deltaTime);
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
