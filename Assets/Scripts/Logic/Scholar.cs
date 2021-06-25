using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore {

    /// <summary>
    /// 诸子
    /// </summary>
    public class Scholar : IPackable<Scholar.Package>
    {
        public class Package
        {
            public string FamilyName;
            public string GivenName;
            public string CourtesyName;
            public int Age;
            // public string BelongTo; // ==> School _belongTo;
            public string Location; // ==> Town _location;
            public int Sophistry;
        }

        public Package Pack()
        {
            Package pkg = new Package {
                FamilyName = _familyName,
                GivenName = _givenName,
                CourtesyName = _courtesyName,
                Age = _age,
                // BelongTo = _belongTo.Name,
                Location = _location.Name,
                Sophistry = _sophistry
            };

            return pkg;
        }
        Package _pkg;
        public void Unpack(Package pkg)
        {
            _pkg = pkg;
            _familyName = _pkg.FamilyName;
            _givenName = _pkg.GivenName;
            _courtesyName = _pkg.CourtesyName;
            _age = _pkg.Age;
            _sophistry = _pkg.Sophistry;
        }
        public void Relink()
        {
            if (_pkg is null)
                return;

            /*
            foreach (var s in Game.CurrentEntities.Schools) {
                if(s.Name == _pkg.BelongTo) {
                    _belongTo = s;
                    break;
                }
            }
            */

            if (_pkg.Location.Length == 0) {
                _location = null;
            } else {
                foreach (var t in Game.CurrentEntities.Towns) {
                    if(t.Name == _pkg.Location) {
                        _location = t;
                        break;
                    }
                }
            }
            _pkg = null;
        }

        string _familyName;
        string _givenName;
        string _courtesyName;
        /// <summary>
        /// 诸子其姓
        /// </summary>
        public string FamilyName => _familyName;
        /// <summary>
        /// 诸子其名
        /// </summary>
        public string GivenName => _givenName;
        /// <summary>
        /// 诸子其字
        /// </summary>
        public string CourtesyName => _courtesyName;
        /// <summary>
        /// 诸子其姓名
        /// </summary>
        public string FullName => _familyName + _givenName;
        /// <summary>
        /// 诸子其姓字
        /// </summary>
        public string FullCourtesyName => _familyName + _courtesyName;

        int _age;
        /// <summary>
        /// 诸子其年寿
        /// </summary>
        public int Age => _age;

        School _belongTo;
        public School BelongTo => _belongTo;

        Town _location;
        /// <summary>
        /// 所在地，如果为京城则代表在某国朝
        /// </summary>
        public Town Location {
            get => _location;
            set {
                if (_location != value) {
                    // 由于每个State对象维护一个在该国朝的Scholars列表,所以离开京城也要修改这些列表
                    if (_location.IsCapital) {
                        _location.Controller.Scholars.Remove(this);
                    }
                    if (value.IsCapital) {
                        value.Controller.Scholars.Add(this);
                    }
                    _location = value;
                }
            }
        }

        int _sophistry;
        /// <summary>
        /// 诸子其诡辩
        /// </summary>
        public int Sophistry { get => _sophistry; }

        ScholarAction _action;
        public ScholarAction Action { get => _action; set { _action = value; } }


        public static Scholar CreateRandomScholar()
        {
            // Todo:
            return new Scholar();
        }



        public Scholar()
        {
            _familyName = _givenName = _courtesyName = "";
            _age = 0;
            _sophistry = 0;
            _location = null;
        }
        public Scholar(Package pkg, School belongTo)
        {
            Unpack(pkg);
            _belongTo = belongTo;
        }
        public Scholar(string familyName, string givenName, string courtesyName, int age, int sophistry, School belongTo, Town location)
        {
            _familyName = familyName;
            _givenName = givenName;
            _courtesyName = courtesyName;
            foreach (var s in Game.CurrentEntities.Scholars) {
                if (s.FullName == this.FullName) {
                    Debug.LogWarning("新建的诸子 " + this.FullName + "已经有同姓名者");
                    break;
                }
            }
            // 同姓字者暂不必检查之
            _age = age;
            _sophistry = sophistry;
            _belongTo = belongTo;
            _location = location;
            _action = null;
        }
    }

}