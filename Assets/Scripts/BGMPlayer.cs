using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public AudioSource TargetSource;
    public AudioClip[] BGMList;
    float _volume;
    public float Volume
    {
        get { return _volume; }
        set
        {
            if (value < 0.0f)
                _volume = 0.0f;
            else if (value > 1.0f)
                _volume = 1.0f;
            else
            {
                _volume = value;
                TargetSource.volume = _volume;
            }
        }
    }

    int _curPlaying;

    void Start () {
        if (BGMList.Length <= 0)
            this.enabled = false;

        _volume = 1.0f;

        TargetSource.volume = _volume;

        _curPlaying = 0;
        TargetSource.PlayOneShot(BGMList[_curPlaying]);
    }
    
    void Update () {
        if (!TargetSource.isPlaying)
        {
            if (_curPlaying < BGMList.Length)
                ++_curPlaying;
            else
                _curPlaying = 0;
            
            TargetSource.PlayOneShot(BGMList[_curPlaying]);
        }
    }
}
