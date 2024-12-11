using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    
    public void SetMasterVolume(float level){
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level)*20);
    }
}

