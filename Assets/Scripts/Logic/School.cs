using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;
using Random = UnityEngine.Random;

namespace SangjiagouCore {

    public class School : IPackable<School.Package>, IAIPlanable
    {
        public class Package
        {
            public int PlayerID;
            public string Name;
            public struct ColorType { public float r, g, b; }
            public ColorType Color;
            public List<Scholar.Package> Members;
            public List<string> AllowedPropositionTypes; // ==> List<Type> _allowedPropositionTypes;
        }
        public Package Pack()
        {
            Package pkg = new Package {
                PlayerID = _playerID,
                Name = _name,
                Color = new Package.ColorType(),
                Members = new List<Scholar.Package>(),
                AllowedPropositionTypes = new List<string>()
            };
            pkg.Color.r = _color.r;
            pkg.Color.g = _color.g;
            pkg.Color.b = _color.b;
            foreach (var m in _members) {
                pkg.Members.Add(m.Pack());
            }
            foreach (var p in _allowedPropositionTypes) {
                pkg.AllowedPropositionTypes.Add(p.Name);
            }
            return pkg;
        }
        Package _pkg;
        public void Unpack(Package pkg)
        {
            _pkg = pkg;
            _playerID = _pkg.PlayerID;
            _name = _pkg.Name;
            _color = new Color(_pkg.Color.r, _pkg.Color.g, _pkg.Color.b);
            _members = new List<Scholar>();
            _allowedPropositionTypes = new List<Type>();
            foreach (var mp in _pkg.Members) {
                _members.Add(new Scholar(mp, this));
            }
            foreach (var pp in _pkg.AllowedPropositionTypes) {
                _allowedPropositionTypes.Add(Type.GetType($"{GetType().Namespace}.{pp}"));
            }
        }
        public void Relink()
        {
            if (_pkg is null)
                return;

            foreach (var m in _members) {
                m.Relink();
            }

            _pkg = null;
        }

        int _playerID;
        /// <summary>
        /// 玩家标识符，若为0则是AI
        /// </summary>
        public int PlayerID {
            set {
                _playerID = value;
            }
            get => _playerID;
        }
        public const int AIControl = 0;
        /// <summary>
        /// 是否是AI控制，即玩家标识符是否为0
        /// </summary>
        public bool IsAI => _playerID == AIControl;

        string _name;
        /// <summary>
        /// 
        /// </summary>
        public string Name => _name;

        Color _color;
        /// <summary>
        /// 该家的代表颜色
        /// </summary>
        public Color Color => _color;

        List<Scholar> _members;
        /// <summary>
        /// 
        /// </summary>
        public List<Scholar> Members => _members;
        /// <summary>
        /// 
        /// </summary>
        public Scholar Leader => _members[0];

        List<Type> _allowedPropositionTypes;
        /// <summary>
        /// 
        /// </summary>
        public List<Type> AllowedPropositionTypes => _allowedPropositionTypes;
        /// <summary>
        /// 
        /// </summary>
        public List<Type> DisallowedPropositionTypes {
            get {
                var l = new List<Type>(Game.CurrentEntities.AllStateActionTypes);
                foreach (var t in _allowedPropositionTypes) {
                    l.Remove(t);
                }
                return l;
            }
        }

        /// <summary>
        /// 根据姓名或者姓字查找该学派内的学者
        /// </summary>
        /// <param name="name">姓名或者姓字。不能是单独的姓、名或字</param>
        /// <returns>找到则返回该学者，否则返回null</returns>
        public Scholar FindScholarByName(string name)
        {
            foreach (var s in _members) {
                if (s.FullName == name || s.FullCourtesyName == name)
                    return s;
            }
            return null;
        }

