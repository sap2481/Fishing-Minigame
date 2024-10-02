using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//As of now, this script is planned to be attached to the player.

public class Fishing : MonoBehaviour
{
    //==== FIELDS ====
    //Mouse Fields
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

    //Mechanic-Related Fields
    float circleScale; //The scale of the target circle
    float scaleSpeed; //The speed at which the scaling circle scales upwards
    int ringTotal; //How many total rings this particular cast requires the player to go through
    int ringCount; //How many rings has the player currently gone through?
    float scaleDifference; //The total difference in scales between target circles and their respective scaling circles.

    //Player
    GameObject player;

    //Other Values
    bool lineCast; //Is the line cast?
    int fishCaught; //Did the player catch a fish?
    int numberOfCasts; //Make sure that each cast is independent of each other cast (aka make sure that the timing of a previous cast doesn't carry over to the next)
    bool fishOnTheLine; //Is a fish on the line?
    float range; //The range at which a player can cast their line away from their ship
    float distance; //How far is the player attempting to cast?

    //==== PROPERTIES ====
    public int FishCaught { get { return fishCaught; } }

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

        //Find Player
        player = GameObject.FindGameObjectWithTag("Player");

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
        range = 4;
        distance = 0;
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

        //Get Crosshair or Bobber Distance from Player
        if (lineCast && bobber != null) { distance = Vector3.Distance(bobber.transform.position, player.transform.position); } //Bobber Distance if Line is Cast
        else if (lineCast && fishOnTheLine) { distance = Vector3.Distance(circles[0].transform.position, player.transform.position); } //Mechanic Distance if Mechanic In Progress
        else { distance = Vector3.Distance(mousePosition, player.transform.position); } //Crosshair Distance is Line is Not Cast

