using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoadingMask : MonoBehaviour
{
    public event UnityAction onFadedIn = delegate { };

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 这个函数会在淡入动画的最后调用
    /// </summary>
    public void OnOpenAnimationFinished()
    {
        onFadedIn();
        GetComponent<Animator>().SetTrigger("Close");
    }

    /// <summary>
    /// 这个函数会在淡出动画的最后调用
    /// </summary>
    public void OnCloseAnimationFinished()
    {
        Destroy(transform.gameObject);
    }
}
