using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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
    public GameObject crosshair;
    public GameObject bobber; //Crosshair but a different color, used when the player has cast their line
    [SerializeField] GameObject notificationPrefab;
    public GameObject notification;
    [SerializeField] GameObject circlePrefab;
    [SerializeField] GameObject whirlpoolPrefab;
    List<GameObject> whirlpools;
    [SerializeField] GameObject whirlpoolHolder;

    //Primary Mechanic Related Fields
    [SerializeField] GameObject conePrefab;
    List<GameObject> targetCones;
    List<GameObject> movingCones;
    List<GameObject> circles;
    float rotationSpeed; //How fast the current moving cone is rotating
    float rotationDifference; //Difference in rotation between target cone and current moving cone
    bool leftOrRight; //Left = false, right = true ; Determines rotation of moving cone
    int ringTotal; //How many total rings this particular cast requires the player to go through - REQUIRED FOR PRIMARY MECHANIC    
    int ringCount; //How many rings has the player currently gone through? - REQUIRED FOR PRIMARY MECHANIC

    //Player
    GameObject player;

    //Collisions
    [SerializeField] Collisions collisions;

    //Sound
    [SerializeField] SoundMixer soundMixer;

    //Fish
    Fish fishInProgress;
    List<Fish> fishList;

    //Other Values
    public bool lineCast; //Is the line cast?
    int fishCaught; //Did the player catch a fish?
    public int numberOfCasts; //Make sure that each cast is independent of each other cast (aka make sure that the timing of a previous cast doesn't carry over to the next)
    public bool fishOnTheLine; //Is a fish on the line?
    float range; //The range at which a player can cast their line away from their ship
    float rangeStorage; //Range gets changed around a bunch, so I need a property that maintains the current range value
    float distance; //How far is the player attempting to cast?
    bool fishFail; //Declare if the player failed to catch a fish (used for the CatchTracker)
    float whirlpoolRotSpeed; //How fast the whirlpools rotate

    Tutorial tutorial;

    //First-Pass Mechanic Related Fields (DEPRECATED)
    //float circleScale; //The scale of the target circle
    //float scaleSpeed; //The speed at which the scaling circle scales upwards
    //float scaleDifference; //The total difference in scales between target circles and their respective scaling circles.
    //GameObject targetCircle;
    //GameObject scalingCircle;

    //==== PROPERTIES ====
    public int FishCaught { get { return fishCaught; } set { fishCaught = value; } }
    public bool FishFail { get { return fishFail; } set { fishFail = value; } }
    public List<Fish> FishList { get { return fishList; } set { fishList = value; } }
    public float Range { get { return range; } set { range = value; } }
    public float RangeStorage { get { return rangeStorage; } set { rangeStorage = value; } }

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

        //Find Tutorial, if applicable
        tutorial = GameObject.FindObjectOfType<Tutorial>();

        //Instantiate Lists
        targetCones = new List<GameObject>();
        movingCones = new List<GameObject>();
        circles = new List<GameObject>();
        fishList = new List<Fish>();
        whirlpools = new List<GameObject>();

        //Instantiate Whirlpools
        if (SceneManager.GetActiveScene().name != "TutorialScene")
        {
            for (int i = 0; i < 10; i++)
            {
                whirlpools.Add(Instantiate(whirlpoolPrefab, new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f)), Quaternion.identity, whirlpoolHolder.transform));
            }
        }
        else
        {
            whirlpools.Add(Instantiate(whirlpoolPrefab, new Vector3(0, 3), Quaternion.identity, whirlpoolHolder.transform));
        }

        //Initialize Other Values
        lineCast = false;
        fishCaught = 0;
        numberOfCasts = 0;
        fishOnTheLine = false;
        ringTotal = 0;
        ringCount = 0;
        range = 4;
        rangeStorage = range; //this is a bad solution because range will eventually be upgradeable, come back to it
        distance = 0;
        fishFail = false;
        rotationSpeed = 0;
        rotationDifference = 0;
        leftOrRight = false;
        whirlpoolRotSpeed = 50;

        /*Deprecated Variable Initialization
        circleScale = 0;
        scaleSpeed = 0;
        scaleDifference = 0;
         * */
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
                        float waitingTime = Random.Range(10f, 15f);
                        foreach (GameObject whirlpool in whirlpools)
                        {
                            if (collisions.CheckSpriteCollision(whirlpool, bobber))
                            {
                                waitingTime = Random.Range(3.0f, 5.0f);
                                break;
                            }
                        }
                        soundMixer.PlayBloop();
                        if (tutorial == null || (tutorial != null && tutorial.increment == 5)) { StartCoroutine(WaitForFish(waitingTime, numberOfCasts)); }
                        
                        break;
                    
                    //Line has been cast, there is a bobber and a notification, but a fish is not yet on the line
                    //(The player is beginning to catch a fish)
                    case (true, false, false, false):
                        
                        fishOnTheLine = true;
                        if (SceneManager.GetActiveScene().name == "TutorialScene") { ringTotal = 2; }
                        else { ringTotal = Random.Range(2, 5); }
                        fishInProgress = new Fish(ringTotal);
                        ringCount = 1;
                        soundMixer.PlayReel();

                        //Instantiate Bullseye Center
                        circles.Add(Instantiate(circlePrefab));
                        circles[0].transform.position = bobber.transform.position;
                        circles[0].transform.localScale = new Vector3(0.5f, 0.5f);
                        circles[0].GetComponent<SpriteRenderer>().color = Color.black;
                        circles[0].GetComponent<SpriteRenderer>().sortingOrder = ringTotal * 4 + 4;

                        //Instantiate First Target Ring
                        circles.Add(Instantiate(circlePrefab));
                        circles[1].transform.position = circles[0].transform.position;
                        circles[1].transform.localScale = new Vector3(1.0f, 1.0f);
                        circles[1].gameObject.GetComponent<SpriteRenderer>().color = new Color32(50, 50, 50, 255);
                        circles[1].GetComponent<SpriteRenderer>().sortingOrder = ringTotal * 4;

                        //Instantiate First Target Cone
                        targetCones.Add(Instantiate(conePrefab));
                        targetCones[0].transform.position = circles[0].transform.position;
                        float targetRot = Random.Range(-179, 180);
                        targetCones[0].transform.eulerAngles = new Vector3(0, 0, targetRot);
                        targetCones[0].transform.localScale = new Vector3(circles[1].transform.localScale.x / 22.5f, circles[1].transform.localScale.y / 22.5f);
                        targetCones[0].GetComponent<SpriteRenderer>().color = Color.cyan;
                        targetCones[0].GetComponent<SpriteRenderer>().sortingOrder = ringTotal * 4 + 2;

                        //Instantiate First Moving Cone
                        movingCones.Add(Instantiate(conePrefab));
                        movingCones[0].transform.position = circles[0].transform.position;
                        float startRot = Random.Range(-179, 180);
                        movingCones[0].transform.eulerAngles = new Vector3(0, 0, startRot);
                        movingCones[0].transform.localScale = new Vector3(circles[1].transform.localScale.x / 22.5f, circles[1].transform.localScale.y / 22.5f);
                        movingCones[0].GetComponent<SpriteRenderer>().color = Color.white;
                        movingCones[0].GetComponent<SpriteRenderer>().sortingOrder = ringTotal * 4 + 3;
                        rotationSpeed = Random.Range(250, 400);

                        //Destroy notification & bobber
                        Destroy(notification.gameObject);
                        notification = null;
                        Destroy(bobber.gameObject);
                        bobber = null;

                        //Decide Whether to Rotate Left or Right
                        float directionPicker = Random.Range(1, 101);
                        if (directionPicker <= 50) { leftOrRight = false; } else { leftOrRight = true; }

                        break;

                    //Line is cast, there is no bobber, there is no notification, and there is a fish on the line
                    //(The player is moving through the mechanic, trying to catch the fish)
                    case (true, true, true, true):
                        
                        ringCount++;
                        soundMixer.PlayPing();

                        //Measure rotation difference between target cone and moving cone
                        float thisDifference = Quaternion.Angle(movingCones[^1].transform.rotation, targetCones[^1].transform.rotation);
                        rotationDifference += thisDifference;

                        if (ringCount > ringTotal) //If the player has completed their progression through the rings...
                        {
                            lineCast = false;
                            fishOnTheLine = false;

                            //Move Whirlpool If Applicable
                            foreach (GameObject whirlpool in whirlpools)
                            {
                                if (collisions.CheckSpriteCollision(whirlpool, circles[0]) && SceneManager.GetActiveScene().name != "TutorialScene")
                                {
                                    whirlpool.transform.position = new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30));
                                    break;
                                }
                            }

                            //Destroy All Mechanic Items
                            foreach (GameObject tCone in targetCones) { Destroy(tCone.gameObject); }
                            targetCones.Clear();
                            foreach (GameObject mCone in movingCones) { Destroy(mCone.gameObject); }
                            movingCones.Clear();
                            foreach (GameObject circle in circles) { Destroy(circle.gameObject); }
                            circles.Clear();
                            soundMixer.StopReel();

                            //Determine a catch
                            if (rotationDifference < fishInProgress.DifficultyLevel) //this difficulty level will later be changed depending on the fish ur catching
                            {
                                fishCaught++;
                                if (fishList.Count < 6) { fishList.Add(fishInProgress); }
                                soundMixer.PlaySuccess();
                                Debug.Log(fishInProgress.Name + " Caught with Rotation Difference " + rotationDifference);
                            }
                            else
                            {
                                fishFail = true;
                                soundMixer.PlayFailure();
                                Debug.Log("Failed to Catch " + fishInProgress.Name + " with Rotation Difference " + rotationDifference);
                            }

                            rotationDifference = 0;
                            fishInProgress = null;
                        }
                        else //If the player still has rings to traverse through...
                        {
                            //Hide Last Target Cone
                            targetCones[^1].SetActive(false);

                            //Play Sound Cue
                            soundMixer.PlayPing();

                            //Measure how well a player did on the last ring, & change the color of the last moving cone to reflect it
                            if (thisDifference < 15) { movingCones[^1].GetComponent<SpriteRenderer>().color = Color.green; }
                            else if (thisDifference < 50) { movingCones[^1].GetComponent<SpriteRenderer>().color = Color.yellow; }
                            else { movingCones[^1].GetComponent<SpriteRenderer>().color = Color.red; }

                            //Instantiate Next Target Ring
                            circles.Add(Instantiate(circlePrefab));
                            circles[^1].transform.position = circles[0].transform.position;
                            circles[^1].transform.localScale = new Vector3(circles[^2].transform.localScale.x + .5f, circles[^2].transform.localScale.y + .5f);
                            circles[^1].GetComponent<SpriteRenderer>().color = new Color32((byte)(ringCount * 50), (byte)(ringCount * 50), (byte)(ringCount * 50), 255);
                            circles[^1].GetComponent<SpriteRenderer>().sortingOrder = circles[^2].GetComponent<SpriteRenderer>().sortingOrder - 4;

                            //Instantiate Next Target Cone
                            targetCones.Add(Instantiate(conePrefab));
                            targetCones[^1].transform.position = circles[0].transform.position;
                            float targetRotation = Random.Range(-179, 180);
                            targetCones[^1].transform.eulerAngles = new Vector3(0, 0, targetRotation);
                            targetCones[^1].transform.localScale = new Vector3(circles[^1].transform.localScale.x / 22.5f, circles[^1].transform.localScale.y / 22.5f);
                            targetCones[^1].GetComponent<SpriteRenderer>().color = Color.cyan;
                            targetCones[^1].GetComponent<SpriteRenderer>().sortingOrder = targetCones[^2].GetComponent<SpriteRenderer>().sortingOrder - 4;

                            //Instantiate Next Moving Cone
                            movingCones.Add(Instantiate(conePrefab));
                            movingCones[^1].transform.position = circles[0].transform.position;
                            float startRotation = Random.Range(-179, 180);
                            movingCones[^1].transform.eulerAngles = new Vector3(0, 0, startRotation);
                            movingCones[^1].transform.localScale = new Vector3(circles[^1].transform.localScale.x / 22.5f, circles[^1].transform.localScale.y / 22.5f);
                            movingCones[^1].GetComponent<SpriteRenderer>().color = Color.white;
                            movingCones[^1].GetComponent<SpriteRenderer>().sortingOrder = movingCones[^2].GetComponent<SpriteRenderer>().sortingOrder - 4;
                            rotationSpeed = Random.Range(250, 500);

                            //Decide Whether to Rotate Left or Right
                            float dirPicker = Random.Range(1, 101);
                            if (dirPicker <= 50) { leftOrRight = false; } else { leftOrRight = true; }
                        }

                        break;

                    //Line is cast, there is a bobber, there is no notification, and there is no fish on the line
                    //(The player is reeling in their line without a fish)
                    case (true, false, true, false):
                        
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
                if (bobber != null) //If the bobber exists, destroy it
                {
                    Destroy(bobber.gameObject);
                    bobber = null;
                }

                //Destroy All Mechanic-Related Cones & Circles
                foreach (GameObject tCone in targetCones) { Destroy(tCone.gameObject); }
                targetCones.Clear();
                foreach (GameObject mCone in movingCones) { Destroy(mCone.gameObject); }
                movingCones.Clear();
                foreach (GameObject circle in circles) { Destroy(circle.gameObject); }
                circles.Clear();

                lineCast = false;
                fishOnTheLine = false;
            }
        }

        if (movingCones.Count > 0) //if there is a moving cone, rotate it
        {
            if (!leftOrRight) //Rotate to the Left (Counterclockwise)
            {
                movingCones[^1].transform.eulerAngles += new Vector3(0f, 0f, rotationSpeed * Time.deltaTime);
            }
            else //Rotate to the Right (Clockwise)
            {
                movingCones[^1].transform.eulerAngles -= new Vector3(0f, 0f, rotationSpeed * Time.deltaTime);
            }
        }

        //Rotate Whirlpools
        /*foreach (GameObject whirlpool in whirlpools)
        {
            whirlpool.transform.eulerAngles += new Vector3(0f, 0f, whirlpoolRotSpeed * Time.deltaTime);
        }*/

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
    public IEnumerator WaitForFish(float waitTime, int currentCastNum)
    {
        yield return new WaitForSeconds(waitTime);
        if (lineCast && currentCastNum == numberOfCasts) //If the line is still cast after the wait time has passed, and is still on the current cast...
        {
            notification = Instantiate(notificationPrefab);
            notification.transform.position = new Vector3(bobber.transform.position.x, bobber.transform.position.y + 1.25f, bobber.transform.position.z);
            yield return new WaitForSeconds(1.0f);
            if (notification != null) //If the notification hasn't been destroyed yet (aka if the player hasn't caught the fish), destroy the notification.
            {
                Destroy(notification.gameObject);
                notification = null;
            }
        }
    }
}
