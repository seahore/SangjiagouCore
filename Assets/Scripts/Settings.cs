using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class Settings
{
    /// <summary>
    /// 方便JSON打包
    /// </summary>
    public struct ValueTable
    {
        #region 音频

        int _overallVolumn;
        public int OverallVolumn {
            get => _overallVolumn; set {
                if (value >= 0 && value <= 100) {
                    _overallVolumn = value;
                } else {
                    Debug.LogWarning("总音量设置超限");
                }
            }
        }

        int _bgmVolumn;
        public int BGMVolumn {
            get => _bgmVolumn; set {
                if (value >= 0 && value <= 100) {
                    _bgmVolumn = value;
                } else {
                    Debug.LogWarning("背景音乐音量设置超限");
                }
            }
        }

        int _sfxVolumn;
        public int SFXVolumn {
            get => _sfxVolumn; set {
                if (value >= 0 && value <= 100) {
                    _sfxVolumn = value;
                } else {
                    Debug.LogWarning("音效音量设置超限");
                }
            }
        }

        #endregion



        #region 显示

        public FullScreenMode FullScreenMode { get; set; }
        public Resolution Resolution { get; set; }

        #endregion

    }
    public static ValueTable Values;

    readonly static ValueTable DefaultValues = new ValueTable {
        OverallVolumn = 60,
        BGMVolumn = 80,
        SFXVolumn = 100,
        FullScreenMode = FullScreenMode.FullScreenWindow,
        Resolution = Screen.currentResolution
    };

    public readonly static string SettingsPath = Application.dataPath + Path.DirectorySeparatorChar + "Settings";
    public readonly static string GlobalSettingsFilename = "global.json";


    /// <summary>
    /// 保存当前设置到文件
    /// </summary>
    /// <param name="filename">文件名</param>
    public static void SaveSettings(string filename)
    {
        if (!Directory.Exists(SettingsPath))
            Directory.CreateDirectory(SettingsPath);
        string path = SettingsPath + Path.DirectorySeparatorChar + filename;
        string json = JsonMapper.ToJson(Values);

        Debug.Log(json);

        FileStream settingFile;
        BinaryWriter bw;
        settingFile = new FileStream(path, FileMode.Create);
        bw = new BinaryWriter(settingFile);
        bw.Write(json);
        bw.Close();
    }
    /// <summary>
    /// 从文件读取设置
    /// </summary>
    /// <param name="filename">文件名</param>
    public static void LoadSettings(string filename)
    {
        if (!Directory.Exists(SettingsPath))
            Directory.CreateDirectory(SettingsPath);
        string path = SettingsPath + Path.DirectorySeparatorChar + filename;
        FileStream file;
        BinaryReader br;
        if (!File.Exists(path)) {
            Values = DefaultValues;
            SaveSettings(GlobalSettingsFilename);
            return;
        }
        file = new FileStream(path, FileMode.Open);
        br = new BinaryReader(file);
        string json;
        json = br.ReadString();

        Debug.Log(json);

        br.Close();
        Values = JsonMapper.ToObject<ValueTable>(json);
    }

}
