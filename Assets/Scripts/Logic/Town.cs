using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore {

    public class Town : IPackable<Town.Package>
    {
        public class Package
        {
            public string Name;
            public struct _Vector2 { public float x, y; }
            public _Vector2 Position;
            public string Controller; // ==> State _controller;
            public uint Development;
            public bool IsCapital;
        }
        public Package Pack()
        {
            Package pkg = new Package {
                Name = _name,
                Position = new Package._Vector2(),
                Controller = _controller.Name,
                Development = _development,
                IsCapital = _isCapital
            };
            pkg.Position.x = _position.x;
            pkg.Position.y = _position.y;

            return pkg;
        }
        Package _pkg;
        public void Unpack(Package pkg)
        {
            _pkg = pkg;
            _name = _pkg.Name;
            _position = new Vector2(_pkg.Position.x, _pkg.Position.y);
            _development = _pkg.Development;
            _isCapital = _pkg.IsCapital;
        }
        public void Relink()
        {
            if (_pkg is null)
                return;

            foreach (var s in Game.CurrentEntities.States) {
                if(_pkg.Controller == s.Name) {
                    _controller = s;
                    break;
                }
            }

            _pkg = null;
        }


        string _name;
        public string Name { get => _name; }

        Vector2 _position;
        public Vector2 Position { get => _position; }

        State _controller;
        public State Controller { get => _controller; }

        uint _development;
        public uint Development { get => _development; }

        bool _isCapital;
        public bool IsCapital { get => _isCapital; set { _isCapital = value; } }


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
        public Town(string name, Vector2 pos, State ctrl, uint dev, bool isCap)
        {
            _name = name;
            _position = pos;
            _controller = ctrl;
            _development = dev;
            _isCapital = isCap;
        }
    }

}
