using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AudioController : MonoBehaviour
{
    private AudioSource target;
    [Header("PowerOutage")]
    public AudioClip[] PowerOutageClips;

    [Header("Earthquake")]
    public AudioClip Earthquake;

    [Header("Malfunction")]
    public AudioClip MalfunctionAlarm;

    void Start()
    {
        target = GetComponent<AudioSource>();
        if (target == null)
        {
            Debug.LogError("No AudioSource found");
        }
    }

    public IEnumerator PlayEarthquake()
    {
        GetComponent<AudioEchoFilter>().enabled = false;
        target.clip = Earthquake;
        target.Play();
        yield return new WaitForSeconds(Earthquake.length);
        GetComponent<AudioEchoFilter>().enabled = true;

        yield return null;
    }

    public IEnumerator PlayPowerOutage()
    {
        foreach (var clip in PowerOutageClips)
        {
            target.clip = clip;
            target.Play();
            yield return new WaitForSeconds(clip.length);
        }

        yield return null;
    }

    public IEnumerator PlayMalfunction()
    {
        target.clip= MalfunctionAlarm;
        target.Play();

        yield return null;
    }
}
