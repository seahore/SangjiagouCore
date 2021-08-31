using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using System.Text;

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

        #region 操作

        /// <summary>
        /// 选取提议后自动分配
        /// </summary>
        public bool AutoAssign { get; set; }
        /// <summary>
        /// 对于只有一个参数的行动自动进入选择模式
        /// </summary>
        public bool AutoSelectMode { get; set; }

        #endregion
    }
    public static ValueTable Values;

    readonly static ValueTable DefaultValues = new ValueTable {
        OverallVolumn = 60,
        BGMVolumn = 80,
        SFXVolumn = 100,
        FullScreenMode = FullScreenMode.FullScreenWindow,
        Resolution = Screen.currentResolution,
        AutoAssign = true,
        AutoSelectMode = true,
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

        File.Create(path).Dispose();
        File.WriteAllBytes(path, Encoding.UTF8.GetBytes(json));
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
        if (!File.Exists(path)) {
            Values = DefaultValues;
            SaveSettings(GlobalSettingsFilename);
            return;
        }
        string json = Encoding.UTF8.GetString(File.ReadAllBytes(path));
        Values = JsonMapper.ToObject<ValueTable>(json);
    }

}
