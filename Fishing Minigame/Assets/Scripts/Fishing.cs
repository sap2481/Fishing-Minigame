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

    //First-Pass Mechanic Related Fields
    float circleScale; //The scale of the target circle
    float scaleSpeed; //The speed at which the scaling circle scales upwards
    int ringTotal; //How many total rings this particular cast requires the player to go through - REQUIRED FOR PRIMARY MECHANIC    
    int ringCount; //How many rings has the player currently gone through? - REQUIRED FOR PRIMARY MECHANIC
    float scaleDifference; //The total difference in scales between target circles and their respective scaling circles.
    [SerializeField] GameObject circlePrefab;
    GameObject targetCircle;
    GameObject scalingCircle;
    List<GameObject> circles;

    //Primary Mechanic Related Fields
    [SerializeField] GameObject conePrefab;
    List<GameObject> targetCones;
    List<GameObject> movingCones;
    float rotationSpeed; //How fast the current moving cone is rotating
    float rotationDifference; //Difference in rotation between target cone and current moving cone

    //Player
    GameObject player;

    //Other Values
    bool lineCast; //Is the line cast?
    int fishCaught; //Did the player catch a fish?
    int numberOfCasts; //Make sure that each cast is independent of each other cast (aka make sure that the timing of a previous cast doesn't carry over to the next)
    bool fishOnTheLine; //Is a fish on the line?
    float range; //The range at which a player can cast their line away from their ship
    float distance; //How far is the player attempting to cast?
    bool fishFail; //Declare if the player failed to catch a fish (used for the CatchTracker)

    //==== PROPERTIES ====
    public int FishCaught { get { return fishCaught; } }
    public bool FishFail { get { return fishFail; } set { fishFail = value; } }

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

        //Instantiate Lists
        targetCones = new List<GameObject>();
        movingCones = new List<GameObject>();   

        //Initialize Other Values
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
        fishFail = false;
        rotationSpeed = 0;
        rotationDifference = 0;
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
                //Is the line cast, is there a bobber, is there a notification, and is there a fish on the line?
                switch (lineCast, bobber == null, notification == null, fishOnTheLine)
                {
                    //Line has not been cast, bobber & notification are null, and there is not a fish on the line
                    //(The player is casting their line)
                    case (false, true, true, false):
                        
                        lineCast = true;
                        bobber = Instantiate(crosshairPrefab);
                        bobber.transform.position = mousePosition;
                        bobber.GetComponent<SpriteRenderer>().color = Color.yellow;
                        numberOfCasts++;
                        StartCoroutine(WaitForFish(Random.Range(3.0f, 5.0f), numberOfCasts));
                        
                        break;
                    
                    //Line has been cast, there is a bobber and a notification, but a fish is not yet on the line
                    //(The player is beginning to catch a fish)
                    case (true, false, false, false):
                        
                        fishOnTheLine = true;
                        ringTotal = 1; //This will later be a random range, most likely from 2 to 4
                        ringCount = 1;

                        //Instantiate First Target Ring
                        circles.Add(Instantiate(circlePrefab));
                        circles[0].transform.position = bobber.transform.position;
                        circles[0].transform.localScale = new Vector3(1.65f, 1.65f);
                        circles[0].GetComponent<SpriteRenderer>().color = Color.gray;
                        circles[0].GetComponent<SpriteRenderer>().sortingOrder = ringTotal + 1;

                        //Destroy notification & bobber
                        Destroy(notification.gameObject);
                        notification = null;
                        Destroy(bobber.gameObject);
                        bobber = null;

                        //Instantiate First Target Cone
                        targetCones.Add(Instantiate(conePrefab));
                        targetCones[0].transform.position = circles[0].transform.position;
                        float targetRot = Random.Range(-179, 179);
                        targetCones[0].transform.eulerAngles = new Vector3(0, 0, targetRot);
                        targetCones[0].transform.localScale = new Vector3(circles[0].transform.localScale.x / 22.5f, circles[0].transform.localScale.y / 22.5f);
                        targetCones[0].GetComponent<SpriteRenderer>().color = Color.cyan;
                        targetCones[0].GetComponent<SpriteRenderer>().sortingOrder = ringTotal + 2;

                        //Instantiate First Moving Cone
                        movingCones.Add(Instantiate(conePrefab));
                        movingCones[0].transform.position = circles[0].transform.position;
                        float startRot = Random.Range(-179, 179);
                        movingCones[0].transform.eulerAngles = new Vector3(0, 0, startRot);
                        movingCones[0].transform.localScale = new Vector3(circles[0].transform.localScale.x / 22.5f, circles[0].transform.localScale.y / 22.5f);
                        movingCones[0].GetComponent<SpriteRenderer>().color = Color.white;
                        movingCones[0].GetComponent<SpriteRenderer>().sortingOrder = ringTotal + 3;
                        rotationSpeed = Random.Range(250, 500);

                        //Instantiate Bullseye Center
                        circles.Add(Instantiate(circlePrefab));
                        circles[1].transform.position = circles[0].transform.position;
                        circles[1].transform.localScale = new Vector3(1.15f, 1.15f);
                        circles[1].GetComponent<SpriteRenderer>().color = Color.black;
                        circles[1].GetComponent<SpriteRenderer>().sortingOrder = ringTotal + 4;

                        break;

                    //Line is cast, there is no bobber, there is no notification, and there is a fish on the line
                    //(The player is moving through the mechanic, trying to catch the fish)
                    case (true, true, true, true):
                        
                        ringCount++;

                        //Measure rotation difference between target cone and moving cone
                        float thisDifference = Quaternion.Angle(movingCones[movingCones.Count - 1].transform.rotation, targetCones[targetCones.Count - 1].transform.rotation);
                        rotationDifference += thisDifference;

                        if (ringCount > ringTotal) //If the player has completed their progression through the rings...
                        {
                            lineCast = false;
                            fishOnTheLine = false;

                            //Destroy All Mechanic Items
                            foreach (GameObject tCone in targetCones) { Destroy(tCone.gameObject); }
                            targetCones.Clear();
                            foreach (GameObject mCone in movingCones) { Destroy(mCone.gameObject); }
                            movingCones.Clear();
                            foreach (GameObject circle in circles) { Destroy(circle.gameObject); }
                            circles.Clear();

                            //Determine a catch
                            if (rotationDifference < (25.0f * ringTotal)) //this difficulty level will later be changed depending on the fish ur catching
                            {
                                fishCaught++;
                                Debug.Log("Fish Caught with Rotation Difference " + rotationDifference);
                            }
                            else
                            {
                                fishFail = true;
                                Debug.Log("Failed to Catch Fish with Rotation Difference " + rotationDifference);
                            }
                        }

                        break;

                    //Line is cast, there is a bobber, there is no notification, and there is no fish on the line
                    //(The player is reeling in their line without a fish)
                    case (true, true, false, false):

                        Destroy(bobber.gameObject);
                        bobber = null;
                        lineCast = false;

                        break;

                    default:
                        break;
                }
            }
        }
        else //If the bobber is out of range of the player, meaning they cannot cast or cannot hold their line...
        {
            crosshair.GetComponent<SpriteRenderer>().color = Color.red;

            if (lineCast) //If the line is cast, recall it
            {
                if (bobber != null)
                {
                    Destroy(bobber.gameObject);
                    bobber = null;
                }
                lineCast = false;
                fishOnTheLine = false;
            }
        }

        if (movingCones.Count > 0) //if there is a moving cone, rotate it
        {
            movingCones[movingCones.Count - 1].transform.eulerAngles += new Vector3(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        mouseLeftLastFrame = mouseLeftThisFrame;

        #region First-Pass Mechanic
        /*if (distance < range) //If the bobber is within range of the player, meaning the player can cast...
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
                        if (scaleDifference < 0.75f) //0.5f is a temp value, and will be replaced by a random value determined by the fish's difficulty
                        {
                            fishCaught++;
                            Debug.Log("Scale Difference: " + scaleDifference);
                        }
                        else
                        {
                            fishFail = true;
                            Debug.Log("Scale Difference: " + scaleDifference);
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
        }*/
        #endregion
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
