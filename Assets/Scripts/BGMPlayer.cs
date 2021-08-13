using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{
    public AudioClip[] BGMList;

    AudioSource source;
    int _curPlaying;

    void Start () {
        source = GetComponent<AudioSource>();
        source.volume = 0.01f * Settings.Values.BGMVolumn * 0.01f * Settings.Values.OverallVolumn;

        if (BGMList.Length <= 0)
            this.enabled = false;

        _curPlaying = 0;
        source.PlayOneShot(BGMList[_curPlaying]);
    }
    
    void Update () {
        if (!source.isPlaying)
        {
            if (_curPlaying < BGMList.Length)
                ++_curPlaying;
            else
                _curPlaying = 0;
            
            source.PlayOneShot(BGMList[_curPlaying]);
        }
    }
}
