using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextTurnInfoPanel : MonoBehaviour
{
    public GameObject MaskPrefab;

    void Start()
    {
        var mask = Instantiate(MaskPrefab, transform.parent);
        transform.SetParent(mask.transform);

        var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam.GetComponent<Animator>().SetTrigger("Unfocus");
    }

    public void OnOKButtonClick()
    {
        if (GameObject.Find("Upper UI Canvas").transform.childCount <= 1) {
            var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
            mainCam.GetComponent<Animator>().SetTrigger("Focus");
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
