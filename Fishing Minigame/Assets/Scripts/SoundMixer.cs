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
    [SerializeField] GameObject notificationPrefab;
    [SerializeField] GameObject impactPrefab;
    [SerializeField] GameObject sinkingPrefab;

    AudioSource bloop;
    AudioSource ping;
    AudioSource success;
    AudioSource failure;
    AudioSource reel;
    AudioSource notification;
    AudioSource impact;
    AudioSource sinking;

    //==== Start ====
    void Start()
    {
        bloop = Instantiate(bloopPrefab, this.transform).GetComponent<AudioSource>();
        bloop.enabled = true;

        ping = Instantiate(pingPrefab, this.transform).GetComponent<AudioSource>();
        ping.enabled = true;

        success = Instantiate(successPrefab, this.transform).GetComponent<AudioSource>();
        success.enabled = true;

        failure = Instantiate(failurePrefab, this.transform).GetComponent<AudioSource>();
        failure.enabled = true;

        reel = Instantiate(reelPrefab, this.transform).GetComponent<AudioSource>();
        reel.enabled = true;

        notification = Instantiate(notificationPrefab, this.transform).GetComponent<AudioSource>();
        notification.enabled = true;

        impact = Instantiate(impactPrefab, this.transform).GetComponent<AudioSource>();
        impact.enabled = true;

        sinking = Instantiate(sinkingPrefab, this.transform).GetComponent<AudioSource>();
        sinking.enabled = true;
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
    public void PlayNotification()
    {
        notification.Play();
        Debug.Log("Notification Played");
    }
    public void PlayImpact()
    {
        impact.Play();
        Debug.Log("Impact Played");
    }
    public void PlaySinking()
    {
        sinking.Play();
        Debug.Log("Sinking Played");
    }
}

//None of them are working oh shit
