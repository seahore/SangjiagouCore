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
    void Awake()
    {
        // 检查目录是否存在
        if (!Directory.Exists(Settings.SettingsPath))
            Directory.CreateDirectory(Settings.SettingsPath);
        if (!Directory.Exists(Game.SavesPath))
            Directory.CreateDirectory(Game.SavesPath);
        if (!Directory.Exists(Game.HistoriesPath))
            Directory.CreateDirectory(Game.HistoriesPath);

        Settings.LoadSettings(Settings.GlobalSettingsFilename);
        Screen.SetResolution(Settings.Values.Resolution.width, Settings.Values.Resolution.height, Settings.Values.FullScreenMode, Settings.Values.Resolution.refreshRate);

        Destroy(gameObject);
    }
}
