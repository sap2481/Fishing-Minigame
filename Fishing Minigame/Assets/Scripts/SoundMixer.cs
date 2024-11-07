using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixer : MonoBehaviour
{
    //==== FIELDS ====
    [SerializeField] GameObject bloopPrefab;
    [SerializeField] GameObject pingPrefab;
    [SerializeField] GameObject successPrefab;
    [SerializeField] GameObject failurePrefab;
    [SerializeField] GameObject reelPrefab;

    AudioSource bloop;
    AudioSource ping;
    AudioSource success;
    AudioSource failure;
    AudioSource reel;

    //==== Start ====
    void Start()
    {
        bloop = Instantiate(bloopPrefab, this).GetComponent<AudioSource>();
        bloop.enabled = true;

        ping = Instantiate(pingPrefab, this).GetComponent<AudioSource>(); ;
        ping.enabled = true;

        success = Instantiate(successPrefab, this).GetComponent<AudioSource>(); ;
        success.enabled = true;

        failure = Instantiate(failurePrefab, this).GetComponent<AudioSource>(); ;
        failure.enabled = true;

        reel = Instantiate(reelPrefab, this).GetComponent<AudioSource>(); ;
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

//None of them are working oh shit
