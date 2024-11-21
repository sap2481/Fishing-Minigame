using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    //==== FIELDS ====
    GameObject player;
    [SerializeField] Slider slider;
    
    //==== START ====
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    //==== UPDATE ====
    void Update()
    {
        slider.maxValue = player.GetComponent<Player>().MaxHull;
        slider.value = player.GetComponent<Player>().Hull;
        if (slider.value < 0) { slider.value = 0; }
    }
}
