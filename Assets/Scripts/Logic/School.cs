using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore {

    public class School : IPackable<School.Package>, IAIPlanable
    {
        public class Package
        {
            public int PlayerID;
            public string Name;
            public List<Scholar.Package> Members;
            public List<string> AllowedPropositionTypes; // ==> List<Type> _allowedPropositionTypes;
        }
        public Package Pack()
        {
            Package pkg = new Package {
                PlayerID = _playerID,
                Name = _name,
                Members = new List<Scholar.Package>(),
                AllowedPropositionTypes = new List<string>()
            };
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


        public void AIPlan()
        {
            List<Scholar> unassigned = new List<Scholar>(Members);  // 还未分配任务的诸子列表

            // 选取影响力占比最小的国家增加影响力
            List<State> states = new List<State>(Game.CurrentEntities.States);
            int stateCompare(State x, State y)
            {
                float rx = x.InfluenceRatio(this);
                float ry = y.InfluenceRatio(this);
                if (rx < ry) return -1;
                return rx == ry ? 0 : 1;
            }
            int scholarSophistryCompare(Scholar x, Scholar y)
            {
                if (x.Sophistry < y.Sophistry) return -1;
                return x.Sophistry == y.Sophistry ? 0 : 1;
            }
            states.Sort(stateCompare);
            unassigned.Sort(scholarSophistryCompare);
            for (int i = 0; i < states.Count; ++i) {
                if (unassigned.Count == 0)
                    return;
                if (states[i].InfluenceRatio(this) > 0.4f)
                    break;
                if (unassigned[0].Location.Name != states[i].Capital.Name)
                    unassigned[0].Action = new TravelAction(unassigned[0], unassigned[0].Location, states[i].Capital);
                else
                    unassigned[0].Action = new DiscussWithMonarchAction(unassigned[0], unassigned[0].Location);
            }
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