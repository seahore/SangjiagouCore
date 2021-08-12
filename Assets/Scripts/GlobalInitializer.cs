using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SangjiagouCore;

/// <summary>
/// 全局初始化脚本
/// </summary>
public class GlobalInitializer : MonoBehaviour
{
    public AudioSource BGMSource;
    public AudioSource SFXSource;

    void Awake()
    {
        // 检查目录是否存在
        if (!Directory.Exists(Settings.SettingsPath))
            Directory.CreateDirectory(Settings.SettingsPath);
        if (!Directory.Exists(Game.SavesPath))
            Directory.CreateDirectory(Game.SavesPath);
        if (!Directory.Exists(Game.ScenariosPath))
            Directory.CreateDirectory(Game.ScenariosPath);

        Settings.LoadSettings(Settings.GlobalSettingsFilename);
        Screen.SetResolution(Settings.Values.Resolution.width, Settings.Values.Resolution.height, Settings.Values.FullScreenMode, Settings.Values.Resolution.refreshRate);

        BGMSource.volume = 0.01f * Settings.Values.BGMVolumn * 0.01f * Settings.Values.OverallVolumn;
        SFXSource.volume = 0.01f * Settings.Values.SFXVolumn * 0.01f * Settings.Values.OverallVolumn;
    }

    void Start()
    {

    }

    void Update()
    {
        
    }
}
