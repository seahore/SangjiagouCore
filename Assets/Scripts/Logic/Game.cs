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

        static Entities _currentEntities;
        public static Entities CurrentEntities { get => _currentEntities; }

        /// <summary>
        /// 读取存档或开场
        /// </summary>
        /// <param name="fileName">文件名（带扩展名）</param>
        /// <param name="encrypted">是否加密并用Base64编码转换过</param>
        /// <param name="isScenario">是否是开场文件</param>
        public static void LoadGame(string fileName, bool encrypted = true, bool isScenario = false)
        {
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


        static void TestInit()
        {
            Monarch.Package mp1 = new Monarch.Package();
            mp1.Name = "鲁庄公";
            mp1.Aggressiveness = -50;
            State.Package sp1 = new State.Package();
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
            School rujia = new School("儒家", new List<Scholar>(), new List<Type> { typeof(DeclareWarAction) });
            School mojia = new School("墨家", new List<Scholar>(), new List<Type>());
            List<Scholar> lsQufu = new List<Scholar> {
                new Scholar("孟", "轲", "子舆", 40, 80, null, tl[0])
            };
            tl[0].Recruits[rujia] = lsQufu;
            List<Scholar> scl1 = new List<Scholar> {
                new Scholar("孔", "丘", "仲尼", 70, 50, rujia, tl[0]),
                new Scholar("颜", "回", "子渊", 40, 50, rujia, tl[0]),
                new Scholar("闵", "损", "子骞", 55, 50, rujia, tl[1]),
                new Scholar("冉", "耕", "伯牛", 63, 50, rujia, tl[2])
            };
            List<Scholar> scl2 = new List<Scholar> {
                new Scholar("墨", "翟", "翟", 30, 100, mojia, tl[2]),
                new Scholar("禽", "滑釐", "慎子", 20, 60, mojia, tl[3]),
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
        }



        static Game()
        {
            _currentEntities = new Entities();

            TestInit();
        }
    }
}
