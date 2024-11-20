using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    //==== FIELDS ====
    //Questlog
    public List<Quest> questList;
    [SerializeField] GameObject questlogPrefab;
    public GameObject questlogInstance;

    //Bools
    bool qKeyLastFrame;
    bool qKeyThisFrame;
    public bool questlogActive;
    bool countdownStarted;

    //Timer Panel
    [SerializeField] GameObject timerPanel;
    int timeRemaining;

    //Other Relevant Instances
    GameObject player;
    float maxSpeedStorage;
    Outpost outpost;
    Menu menu;
    
    //==== START ====
    void Start()
    {
        //Find Relevant Instances
        player = GameObject.FindGameObjectWithTag("Player");
        outpost = GameObject.FindObjectOfType<Outpost>();
        menu = GameObject.FindObjectOfType<Menu>();

        //Set bools
        qKeyLastFrame = false;
        qKeyThisFrame = false;
        questlogActive = false;

        //Instantiate Questlist
        questList = new List<Quest>();

        timerPanel.SetActive(false);
    }

    //==== UPDATE ====
    void Update()
    {
        qKeyThisFrame = Keyboard.current.qKey.isPressed;

        if (!qKeyThisFrame && qKeyLastFrame) //If Q was pressed...
        {
            if (!menu.menuInstance && !outpost.outpostActive && !questlogActive) //If no menus are active, activate this one
            {
                questlogActive = true;
                questlogInstance = Instantiate(questlogPrefab);
                questlogInstance.transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(xOut);

                maxSpeedStorage = player.GetComponent<Player>().MaxSpeed;
                player.GetComponent<Player>().MaxSpeed = 0;
            }
            else if (questlogActive) //If Q was pressed while the questlog is active, exit the questlog
            {
                xOut();
            }
        }

        //If questlog is open, don't let the player fish
        if (questlogActive) { player.GetComponent<Fishing>().Range = 0; }
        else { player.GetComponent<Fishing>().Range = player.GetComponent<Fishing>().RangeStorage; }

        if (questlogInstance != null) //If there's a questlog instance, fill it in
        {
            for (int i = 0; i < 3; i++)
            {
                if (i < questList.Count)
                {
                    questlogInstance.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = questList[i].Description;
                }
                else
                {
                    questlogInstance.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = "--EMPTY--";
                }
            }
        }

        //If a quest is a timer quest, activate the timer panel
        foreach (Quest quest in questList)
        {
            timerPanel.SetActive(false);
            if (quest.Type == 2) //if the quest is a timer quest, set the timer panel to active
            {
                timerPanel.SetActive(true);
                if (!countdownStarted)
                {
                    StartCoroutine(StartCountdown(quest.Seconds));
                    countdownStarted = true;
                }
                break;
            }
        }

        qKeyLastFrame = qKeyThisFrame;
    }

    //==== METHODS & COROUTINES
    public void xOut()
    {
        questlogActive = false;
        Destroy(questlogInstance.gameObject);
        questlogInstance = null;
        player.GetComponent<Player>().MaxSpeed = maxSpeedStorage;
    }
    public IEnumerator StartCountdown(int countdownValue)
    {
        timeRemaining = countdownValue;
        while (timeRemaining > 0)
        {
            if (timerPanel.activeSelf && !menu.menuInstance && !outpost.outpostActive && !questlogActive)
            {
                yield return new WaitForSeconds(1.0f);
                timeRemaining--;
                timerPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = timeRemaining.ToString();
            }
        }
        
        timerPanel.SetActive(false);
        foreach (Quest quest in questList)
        {
            if (quest.Type == 2)
            {
                questList.Remove(quest);
            }
        }
    }
}
