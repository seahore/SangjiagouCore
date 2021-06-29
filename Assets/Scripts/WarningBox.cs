using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 警告窗口脚本
/// </summary>
public class WarningBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam.GetComponent<ScreenBlurFX>().BlurSizeChangeTo(0.2f);
    }

    void OnDestroy()
    {
        if (GameObject.Find("Upper UI Canvas").transform.childCount > 1)
            return;

        var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam.GetComponent<ScreenBlurFX>().BlurSizeChangeTo(0);
        var mask = GameObject.FindGameObjectWithTag("UpperUIMask");
        if (!(mask is null)) Destroy(mask);
    }

    public void OnCloseButtonClick()
    {
        Destroy(gameObject);
    }
}
