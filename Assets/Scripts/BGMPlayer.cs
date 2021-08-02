using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public AudioSource TargetSource;
    public AudioClip[] BGMList;

    int _curPlaying;

    void Start () {
        if (BGMList.Length <= 0)
            this.enabled = false;

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
