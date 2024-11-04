using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixer : MonoBehaviour
{
    //==== FIELDS ====
    [SerializeField] AudioSource bloop;
    [SerializeField] AudioSource ping;
    [SerializeField] AudioSource success;
    [SerializeField] AudioSource failure;
    [SerializeField] AudioSource reel;

    //==== Start ====
    void Start()
    {
        bloop = GetComponent<AudioSource>();
        bloop.enabled = true;

        ping = GetComponent<AudioSource>();
        ping.enabled = true;

        success = GetComponent<AudioSource>();
        success.enabled = true;

        failure = GetComponent<AudioSource>();
        failure.enabled = true;

        reel = GetComponent<AudioSource>();
        reel.enabled = true;
    }

    //==== METHODS ====
    public void PlayBloop()
    {
        bloop.Play();
        Debug.Log("Bloop played");
    }
    public void PlayPing()
    {
        ping.Play();
        Debug.Log("Ping Played");
    }
    public void PlaySuccess()
    {
        success.Play();
        Debug.Log("Success played");
    }
    public void PlayFailure()
    {
        failure.Play();
        Debug.Log("Failure played");
    }
    public void PlayReel()
    {
        reel.Play();
        Debug.Log("Reel playing");
    }
    public void StopReel()
    {
        reel.Stop();
        Debug.Log("Reel stopped");
    }
}

//I don't think reel is working but can't be sure.