        if (distance < range) //If the bobber is within range of the player, meaning the player can cast...
        {
            crosshair.GetComponent<SpriteRenderer>().color = Color.green;

            if (!mouseLeftThisFrame && mouseLeftLastFrame) //If the mouse was just clicked...
            {
                if (!lineCast && bobber == null) //If the line was just cast...
                {
                    lineCast = true;
                    bobber = Instantiate(crosshairPrefab);
                    bobber.transform.position = mousePosition;
                    bobber.GetComponent<SpriteRenderer>().color = Color.yellow;
                    numberOfCasts++;
                    StartCoroutine(WaitForFish(Random.Range(3.0f, 5.0f), numberOfCasts));
                }
                else if (lineCast && bobber != null && notification != null) //If the line is cast, and the player can catch a fish...
                {
                    fishOnTheLine = true;
                    ringTotal = Random.Range(2, 4);
                    ringCount = 1;

                    //Instantiate First Target Circle
                    circles.Add(Instantiate(circlePrefab));
                    targetCircle = circles[0];
                    circleScale = Random.Range(1.0f, 1.5f);
                    targetCircle.transform.position = bobber.transform.position;
                    targetCircle.transform.localScale = new Vector3(circleScale, circleScale);
                    targetCircle.GetComponent<SpriteRenderer>().sortingOrder = ringTotal + 1;

                    Color tempColor = new Color(230, 0, 255);
                    targetCircle.GetComponent<SpriteRenderer>().color = tempColor;

                    //Destroy notification & bobber
                    Destroy(notification.gameObject);
                    notification = null;
                    Destroy(bobber.gameObject);
                    bobber = null;

                    //Instantiate First Scaling Circle
                    circles.Add(Instantiate(circlePrefab));
                    scalingCircle = circles[1];
                    scaleSpeed = Random.Range(0.5f, 1f);
                    scalingCircle.transform.position = targetCircle.transform.position;
                    scalingCircle.transform.localScale = new Vector3(0.01f, 0.01f);
                    scalingCircle.GetComponent<SpriteRenderer>().sortingOrder = ringTotal + 1;

                }
                else if (fishOnTheLine && ringCount <= ringTotal) //If the player is in the process of catching the fish...
                {
                    ringCount++;

                    if (ringCount > ringTotal) //If the player has completed their progression through the rings...
                    {
                        foreach (GameObject circle in circles) //Destroy all circles & clear them from the list
                        {
                            Destroy(circle.gameObject);
                        }
                        circles.Clear();
                        lineCast = false;
                        fishOnTheLine = false;

                        //Determine a catch
                        if (scaleDifference < 0.5f) //0.5f is a temp value, and will be replaced by a random value determined by the fish's difficulty
                        {
                            fishCaught++;
                            Debug.Log("Fish Caught! Scale Difference: " + scaleDifference);
                        }
                        else
                        {
                            Debug.Log("Failed; the fish got away. Scale Difference: " + scaleDifference);
                        }
                    }
                    else //If the player still has rings to progress to...
                    {
                        //Measure difference in scales between targetCircle and scalingCircle
                        float thisDifference = (targetCircle.transform.localScale.x - scalingCircle.transform.localScale.x);
                        if (thisDifference < 0) { thisDifference *= -1; }
                        scaleDifference += thisDifference;

                        targetCircle.SetActive(false); //Turn target circle invisible before initializing a new target circle
                        GameObject lastTargetCircle = targetCircle;
                        GameObject lastScalingCircle = scalingCircle;

                        //Set Last Scaling Circle Color 
                        if (ringCount == 2) { scalingCircle.GetComponent<SpriteRenderer>().color = Color.red; }
                        else if (ringCount == 3) { scalingCircle.GetComponent<SpriteRenderer>().color = Color.yellow; }
                        else if (ringCount == 4) { scalingCircle.GetComponent<SpriteRenderer>().color = Color.green; }

                        //Instantiate Next Target Circle
                        circles.Add(Instantiate(circlePrefab));
                        int tcIndex = circles.IndexOf(targetCircle);
                        targetCircle = circles[tcIndex + 2];
                        targetCircle.GetComponent<SpriteRenderer>().sortingOrder = lastTargetCircle.GetComponent<SpriteRenderer>().sortingOrder - 1;
                        circleScale = Random.Range(1.0f, 1.5f);
                        targetCircle.transform.localScale = new Vector3(lastTargetCircle.transform.localScale.x + circleScale, lastTargetCircle.transform.localScale.y + circleScale);
                        targetCircle.transform.position = lastTargetCircle.transform.position;
                        targetCircle.GetComponent<SpriteRenderer>().color = new Color(230, 0, 255);

                        //Instantiate Next Scaling Circle
                        circles.Add(Instantiate(circlePrefab));
                        int scIndex = circles.IndexOf(scalingCircle);
                        scalingCircle = circles[scIndex + 2];
                        scalingCircle.GetComponent<SpriteRenderer>().sortingOrder = lastScalingCircle.GetComponent<SpriteRenderer>().sortingOrder - 1;
                        scaleSpeed = Random.Range(0.5f, 1f);
                        scalingCircle.transform.position = lastScalingCircle.transform.position;
                        scalingCircle.transform.localScale = lastScalingCircle.transform.localScale;
                        Color tempcolor = scalingCircle.GetComponent<SpriteRenderer>().color;
                        tempcolor.g += 100;
                        scalingCircle.GetComponent<SpriteRenderer>().color = tempcolor;
                    }
                }
                else if (lineCast && bobber != null && !fishOnTheLine) //If the line was just reeled in without a catch...
                {
                    Destroy(bobber.gameObject);
                    bobber = null;
                    lineCast = false;
                }
            }
        }
        else //If the bobber is out of range of the player, meaning they cannot cast or cannot hold their line...
        {
            crosshair.GetComponent <SpriteRenderer>().color = Color.red;

            if (lineCast) //If the line is cast, recall it
            {
                if (bobber != null)
                {
                    Destroy(bobber.gameObject);
                    bobber = null;
                }
                foreach (GameObject circle in circles) { Destroy(circle.gameObject); }
                circles.Clear();
                lineCast = false;
                fishOnTheLine = false;
            }
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
