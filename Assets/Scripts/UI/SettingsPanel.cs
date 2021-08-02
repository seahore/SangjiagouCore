using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置面板控制
/// </summary>
public class SettingsPanel : MonoBehaviour
{
    public const char ResolutionSeperator = '×';

    AudioSource bgmSource;
    AudioSource sfxSource;

    void ResetFullScreenModeDropdown()
    {
        switch (Settings.Values.FullScreenMode) {
            case FullScreenMode.ExclusiveFullScreen:
                transform.Find("Full Screen Mode Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(0);
                break;
            case FullScreenMode.FullScreenWindow:
                transform.Find("Full Screen Mode Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(1);
                break;
            case FullScreenMode.Windowed:
                transform.Find("Full Screen Mode Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(2);
                break;
        }
    }

    void ResetResolutionDropdown()
    {
        List<string> sl = new List<string>();
        List<Resolution> t = new List<Resolution>(Screen.resolutions);
        t.Sort((Resolution x, Resolution y) => x.width < y.width || x.height < x.height ? 1 : -1);
        foreach (Resolution r in t) {
            string s = r.width.ToString() + ResolutionSeperator + r.height;
            if (r.width < 1024 || r.height < 720)
                s += "（不推荐）";
            sl.Add(s);
        }
        sl = new List<string>(sl.Distinct());
        transform.Find("Resolution Dropdown").GetComponent<Dropdown>().ClearOptions();
        transform.Find("Resolution Dropdown").GetComponent<Dropdown>().AddOptions(sl);
        int i;
        for (i = 0; sl[i] != Settings.Values.Resolution.width.ToString() + ResolutionSeperator + Settings.Values.Resolution.height; i++) ;
        transform.Find("Resolution Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(i);
    }

    void ResetRefreshRateDropdown()
    {
        List<string> rrl = new List<string>();
        var t = new List<int>();
        foreach (var r in Screen.resolutions) {
            if((r.width, r.height) == (Settings.Values.Resolution.width, Settings.Values.Resolution.height)) {
                t.Add(r.refreshRate);
            }
        }
        t.Sort((int x, int y) => x > y ? 1 : -1);
        foreach (var rr in t) {
            rrl.Add(rr.ToString());
        }
        transform.Find("Refresh Rate Dropdown").GetComponent<Dropdown>().ClearOptions();
        transform.Find("Refresh Rate Dropdown").GetComponent<Dropdown>().AddOptions(rrl);
        int i;
        for (i = 0; rrl[i] != Settings.Values.Resolution.refreshRate.ToString(); i++) ;
        transform.Find("Refresh Rate Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(i);
    }

    void ResetMainVolumnSlider()
    {
        transform.Find("Main Volumn Slider").GetComponent<Slider>().value = Settings.Values.OverallVolumn;
        transform.Find("Main Volumn Value Text").GetComponent<Text>().text = Settings.Values.OverallVolumn.ToString();
    }

    void ResetBGMVolumnSlider()
    {
        transform.Find("BGM Volumn Slider").GetComponent<Slider>().value = Settings.Values.BGMVolumn;
        transform.Find("BGM Volumn Value Text").GetComponent<Text>().text = Settings.Values.BGMVolumn.ToString();
    }

    void ResetSFXVolumnSlider()
    {
        transform.Find("SFX Volumn Slider").GetComponent<Slider>().value = Settings.Values.SFXVolumn;
        transform.Find("SFX Volumn Value Text").GetComponent<Text>().text = Settings.Values.SFXVolumn.ToString();
    }


    void Start()
    {
        var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam.GetComponent<Animator>().SetTrigger("Unfocus");

        bgmSource = GameObject.FindGameObjectWithTag("BGMSource").GetComponent<AudioSource>();
        sfxSource = GameObject.FindGameObjectWithTag("SFXSource").GetComponent<AudioSource>();

        ResetFullScreenModeDropdown();
        ResetResolutionDropdown();
        ResetRefreshRateDropdown();

        ResetMainVolumnSlider();
        ResetBGMVolumnSlider();
        ResetSFXVolumnSlider();
    }

    public void OnCloseButtonClick()
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
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        Settings.SaveSettings(Settings.GlobalSettingsFilename);
    }

    public void OnFullScreenModeDropdownChanged()
    {
        switch (transform.Find("Full Screen Mode Dropdown").GetComponent<Dropdown>().value) {
            case 0:
                Settings.Values.FullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Settings.Values.FullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Settings.Values.FullScreenMode = FullScreenMode.Windowed;
                break;
        }
        Screen.SetResolution(Settings.Values.Resolution.width, Settings.Values.Resolution.height, Settings.Values.FullScreenMode, Settings.Values.Resolution.refreshRate);
    }

    public void OnResolutionDropdownChanged()
    {
        var sa = transform.Find("Resolution Dropdown").GetComponent<Dropdown>().captionText.text.Split(ResolutionSeperator, '（');
        (int, int) target = (int.Parse(sa[0]), int.Parse(sa[1]));
        foreach (var r in Screen.resolutions) {
            if(target == (r.width, r.height)) {
                Settings.Values.Resolution = r;    // 此时刷新率也顺便更改了
                ResetRefreshRateDropdown();
                break;
            }
        }
        Screen.SetResolution(Settings.Values.Resolution.width, Settings.Values.Resolution.height, Settings.Values.FullScreenMode, Settings.Values.Resolution.refreshRate);
    }

    public void OnRefreshRateDropdownChanged()
    {
        int rr = int.Parse(transform.Find("Refresh Rate Dropdown").GetComponent<Dropdown>().captionText.text);
        Resolution target = Settings.Values.Resolution;
        target.refreshRate = rr;
        foreach (var r in Screen.resolutions) {
            if ((target.width, target.height, target.refreshRate) == (r.width, r.height, r.refreshRate)) {
                Settings.Values.Resolution = r;
                break;
            }
        }
        Screen.SetResolution(Settings.Values.Resolution.width, Settings.Values.Resolution.height, Settings.Values.FullScreenMode, Settings.Values.Resolution.refreshRate);
    }

    public void OnMainVolumnSliderChanged()
    {
        int value = (int)transform.Find("Main Volumn Slider").GetComponent<Slider>().value;
        Settings.Values.OverallVolumn = value;
        transform.Find("Main Volumn Value Text").GetComponent<Text>().text = value.ToString();
        bgmSource.volume = 0.01f * Settings.Values.BGMVolumn * 0.01f * Settings.Values.OverallVolumn;
        sfxSource.volume = 0.01f * Settings.Values.SFXVolumn * 0.01f * Settings.Values.OverallVolumn;
    }

    public void OnBGMVolumnSliderChanged()
    {
        int value = (int)transform.Find("BGM Volumn Slider").GetComponent<Slider>().value;
        Settings.Values.BGMVolumn= value;
        transform.Find("BGM Volumn Value Text").GetComponent<Text>().text = value.ToString();
        bgmSource.volume = 0.01f * Settings.Values.BGMVolumn * 0.01f * Settings.Values.OverallVolumn;
    }

    public void OnSFXVolumnSliderChanged()
    {
        int value = (int)transform.Find("SFX Volumn Slider").GetComponent<Slider>().value;
        Settings.Values.SFXVolumn = value;
        transform.Find("SFX Volumn Value Text").GetComponent<Text>().text = value.ToString();
        sfxSource.volume = 0.01f * Settings.Values.SFXVolumn * 0.01f * Settings.Values.OverallVolumn;
    }
}
