using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore
{

    /// <summary>
    /// 游戏逻辑中的所有实体及状态
    /// </summary>
    public class Entities : IPackable<Entities.Package>
    {
        /// <summary>
        /// 将该类所有实际存在的实体加入一个类以打包为存档文件
        /// </summary>
        public class Package
        {
            public int TotalMonth;
            public List<School.Package> Schools;
            public List<State.Package> States;
            public List<Town.Package> Towns;
            public List<List<string>> Roads; // ==> HashSet<(Town, Town)> _roads;
        }
        public Package Pack()
        {
            Package pkg = new Package {
                TotalMonth = _totalMonth,
                Schools = new List<School.Package>(),
                States = new List<State.Package>(),
                Towns = new List<Town.Package>(),
                Roads = new List<List<string>>()
            };
            foreach (var s in _schools) {
                pkg.Schools.Add(s.Pack());
            }
            foreach (var s in _states) {
                pkg.States.Add(s.Pack());
            }
            foreach (var t in _towns) {
                pkg.Towns.Add(t.Pack());
            }
            foreach(var r in _roads) {
                pkg.Roads.Add(new List<string> { r.Item1.Name, r.Item2.Name });
            }
            return pkg;
        }
        Package _pkg;
        public void Unpack(Package pkg)
        {
            _pkg = pkg;
            _totalMonth = _pkg.TotalMonth;
            _schools = new List<School>();
            foreach (var sp in _pkg.Schools) {
                School s = new School();
                s.Unpack(sp);
                _schools.Add(s);
            }
            _states = new List<State>();
            foreach (var sp in _pkg.States) {
                State s = new State();
                s.Unpack(sp);
                _states.Add(s);
            }
            _towns = new List<Town>();
            foreach (var tp in _pkg.Towns) {
                Town t = new Town();
                t.Unpack(tp);
                _towns.Add(t);
            }
        }
        public void Relink()
        {
            if (_pkg is null)
                return;

            foreach (var s in _schools) {
                s.Relink();
            }
            foreach (var s in _states) {
                s.Relink();
            }
            foreach (var t in _towns) {
                t.Relink();
            }

            _roads = new HashSet<(Town, Town)>();
            foreach(var r in _pkg.Roads) {
                Town town1 = null, town2 = null;
                foreach(var t in _towns) {
                    if(t.Name == r[0]) {
                        town1 = t;
                        break;
                    }
                }
                foreach (var t in _towns) {
                    if (t.Name == r[1]) {
                        town2 = t;
                        break;
                    }
                }
                _roads.Add((town1, town2));
            }

            _pkg = null;
        }

        int _totalMonth;
        public int TotalMonth => _totalMonth;
        /// <summary>
        /// 当前月份
        /// </summary>
        public int Month => (_totalMonth - 1) % 12 + 1;
        /// <summary>
        /// 当前年份，自月份除12取整获得
        /// </summary>
        public int Year => _totalMonth / 12 + 1;

        List<School> _schools;
        /// <summary>
        /// 百家
        /// </summary>
        public List<School> Schools => _schools;

        /// <summary>
        /// 诸子
        /// </summary>
        public List<Scholar> Scholars {
            get {
                List<Scholar> r = new List<Scholar>();
                foreach (var s in _schools) {
                    r.AddRange(s.Members);
                }
                return r;
            }
        }

        List<State> _states;
        /// <summary>
        /// 所有诸侯国
        /// </summary>
        public List<State> States => _states;

        /// <summary>
        /// 天下人口
        /// </summary>
        public int TotalPopulation {
            get {
                int res = 0;
                foreach (var s in _states)
                    res += s.Population;
                return res;
            }
        }
        List<Town> _towns;
        /// <summary>
        /// 所有城郭
        /// </summary>
        public List<Town> Towns => _towns;

        HashSet<(Town, Town)> _roads;
        /// <summary>
        /// 两城之间有直接道路连接的集合
        /// </summary>
        public HashSet<(Town, Town)> Roads => _roads;

        /// <summary>
        /// 进行下一回合
        /// </summary>
        public void NextTurn()
        {
            ++_totalMonth;
            foreach(var s in _schools){
                s.NextTurn();
            }
            foreach (var s in _states) {
                s.NextTurn();
            }
        }

        public Entities()
        {
            _totalMonth = 1;
            _schools = new List<School>();
            _states = new List<State>();
            _towns = new List<Town>();
            _roads = new HashSet<(Town, Town)>();
        }
    }

}