        public void AIPlan()
        {
            List<Scholar> unassigned = new List<Scholar>(Members);  // 还未分配任务的诸子列表

            #region 选取影响力占比最小的国家增加影响力

            List<State> states = new List<State>(Game.CurrentEntities.States);
            states.Sort((State x, State y) => {             // 按影响力占比从小到大给各国排序
                float rx = x.InfluenceRatio(this);
                float ry = y.InfluenceRatio(this);
                if (rx < ry) return -1;
                return rx == ry ? 0 : 1;
            });
            /*
            unassigned.Sort((Scholar x, Scholar y) => {     // 按诡辩从大到小给诸子排序
                if (x.Sophistry < y.Sophistry) return -1;
                return x.Sophistry == y.Sophistry ? 0 : 1;
            });
            */
            for (int i = 0; i < states.Count; ++i) {
                if (unassigned.Count == 0)
                    return;
                if (states[i].InfluenceRatio(this) > 0.4f)
                    continue;
                if (unassigned[0].Location != states[i].Capital)
                    unassigned[0].Action = new TravelAction(unassigned[0], unassigned[0].Location, states[i].Capital);
                else
                    unassigned[0].Action = new LobbyingAction(unassigned[0], unassigned[0].Location);
                unassigned.RemoveAt(0);
                break;
            }

            #endregion

            #region 检查可进行不义战且有实力差距的两个国家提议战争

            if (AllowedPropositionTypes.Contains(typeof(DeclareAggressiveWarAction))) {
                List<Scholar> assigned = new List<Scholar>();
                foreach (var s in unassigned) {
                    if (!s.Location.IsCapital) continue;
                    var state = s.Location.Controller;
                    foreach (Town t in state.NeighbourTowns) {
                        if (t.Controller.Army < Normal.Sample(mean: 0.7, stddev: 0.2) * state.Army) {
                            s.Action = new ProposeAction(s, s.Location, new DeclareAggressiveWarAction(state, this, t.Controller, t));
                            assigned.Add(s);
                            continue;
                        }
                    }
                }
                foreach (var s in assigned) {
                    unassigned.Remove(s);
                }
            }

            #endregion

            #region 检查可进行义战且有实力差距的两个国家提议战争

            if (AllowedPropositionTypes.Contains(typeof(DeclareWarAction))) {
                List<Scholar> assigned = new List<Scholar>();
                foreach (var s in unassigned) {
                    if (!s.Location.IsCapital) continue;
                    var state = s.Location.Controller;
                    foreach (var n in state.Neighbours) {
                        if (n.Monarch.IsInvader && n.Army < Normal.Sample(mean: 0.7, stddev: 0.2) * state.Army) {
                            s.Action = new ProposeAction(s, s.Location, new DeclareWarAction(state, this, n));
                            assigned.Add(s);
                            continue;
                        }
                    }
                }
                foreach (var s in assigned) {
                    unassigned.Remove(s);
                }
            }

            #endregion

            #region 随机选取一个城进行营造

            if (AllowedPropositionTypes.Contains(typeof(DevelopAction))) {
                List<Scholar> assigned = new List<Scholar>();
                foreach (var s in unassigned) {
                    if (!s.Location.IsCapital) continue;
                    var state = s.Location.Controller;
                    var towns = state.Territory;
                    s.Action = new ProposeAction(s, s.Location, new DevelopAction(state, s.BelongTo, towns[Random.Range(0, towns.Count)]));
                }
                foreach (var s in assigned) {
                    unassigned.Remove(s);
                }
            }

            #endregion
        }

        public void NextTurn()
        {
            if (IsAI)
                AIPlan();

            foreach (var s in _members) {
                s.DoAction();
            }
        }




        public School()
        {
            _name = "";
            _members = new List<Scholar>();
            _allowedPropositionTypes = new List<Type>();
        }
        public School(Package pkg)
        {
            Unpack(pkg);
        }

        public School(string name, List<Scholar> members, List<Type> allowedPropositions)
        {
            _name = name;
            _members = members;
            _allowedPropositionTypes = allowedPropositions;
        }
    }

}