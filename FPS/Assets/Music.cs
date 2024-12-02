using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void MuteMusic()
    {
        audioSource.mute = true;
    }

    public void UnmuteMusic()
    {
        audioSource.mute = false;
    }
}
