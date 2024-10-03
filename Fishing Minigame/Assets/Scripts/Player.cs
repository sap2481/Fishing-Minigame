using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private float maxSpeed = 5f;
    private float accel = 0.05f;

    //Camera
    Camera cam;
    float camHeight;
    float camWidth;

    //==== PROPERTIES ====
    public Vector3 Position { get { return position; } }
    public Vector3 Direction { get { return direction; } }
    public Vector3 Velocity { get { return velocity; } }
    public float MaxSpeed { get { return maxSpeed; } }
    public float Acceleration { get { return accel; } }

    //==== START ====
    void Start()
    {
        position = transform.position;

        //Set Camera
        cam = Camera.main;
        camHeight = cam.orthographicSize * 2.0f;
        camWidth = cam.orthographicSize * cam.aspect;
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
            if (leftSpeed > 0f) { xSpeed = leftSpeed; } else if (rightSpeed > 0f) { xSpeed = rightSpeed; } else { xSpeed = 0f; }
            if (downSpeed > 0f) { ySpeed = -downSpeed; } else if (upSpeed > 0f) { ySpeed = upSpeed; } else { ySpeed = 0f; }
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

        //If moving, store the direction
        if (IsMoving())
        {
            storedDirection = direction;
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
}
