using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore {

    public class Monarch : IPackable<Monarch.Package>
    {
        public class Package
        {
            public string Name;
            public int PoliticsAbility;
            public int MilitaryAbility;
            public bool IsInvader;
        }
        public Package Pack()
        {
            Package pkg = new Package {
                Name = _name,
                PoliticsAbility = _politicsAbility,
                MilitaryAbility = _militaryAbility,
                IsInvader = _isInvader
            };
            return pkg;
        }
        Package _pkg;
        public void Unpack(Package pkg)
        {
            _pkg = pkg;
            _name = _pkg.Name;
            _politicsAbility = _pkg.PoliticsAbility;
            _militaryAbility = _pkg.MilitaryAbility;
            _isInvader = _pkg.IsInvader;
        }
        public void Relink()
        {
            if (_pkg is null)
                return;

            _pkg = null;
        }

        string _name;
        public string Name { get => _name; }

        int _politicsAbility;
        public int PoliticsAbility { get => _politicsAbility; }

        int _militaryAbility;
        public int MilitaryAbility { get => _militaryAbility; }

        bool _isInvader;
        public bool IsInvader { get => _isInvader; set { _isInvader = value; } }

        public Monarch(Package pkg)
        {
            Unpack(pkg);
        }
        public Monarch(string name, int politicsAbility, int militaryAbility) {
            _name = name;
            _politicsAbility = politicsAbility;
            _militaryAbility = militaryAbility;
            _isInvader = false;
        }
    }

}