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
    bool countdownActive;

    //Timer Panel
    [SerializeField] GameObject timerPanel;
    float timeRemaining;

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
                    questlogInstance.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<Image>().color = Color.yellow;
                }
                else
                {
                    questlogInstance.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = "--EMPTY--";
                    questlogInstance.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<Image>().color = Color.white;
                }
            }
        }

        //If a quest is a timer quest, and the timer panel hasn't been activated yet, activate the timer panel
        if (!timerPanel.activeSelf)
        {
            foreach (Quest quest in questList)
            {
                timerPanel.SetActive(false);
                if (quest.Type == 2) //if the quest is a timer quest, set the timer panel to active
                {
                    timerPanel.SetActive(true);
                    timeRemaining = quest.Seconds;
                    timerPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = "Time Left: " + timeRemaining;
                    break;
                }
            }
        }

        if (CheckForTimerQuest()) //If there's a timer quest in the questlog, count down the timer
        {
            if (!menu.menuInstance && !outpost.outpostActive && !questlogActive) { timeRemaining -= Time.deltaTime; } //If no menus are open, count down the timer
            timerPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = "Time Left: " + (int)timeRemaining;

            //If the player runs out of time, kill the timer panel & the quest
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerPanel.SetActive(false);
                foreach (Quest quest in questList)
                {
                    if (quest.Type == 2)
                    {
                        questList.Remove(quest);
                        break;
                    }
                }
            }
        }
        else //If there is no timer quest, deactivate panel
        {
            timerPanel.SetActive(false);
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
    public bool CheckForTimerQuest()
    {
        bool timerQuest = false;
        foreach (Quest quest in questList)
        {
            if (quest.Type == 2)
            {
                timerQuest = true;
            }
        }
        return timerQuest;
    }
}
