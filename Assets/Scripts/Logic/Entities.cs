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
            public uint TotalMonth;
            public List<School.Package> Schools;
            public List<State.Package> States;
            public List<Town.Package> Towns;
        }
        public Package Pack()
        {
            Package pkg = new Package {
                TotalMonth = _totalMonth,
                Schools = new List<School.Package>(),
                States = new List<State.Package>(),
                Towns = new List<Town.Package>()
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

            _pkg = null;
        }

        uint _totalMonth;
        public uint TotalMonth => _totalMonth;
        /// <summary>
        /// 当前月份
        /// </summary>
        public uint Month => (_totalMonth - 1) % 12 + 1;
        /// <summary>
        /// 当前年份，自月份除12取整获得
        /// </summary>
        public uint Year => _totalMonth / 12 + 1;

        List<School> _schools;
        /// <summary>
        /// 百家
        /// </summary>
        public List<School> Schools { get => _schools; }

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
        public List<State> States { get => _states; }

        List<Town> _towns;
        /// <summary>
        /// 所有城郭
        /// </summary>
        public List<Town> Towns { get => _towns; }


        /// <summary>
        /// 进行下一回合
        /// </summary>
        public void NextTurn()
        {
            ++_totalMonth;
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
        }
    }

}