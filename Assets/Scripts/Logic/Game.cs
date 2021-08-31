using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using UnityEngine;
using LitJson;


namespace SangjiagouCore {

    public static class Game
    {
        readonly static byte[] _cryptKeys = { 0xCA, 0xFE, 0xBA, 0xBE, 0xDA, 0xD0, 0xBE, 0xD0 };

        public readonly static string HistoriesPath = Application.dataPath + Path.DirectorySeparatorChar + "Histories";
        public readonly static string SavesPath = Application.dataPath + Path.DirectorySeparatorChar + "Saves";
        public readonly static string AvatarsPath = Application.dataPath + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar + "Avatars";
        public readonly static string FlagsPath = Application.dataPath + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar + "Flags";

        static Entities _currentEntities;
        public static Entities CurrentEntities { get => _currentEntities; }
        public static bool Playing => !(_currentEntities is null);

        /// <summary>
        /// 读取存档或开场
        /// </summary>
        /// <param name="fileName">文件名（带扩展名）</param>
        /// <param name="isScenario">是否是开场文件</param>
        /// <param name="encrypted">是否加密并用Base64编码转换过</param>
        public static void LoadGame(string fileName, bool isScenario = false, bool encrypted = true)
        {
            _currentEntities = new Entities();

            string path;
            if (isScenario) {
                if (!Directory.Exists(HistoriesPath))
                    Directory.CreateDirectory(HistoriesPath);
                path = HistoriesPath;
            } else {
                if (!Directory.Exists(SavesPath))
                    Directory.CreateDirectory(SavesPath);
                path = SavesPath;
            }
            path += Path.DirectorySeparatorChar + fileName;
            string json;
            if (encrypted) {
                byte[] encryptedData = Convert.FromBase64String(Encoding.UTF8.GetString(File.ReadAllBytes(path)));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream mstream = new MemoryStream();
                CryptoStream cstream = new CryptoStream(mstream, des.CreateDecryptor(_cryptKeys, _cryptKeys), CryptoStreamMode.Write);
                cstream.Write(encryptedData, 0, encryptedData.Length);
                cstream.FlushFinalBlock();
                json = Encoding.UTF8.GetString(mstream.ToArray());
            } else {
                json = Encoding.UTF8.GetString(File.ReadAllBytes(path));
            }

            _currentEntities.Unpack(JsonMapper.ToObject<Entities.Package>(json));
            _currentEntities.Relink();
        }
        /// <summary>
        /// 保存游戏到文件
        /// </summary>
        /// <param name="fileName">文件名（带扩展名）</param>
        /// <param name="encrypt">是否加密并用Base64编码转换</param>
        public static void SaveGame(string fileName, bool encrypt = true)
        {
            if (!Playing) return;

            if (!Directory.Exists(SavesPath))
                Directory.CreateDirectory(SavesPath);
            string saveFilePath = SavesPath + Path.DirectorySeparatorChar + fileName;
            string json = JsonMapper.ToJson(_currentEntities.Pack());

            File.Create(saveFilePath).Dispose();
            if (encrypt) {
                byte[] data = Encoding.UTF8.GetBytes(json);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream mstream = new MemoryStream();
                CryptoStream cstream = new CryptoStream(mstream, des.CreateEncryptor(_cryptKeys, _cryptKeys), CryptoStreamMode.Write);
                cstream.Write(data, 0, data.Length);
                cstream.FlushFinalBlock();
                string encryptedData = Convert.ToBase64String(mstream.ToArray());
                File.WriteAllBytes(saveFilePath, Encoding.UTF8.GetBytes(encryptedData));
            } else {
                File.WriteAllBytes(saveFilePath, Encoding.UTF8.GetBytes(json));
            }
        }

        /// <summary>
        /// 读取所有历史文件并解析其中的开场
        /// </summary>
        /// <returns>所有读取的历史</returns>
        public static List<History> LoadHistories()
        {
            List<History> histories = new List<History>();
            foreach (var file in new DirectoryInfo(HistoriesPath).GetFiles()) {
                if (file.Extension != ".lishi" && file.Extension != ".json") continue;
                string json = Encoding.UTF8.GetString(File.ReadAllBytes(file.FullName));
                histories.Add(JsonMapper.ToObject<History>(json));
            }
            return histories;
        }

        /// <summary>
        /// 通用的读取图像方法
        /// </summary>
        /// <param name="fullPath">图像文件的完整路径</param>
        /// <returns>封装图像的Sprite</returns>
        public static Sprite LoadSprite(string fullPath)
        {
            if (File.Exists(fullPath)) {
                var ts = fullPath.Trim().Split('.');
                switch (ts[ts.Length - 1].ToLower()) {
                    case "jpg":
                    case "png":
                        var t2d = new Texture2D(100, 100);
                        t2d.LoadImage(File.ReadAllBytes(fullPath));
                        return Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0, 0));
                }
                throw new BadImageFormatException();
            } else {
                throw new FileNotFoundException("找不到指定的图像文件：" + fullPath);
            }
        }

        /// <summary>
        /// 读取肖像图片
        /// </summary>
        /// <param name="filename">图片文件名</param>
        /// <returns>封装图像的Sprite</returns>
        public static Sprite LoadAvatar(string filename)
        {
            if (!Directory.Exists(AvatarsPath))
                Directory.CreateDirectory(AvatarsPath);

            return LoadSprite(AvatarsPath + Path.DirectorySeparatorChar + filename);
        }

        /// <summary>
        /// 读取旗帜图片
        /// </summary>
        /// <param name="filename">图片文件名</param>
        /// <returns>封装图像的Sprite</returns>
        public static Sprite LoadFlag(string filename)
        {
            if (!Directory.Exists(FlagsPath))
                Directory.CreateDirectory(FlagsPath);

            return LoadSprite(FlagsPath + Path.DirectorySeparatorChar + filename);
        }

        static Game()
        {
            _currentEntities = null;
        }
    }
}
