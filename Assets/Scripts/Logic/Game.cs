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

        public readonly static string ScenariosPath = Application.dataPath + Path.DirectorySeparatorChar + "Scenarios";
        public readonly static string SavesPath = Application.dataPath + Path.DirectorySeparatorChar + "Saves";
        public readonly static string AvatarsPath = Application.dataPath + Path.DirectorySeparatorChar + "Avatars";

        static Entities _currentEntities;
        public static Entities CurrentEntities { get => _currentEntities; }
        public static bool Playing => !(_currentEntities is null);

        /// <summary>
        /// 读取存档或开场
        /// </summary>
        /// <param name="fileName">文件名（带扩展名）</param>
        /// <param name="encrypted">是否加密并用Base64编码转换过</param>
        /// <param name="isScenario">是否是开场文件</param>
        public static void LoadGame(string fileName, bool encrypted = true, bool isScenario = false)
        {
            _currentEntities = new Entities();

            string path;
            if (isScenario) {
                if (!Directory.Exists(ScenariosPath))
                    Directory.CreateDirectory(ScenariosPath);
                path = ScenariosPath;
            } else {
                if (!Directory.Exists(SavesPath))
                    Directory.CreateDirectory(SavesPath);
                path = SavesPath;
            }
            path += Path.DirectorySeparatorChar + fileName;
            FileStream file;
            BinaryReader br;
            file = new FileStream(path, FileMode.Open);
            br = new BinaryReader(file);
            string json;
            if (encrypted) {
                byte[] encryptedData = Convert.FromBase64String(br.ReadString());
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream mstream = new MemoryStream();
                CryptoStream cstream = new CryptoStream(mstream, des.CreateDecryptor(_cryptKeys, _cryptKeys), CryptoStreamMode.Write);
                cstream.Write(encryptedData, 0, encryptedData.Length);
                cstream.FlushFinalBlock();
                json = Encoding.UTF8.GetString(mstream.ToArray());
            } else {
                json = br.ReadString();
            }
            br.Close();

            Debug.Log(json);

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

            Debug.Log(json);

            FileStream saveFile;
            BinaryWriter bw;
            saveFile = new FileStream(saveFilePath, FileMode.Create);
            bw = new BinaryWriter(saveFile);
            if (encrypt) {
                byte[] data = Encoding.UTF8.GetBytes(json);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream mstream = new MemoryStream();
                CryptoStream cstream = new CryptoStream(mstream, des.CreateEncryptor(_cryptKeys, _cryptKeys), CryptoStreamMode.Write);
                cstream.Write(data, 0, data.Length);
                cstream.FlushFinalBlock();
                string encryptedData = Convert.ToBase64String(mstream.ToArray());
                bw.Write(encryptedData);
            } else {
                bw.Write(json);
            }
            bw.Close();
        }

        /// <summary>
        /// 读取肖像图片
        /// </summary>
        /// <param name="filename">图片文件名</param>
        /// <returns></returns>
        public static Sprite LoadAvatar(string filename)
        {
            if (!Directory.Exists(AvatarsPath))
                Directory.CreateDirectory(AvatarsPath);

            string path = AvatarsPath + Path.DirectorySeparatorChar + filename;
            if (File.Exists(path)) {
                var ts = filename.Trim().Split('.');
                switch (ts[ts.Length - 1].ToLower()) {
                    case "jpg":
                    case "png":
                        var t2d = new Texture2D(100, 100);
                        t2d.LoadImage(File.ReadAllBytes(path));
                        return Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0, 0));
                }
                throw new BadImageFormatException();
            } else {
                throw new FileNotFoundException("找不到指定的肖像文件：" + path);
            }
        }


        static void TestInit()
        {
            _currentEntities = new Entities();
            Monarch.Package mp1 = new Monarch.Package();
            mp1.Name = "鲁庄公";
            mp1.Aggressiveness = -50;
            State.Package sp1 = new State.Package();
            sp1.PrimaryColor = new State.Package._Color { r = 0.70f, g = 0.20f, b = 0.40f };
            sp1.Name = "鲁";
            sp1.Monarch = mp1;
            sp1.Food = 1000000;
            sp1.Satisfaction = 80;
            sp1.Ceremony = 90;
            sp1.PoliTech = 50;
            sp1.MiliTech = 40;
            sp1.Scholars = new List<string> { "孔丘", "颜回" };
            sp1.InfluenceOfSchools = new Dictionary<string, int> {
                { "儒家", 100 },
                { "墨家", 20 }
            };
            sp1.CrownPrince = null;
            sp1.Population = 30000;
            sp1.Army = 4200;
            sp1.AttitudeTowards = new Dictionary<string, int> {
                { "齐", 50 }
            };
            State lu = new State(sp1);
            Monarch.Package mp2 = new Monarch.Package();
            mp2.Name = "齐桓公";
            mp2.Aggressiveness = 90;
            State.Package sp2 = new State.Package();
            sp2.PrimaryColor = new State.Package._Color { r = 0.10f, g = 0.80f, b = 0.50f };
            sp2.Name = "齐";
            sp2.Monarch = mp2;
            sp2.Food = 1500000;
            sp2.Satisfaction = 50;
            sp2.Ceremony = 50;
            sp2.PoliTech = 30;
            sp2.MiliTech = 30;
            sp2.Scholars = new List<string> { "冉耕" };
            sp2.InfluenceOfSchools = new Dictionary<string, int> {
                { "儒家", 50 },
                { "墨家", 60 }
            };
            sp2.CrownPrince = null;
            sp2.Population = 100000;
            sp2.Army = 8400;
            sp2.AttitudeTowards = new Dictionary<string, int> {
                { "鲁", -10 }
            };
            State qi = new State(sp2);
            List<State> stl = new List<State> { lu, qi };
            List<Town> tl = new List<Town> {
                new Town("曲阜", new Vector2Int(3, 2), lu, 40000, true, new Dictionary<School, List<Scholar>>()),
                new Town("单父", new Vector2Int(2, 5), lu, 10000, false, new Dictionary<School, List<Scholar>>()),
                new Town("临淄", new Vector2Int(1, 1), qi, 80000, true, new Dictionary<School, List<Scholar>>()),
                new Town("莒", new Vector2Int(6, 2), qi, 20000, false, new Dictionary<School, List<Scholar>>())
            };
            _currentEntities.Roads.Add((tl[0], tl[1]));
            _currentEntities.Roads.Add((tl[1], tl[0]));
            _currentEntities.Roads.Add((tl[1], tl[2]));
            _currentEntities.Roads.Add((tl[2], tl[1]));
            _currentEntities.Roads.Add((tl[2], tl[3]));
            _currentEntities.Roads.Add((tl[3], tl[2]));
            School rujia = new School("儒家", new List<Scholar>(), new List<Type> { typeof(DeclareWarAction), typeof(DeclareAggressiveWarAction), typeof(DevelopAction) });
            School mojia = new School("墨家", new List<Scholar>(), new List<Type> { typeof(DeclareWarAction), typeof(DeclareAggressiveWarAction), typeof(DevelopAction) });
            List<Scholar> lsQufu = new List<Scholar> {
                new Scholar("孟", "轲", "子舆", 40, 80, null, tl[0], "TogashiYuta.jpg")
            };
            tl[0].Recruits[rujia] = lsQufu;
            List<Scholar> scl1 = new List<Scholar> {
                new Scholar("孔", "丘", "仲尼", 70, 50, rujia, tl[0], "NibutaniShinka.jpg"),
                new Scholar("颜", "回", "子渊", 40, 50, rujia, tl[0], "KannagiKazari.jpg"),
                new Scholar("闵", "损", "子骞", 55, 50, rujia, tl[1], "TakanashiToka.jpg"),
                new Scholar("冉", "耕", "伯牛", 63, 50, rujia, tl[2], "TsuyuriKumin.jpg")
            };
            List<Scholar> scl2 = new List<Scholar> {
                new Scholar("墨", "翟", "翟", 30, 100, mojia, tl[2], "TakanashiRikka.jpg"),
                new Scholar("禽", "滑釐", "慎子", 20, 60, mojia, tl[3], "DekomoriSanae.jpg"),
                new Scholar("胡", "非", "非子", 20, 60, mojia, tl[3], "ShichimiyaSatone.jpg"),
            };
            rujia.Members.AddRange(scl1);
            mojia.Members.AddRange(scl2);
            List<School> schl = new List<School> { rujia, mojia };
            _currentEntities.Schools.AddRange(schl);
            _currentEntities.States.AddRange(stl);
            _currentEntities.Towns.AddRange(tl);
            foreach (var t in _currentEntities.Schools) {
                t.Relink();
            }
            foreach (var t in _currentEntities.States) {
                t.Relink();
            }
            foreach (var t in _currentEntities.Towns) {
                t.Relink();
            }
            foreach (var t in _currentEntities.Schools) {
                t.PlayerID = t.Name == "儒家" ? 1 : 0;
            }
        }



        static Game()
        {
            _currentEntities = null;


            #region Debug时使用的预设值

            TestInit();

            #endregion
        }
    }
}
