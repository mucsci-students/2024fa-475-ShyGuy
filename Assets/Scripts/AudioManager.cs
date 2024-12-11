using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Slider volumeSlider;

    AudioSource audioSource;
 
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
}   

