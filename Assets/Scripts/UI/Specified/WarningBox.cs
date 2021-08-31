using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 警告窗口脚本
/// </summary>
public class WarningBox : MonoBehaviour
{
    public GameObject MaskPrefab;
    // Start is called before the first frame update
    void Start()
    {
        var mask = Instantiate(MaskPrefab, transform.parent);
        transform.SetParent(mask.transform);
    }

    public void OnCloseButtonClick()
    {
        if (GameObject.Find("Upper UI Canvas").transform.childCount <= 1) {
            var mask = GameObject.FindGameObjectWithTag("UpperUIMask");
            if (!(mask is null)) Destroy(mask);
        }
        GetComponent<Animator>().SetTrigger("Close");
    }

    /// <summary>
    /// 这个函数会在关闭动画的最后调用
    /// </summary>
    public void OnCloseAnimationFinished()
    {
        Destroy(transform.parent.gameObject);
    }
}
