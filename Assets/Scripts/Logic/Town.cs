using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore
{

    public class Town : IPackable<Town.Package>
    {
        public class Package
        {
            public string Name;
            public struct _Vector2Int { public int x, y; }
            public _Vector2Int Position;
            public string Controller; // ==> State _controller;
            public int Development;
            public bool IsCapital;
            public Dictionary<string, List<Scholar.Package>> Recruits;
        }
        public Package Pack()
        {
            Package pkg = new Package {
                Name = _name,
                Position = new Package._Vector2Int { x = _position.x , y = _position.y },
                Controller = _controller.Name,
                Development = _development,
                IsCapital = _isCapital,
                Recruits = new Dictionary<string, List<Scholar.Package>>()
            };
            foreach (var r in _recruits) {
                List<Scholar.Package> l = new List<Scholar.Package>();
                foreach (var s in r.Value) {
                    l.Add(s.Pack());
                }
                pkg.Recruits.Add(r.Key.Name, l);
            }

            return pkg;
        }
        Package _pkg;
        Dictionary<string, List<Scholar>> _unlinkRecruits;
        public void Unpack(Package pkg)
        {
            _pkg = pkg;
            _name = _pkg.Name;
            _position = new Vector2Int(_pkg.Position.x, _pkg.Position.y);
            _development = _pkg.Development;
            _isCapital = _pkg.IsCapital;
            _unlinkRecruits = new Dictionary<string, List<Scholar>>();
            foreach (var r in pkg.Recruits) {
                List<Scholar> l = new List<Scholar>();
                foreach (var sp in r.Value) {
                    l.Add(new Scholar(sp, null));
                }
                _unlinkRecruits.Add(r.Key, l);
            }
        }
        public void Relink()
        {
            if (_pkg is null)
                return;

            foreach (var s in Game.CurrentEntities.States) {
                if (_pkg.Controller == s.Name) {
                    _controller = s;
                    break;
                }
            }
            _recruits = new Dictionary<School, List<Scholar>>();
            foreach (var i in _unlinkRecruits) {
                School school = null;
                foreach (var s in Game.CurrentEntities.Schools) {
                    if(i.Key == s.Name) {
                        school = s;
                        break;
                    }
                }
                foreach (var s in i.Value) {
                    s.Relink();
                }
                _recruits.Add(school, i.Value);
            }

            _pkg = null;
        }


        string _name;
        /// <summary>
        /// 城名
        /// </summary>
        public string Name { get => _name; }

        Vector2Int _position;
        /// <summary>
        /// 该城在地图上的位置
        /// </summary>
        public Vector2Int Position { get => _position; }

        State _controller;
        /// <summary>
        /// 统治该城的国家
        /// </summary>
        public State Controller { get => _controller; }

        int _development;
        /// <summary>
        /// 该城发展度
        /// </summary>
        public int Development { get => _development; }

        bool _isCapital;
        /// <summary>
        /// 该城是否作为某国的京城
        /// </summary>
        public bool IsCapital { get => _isCapital; set { _isCapital = value; } }


        Dictionary<School, List<Scholar>> _recruits;
        /// <summary>
        /// 每个学家在该地可招收的成员列表
        /// </summary>
        public Dictionary<School, List<Scholar>> Recruits => _recruits;

        /// <summary>
        /// 将该城割让给receiver
        /// </summary>
        /// <param name="receiver">要割让到的国家</param>
        public void CedeTo(State receiver)
        {
            if (_isCapital) {
                State formerController = _controller;
                _isCapital = false;
                _controller = receiver;
                if (formerController.Territory.Count == 0) {
                    // TODO: 国家被消灭的处理
                    return;
                }
                formerController.Territory[0]._isCapital = true;
                formerController.MoveCapital();
            } else {
                _controller = receiver;
            }
        }


        public Town() { }
        public Town(Package pkg)
        {
            Unpack(pkg);
        }
        public Town(string name, Vector2Int pos, State ctrl, int dev, bool isCap, Dictionary<School, List<Scholar>> recruits)
        {
            _name = name;
            _position = pos;
            _controller = ctrl;
            _development = dev;
            _isCapital = isCap;
            _recruits = recruits;
        }
    }

}
