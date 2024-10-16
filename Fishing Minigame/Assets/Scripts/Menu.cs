using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    //==== FIELDS ====
    [SerializeField] GameObject menuPrefab;
    GameObject menuInstance;
    bool escKeyPressedLastFrame;
    bool escKeyPressedThisFrame;
    bool menuActive;
    
    //==== START ====
    void Start()
    {
        escKeyPressedLastFrame = false;
        escKeyPressedThisFrame = false;
        menuActive = false;
    }

    //==== UPDATE ====
    void Update()
    {
        escKeyPressedThisFrame = Keyboard.current.escapeKey.isPressed;

        if (!escKeyPressedThisFrame && escKeyPressedLastFrame) //If the Escape key was just pressed...
        {
            if (!menuActive) //If the menu is not active, activate it
            {
                menuActive = true;
                menuInstance = Instantiate(menuPrefab);
            }
            else //If the menu is active, deactivate it
            {
                menuActive = false;
                Destroy(menuInstance.gameObject);
                menuInstance = null;
            }
        }

        escKeyPressedLastFrame = escKeyPressedThisFrame;
    }
}
