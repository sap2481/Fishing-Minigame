using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixer : MonoBehaviour
{
    //==== FIELDS ====
    [SerializeField] AudioSource bloop;

    //==== Start ====
    void Start()
    {
        bloop = GetComponent<AudioSource>();
        bloop.enabled = true;
    }

    //==== METHODS ====
    public void PlayBloop()
    {
        bloop.Play();
        Debug.Log("Bloop played");
    }
}
