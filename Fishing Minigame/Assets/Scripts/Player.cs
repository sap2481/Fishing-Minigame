using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //==== FIELDS ====
    private Vector3 position = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private Vector3 storedDirection = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] float leftSpeed = 0f;
    [SerializeField] float rightSpeed = 0f;
    [SerializeField] float upSpeed = 0f;
    [SerializeField] float downSpeed = 0f;

    private float maxSpeed;
    private float maxSpeedStorage;
    private float accel;

    bool bounceback = false;
    GameObject collidingObj;
    float hull = 100f; //Health
    float maxHull = 100f;
    bool sinking = false; //When the player reaches 0 hull, the boat sinks & the player resets
    bool resetTiles; //Tells the environment to reset the tiles properly

    //Camera
    Camera cam;
    float camHeight;
    float camWidth;

    float money;

    Scene tutorial;

    //==== PROPERTIES ====
    public Vector3 Position { get { return position; } set { position = value; } }
    public Vector3 Direction { get { return direction; } }
    public Vector3 Velocity { get { return velocity; } }
    public float MaxSpeed { get { return maxSpeed; } set { maxSpeed = value; } }
    public float MaxSpeedStorage { get { return maxSpeedStorage; } set { maxSpeedStorage = value; } }
    public float Acceleration { get { return accel; } set { accel = value; } }
    public bool Bounceback { get { return bounceback; } set { bounceback = value; } }
    public float Hull { get { return hull; } set { hull = value; } }
    public float MaxHull { get { return maxHull; } set { maxHull = value; } }
    public bool ResetTiles { get { return resetTiles; } set { resetTiles = value; } }
    public float Money { get { return money; } set { money = value; } }

    //==== START ====
    void Start()
    {
        position = transform.position;

        //Set Camera
        cam = Camera.main;
        camHeight = cam.orthographicSize * 2.0f;
        camWidth = cam.orthographicSize * cam.aspect;

        if (SceneManager.GetActiveScene().name == "TutorialScene") { maxSpeed = 0; } else { maxSpeed = 5; }
        maxSpeedStorage = maxSpeed;
        accel = 0.05f;
    }

    //==== UPDATE ====
    void Update()
    {
        //Move Camera
        cam.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, -10);
        
        //Update Speed
        if (!IsMoving()) //If Not Moving, Decelerate All Directions
        {
            leftSpeed -= accel;
            if (leftSpeed < 0f) { leftSpeed = 0f; }

            rightSpeed -= accel;
            if (rightSpeed < 0f) { rightSpeed = 0f; }

            upSpeed -= accel;
            if (upSpeed < 0f) { upSpeed = 0f; }

            downSpeed -= accel;
            if (downSpeed < 0f) { downSpeed = 0f; }
        }
        else //If Moving...
        {
            //PLAYER MOVING LEFT
            if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) //Is the player moving left?
            {
                if (rightSpeed > 0f) //If they were going to the right, decelerate right
                {
                    rightSpeed -= accel;
                }
                else //If there is no rightward movement, accelerate left
                {
                    leftSpeed += accel;
                    if (leftSpeed >= maxSpeed)
                    {
                        leftSpeed = maxSpeed;
                    }
                }
            }

            //PLAYER MOVING RIGHT
            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) //Is the player moving right?
            {
                if (leftSpeed > 0f) //If they were going to the left, decelerate left
                {
                    leftSpeed -= accel;
                }
                else //If there is no leftward movement, accelerate right
                {
                    rightSpeed += accel;
                    if (rightSpeed >= maxSpeed)
                    {
                        rightSpeed = maxSpeed;
                    }
                }
            }

            //PLAYER MOVING UP
            if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed) //Is the player moving up?
            {
                if (downSpeed > 0f) //If they were going down, decelerate down
                {
                    downSpeed -= accel;
                }
                else //If there is no downward movement, accelerate up
                {
                    upSpeed += accel;
                    if (upSpeed >= maxSpeed)
                    {
                        upSpeed = maxSpeed;
                    }
                }
            }

            //PLAYER MOVING DOWN
            if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed) //Is the player moving down?
            {
                if (upSpeed > 0f) //If they were going up, decelerate up
                {
                    upSpeed -= accel;
                }
                else //If there is no upward movement, accelerate down
                {
                    downSpeed += accel;
                    if (downSpeed >= maxSpeed)
                    {
                        downSpeed = maxSpeed;
                    }
                }
            }
        }
        
        //Normalize direction
        direction.Normalize();

        //Rotate transform towards movement
        if (IsMoving())
        {
            float xSpeed; float ySpeed;
            if (leftSpeed > 0f) { xSpeed = leftSpeed; } else if (rightSpeed > 0f) { xSpeed = rightSpeed; } else { xSpeed = .1f; }
            if (downSpeed > 0f) { ySpeed = -downSpeed; } else if (upSpeed > 0f) { ySpeed = upSpeed; } else { ySpeed = .1f; }
            Vector3 moveDirection = new Vector3(xSpeed, ySpeed, 0);
            moveDirection.Normalize();
            if (moveDirection != Vector3.zero)
            {
                this.gameObject.transform.rotation = Quaternion.LookRotation(moveDirection);
                this.gameObject.transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.x + 90f);
                if (rightSpeed > 0f) //Make sure the player can rotate to the right
                {
                    Quaternion tempRot = this.gameObject.transform.rotation;
                    tempRot.z = -tempRot.z;
                    this.gameObject.transform.rotation = tempRot;
                }
            }
        }

        if (bounceback) //If the player is bouncing back from a collision, do that
        {
            Vector3 bounceDir = (transform.position - collidingObj.transform.position).normalized;
            velocity = bounceDir * 5f * Time.deltaTime;
            position += velocity;
            transform.position = position;
        }
        else //If the player is not bouncing back from a collision, move as normal
        {
            //Set velocity
            //Find X-Velocity
            if (leftSpeed > 0f) { velocity.x = -leftSpeed * Time.deltaTime; }
            else if (rightSpeed > 0f) { velocity.x = rightSpeed * Time.deltaTime; }
            else { velocity.x = 0f; }

            //Find Y-Velocity
            if (upSpeed > 0f) { velocity.y = upSpeed * Time.deltaTime; }
            else if (downSpeed > 0f) { velocity.y = -downSpeed * Time.deltaTime; }
            else { velocity.y = 0f; }

            //Set position
            position += velocity;
            transform.position = position;
        }

        //If moving, store the direction
        if (IsMoving())
        {
            storedDirection = direction;
        }

        if (hull <= 0) { sinking = true; } //if the player reaches 0 Hull, start sinking the ship
        if (sinking) //If the ship is sinking...
        {
            this.transform.localScale -= new Vector3(0.5f * Time.deltaTime, 0.5f * Time.deltaTime);
            leftSpeed = 0f; rightSpeed = 0f; upSpeed = 0f; downSpeed = 0f;
            if (this.transform.localScale.x <= 0f) //When the ship has "sunk", reset it
            {
                this.gameObject.GetComponent<Fishing>().FishList.Clear();
                this.transform.localScale = new Vector3(0.5f, 0.5f);
                position = new Vector3(0, 0, 0);
                hull = maxHull;
                money = 0;
                sinking = false;
                resetTiles = true;
            }
        }
    }

    //==== FUNCTIONS ====
    public void OnMove(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    //Check to see if a movement key is being pressed (aka if the player actively moving)
    bool IsMoving()
    {
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed || Keyboard.current.upArrowKey.isPressed || Keyboard.current.downArrowKey.isPressed
            || Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed || Keyboard.current.sKey.isPressed || Keyboard.current.dKey.isPressed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void StartBounceback(GameObject collidingObj)
    {
        this.collidingObj = collidingObj;
        if (!bounceback) { hull -= 10; }
        bounceback = true;
        leftSpeed = 0f;
        rightSpeed = 0f;
        upSpeed = 0f;
        downSpeed = 0f;
        StartCoroutine(BouncebackTimer(0.25f));
    }

    private IEnumerator BouncebackTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        bounceback = false;
    }
}
