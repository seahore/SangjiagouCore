using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore
{

    public class State : IPackable<State.Package>, IAIPlanable
    {
        public class Package
        {
            public string Name;
            public struct Color { public float r, g, b; }
            public Color PrimaryColor;
            public Color SecondaryColor;
            public string FlagFilename;
            public Monarch.Package Monarch;
            public Monarch.Package CrownPrince;
            public int Politech;
            public int Militech;
            public int Population;
            public int Army;
            public int Satisfaction;
            public int Ceremony;
            public int Food;
            public Dictionary<string, int> AttitudeTowards; // ==>  Dictionary<State, int> _attitudeTowards;
            public Dictionary<string, int> InfluenceOfSchools; // ==> Dictionary<School, int> _influenceOfSchools;
            public List<string> Scholars; // ==> List<Scholar> _scholars;
        }
        public Package Pack()
        {
            Package pkg = new Package {
                Name = _name,
                PrimaryColor = new Package.Color(),
                SecondaryColor = new Package.Color(),
                FlagFilename = _flagFilename,
                Monarch = _monarch.Pack(),
                Politech = _politech,
                Militech = _militech,
                Population = _population,
                Army = _army,
                Satisfaction = _satisfaction,
                Ceremony = _ceremony,
                Food = _food,
                AttitudeTowards = new Dictionary<string, int>(),
                InfluenceOfSchools = new Dictionary<string, int>(),
                Scholars = new List<string>()
            };
            pkg.PrimaryColor.r = _primaryColor.r;
            pkg.PrimaryColor.g = _primaryColor.g;
            pkg.PrimaryColor.b = _primaryColor.b;
            pkg.SecondaryColor.r = _secondaryColor.r;
            pkg.SecondaryColor.g = _secondaryColor.g;
            pkg.SecondaryColor.b = _secondaryColor.b;

            if (_crownPrince is null) {
                pkg.CrownPrince = null;
            } else {
                pkg.CrownPrince = _crownPrince.Pack();
            }

            foreach (var p in _attitudeTowards) {
                pkg.AttitudeTowards.Add(p.Key.Name, p.Value);
            }
            foreach (var p in _influenceOfSchools) {
                pkg.InfluenceOfSchools.Add(p.Key.Name, p.Value);
            }
            foreach (var s in _scholars) {
                pkg.Scholars.Add(s.FullName);
            }

            return pkg;
        }
        Package _pkg;
        public void Unpack(Package pkg)
        {
            _pkg = pkg;
            _name = _pkg.Name;
            _primaryColor = new Color(_pkg.PrimaryColor.r, _pkg.PrimaryColor.g, _pkg.PrimaryColor.b);
            _secondaryColor = new Color(_pkg.SecondaryColor.r, _pkg.SecondaryColor.g, _pkg.SecondaryColor.b);
            _flagFilename = _pkg.FlagFilename;
            _flag = Game.LoadFlag(_pkg.FlagFilename);
            _monarch = new Monarch(_pkg.Monarch);
            if (pkg.CrownPrince is null) {
                _crownPrince = null;
            } else {
                _crownPrince = new Monarch(_pkg.CrownPrince);
            }
            _politech = _pkg.Politech;
            _militech = _pkg.Militech;
            _population = _pkg.Population;
            _army = _pkg.Army;
            _satisfaction = _pkg.Satisfaction;
            _ceremony = _pkg.Ceremony;
            _food = _pkg.Food;
            _actionQueue = new List<StateAction>();
        }
        public void Relink()
        {
            if (_pkg is null)
                return;

            _attitudeTowards = new Dictionary<State, int>();
            foreach (var p in _pkg.AttitudeTowards) {
                foreach (var s in Game.CurrentEntities.States) {
                    if (s.Name == p.Key) {
                        _attitudeTowards.Add(s, p.Value);
                        break;
                    }
                }
            }
            _influenceOfSchools = new Dictionary<School, int>();
            foreach (var p in _pkg.InfluenceOfSchools) {
                foreach (var s in Game.CurrentEntities.Schools) {
                    if (s.Name == p.Key) {
                        _influenceOfSchools.Add(s, p.Value);
                        break;
                    }
                }
            }
            _scholars = new List<Scholar>();
            foreach (var sn in _pkg.Scholars) {
                foreach (var s in Game.CurrentEntities.Scholars) {
                    if (s.FullName == sn) {
                        _scholars.Add(s);
                        break;
                    }
                }
            }

            _pkg = null;
        }




        string _name;
        /// <summary>
        /// ????????????
        /// </summary>
        public string Name => _name;

        Color _primaryColor;
        Color _secondaryColor;
        /// <summary>
        /// ?????????????????????
        /// </summary>
        public Color PrimaryColor => _primaryColor;
        /// <summary>
        /// ?????????????????????
        /// </summary>
        public Color SecondaryColor => _secondaryColor;

        string _flagFilename;
        Sprite _flag;
        /// <summary>
        /// ??????????????????????????????
        /// </summary>
        public Sprite Flag => _flag;

        Monarch _monarch;
        /// <summary>
        /// ???????????????
        /// </summary>
        public Monarch Monarch => _monarch;

        Monarch _crownPrince;
        /// <summary>
        /// ???????????????
        /// </summary>
        public Monarch CrownPrince => _crownPrince;

        /// <summary>
        /// ?????????????????????
        /// </summary>
        public List<Town> Territory {
            get {
                List<Town> t = new List<Town>();
                foreach (var i in Game.CurrentEntities.Towns) {
                    if (i.Controller == this) {
                        t.Add(i);
                    }
                }
                return t;
            }
        }
        /// <summary>
        /// ????????????
        /// </summary>
        public Town Capital {
            get {
                foreach (var t in Territory) {
                    if (t.IsCapital) return t;
                }
                Debug.LogWarning(_name + " ???????????????????????????");
                return null;
            }
        }

        int _politech;
        public int Politech { get => _politech; set => _politech = value; }
        public int Politics => _politech + _monarch.PoliticsAbility;


        int _militech;
        public int Militech { get => _militech; set => _militech = value; }
        public int Military => Militech + _monarch.MilitaryAbility;



        int _population;
        /// <summary>
        /// ????????????
        /// </summary>
        public int Population {
            get => _population;
            set {
                if (value > MaxPopulation) _population = MaxPopulation;
                else if (value < 0) _population = 0;
                else _population = value;
            }
        }

        /// <summary>
        /// ?????????????????????
        /// </summary>
        public int TotalDevelopment {
            get {
                int r = 0;
                foreach (var i in Territory) {
                    r += i.Development;
                }
                return r;
            }
        }

        /// <summary>
        /// ???????????????????????????????????????
        /// </summary>
        public int MaxPopulation => TotalDevelopment;

        int _army;
        /// <summary>
        /// ??????????????????
        /// </summary>
        public int Army {
            get => _army;
            set {
                if (value > MaxArmy) _army = MaxArmy;
                else if (value < 0) _army = 0;
                else _army = value;
            }
        }

        Dictionary<State, int> _attitudeTowards;
        /// <summary>
        /// ??????????????????????????????
        /// </summary>
        public Dictionary<State, int> AttitudeTowards => _attitudeTowards;
        /// <summary>
        /// ????????????????????????????????????
        /// </summary>
        public int StandingArmy => (int)(0.15f * (float)_population);
        /// <summary>
        /// ???????????????????????????????????????
        /// </summary>
        public int MaxArmy => (int)(0.3f * (float)_population);

        int _satisfaction;
        /// <summary>
        /// ????????????
        /// </summary>
        public int Satisfaction { get => _satisfaction; set => _satisfaction = value; }

        int _ceremony;
        /// <summary>
        /// ????????????
        /// </summary>
        public int Ceremony { get => _ceremony; set => _ceremony = value; }

        int _food;
        /// <summary>
        /// ??????????????????
        /// </summary>
        public int Food { get => _food; set => _food = value; }

        Dictionary<School, int> _influenceOfSchools;
        /// <summary>
        /// ???????????????????????????
        /// </summary>
        public Dictionary<School, int> InfluenceOfSchools => _influenceOfSchools;

        List<Scholar> _scholars;
        /// <summary>
        /// ?????????????????????
        /// </summary>
        public List<Scholar> Scholars => _scholars;
        // ??????Scholar????????????????????????????????????State?????????????????????????????????Scholar??????????????????????????????


        public HashSet<Town> NeighbourTowns {
            get {
                HashSet<Town> s = new HashSet<Town>();
                foreach(var t in Territory) {
                    foreach (var i in t.HasRoadTo) {
                        if (i.Controller != this)
                            s.Add(i);
                    }
                }
                return s;
            }
        }

        public HashSet<State> Neighbours {
            get {
                HashSet<State> s = new HashSet<State>();
                foreach (var t in NeighbourTowns) {
                    s.Add(t.Controller);
                }
                return s;
            }
        }





        StateAction _formerAction;
        public StateAction FormerAction => _formerAction;

        StateAction _selectedAction;

        List<StateAction> _actionQueue;
        public List<StateAction> ActionQueue => _actionQueue;




        public float InfluenceRatio(School school)
        {
            int sum = 0;
            foreach (var p in InfluenceOfSchools.Values) {
                sum += p;
            }
            return (float)InfluenceOfSchools[school] / sum;
        }



        int PopulationUpdate()
        {
            int newPopulation = (int)((1.01f + 0.0002f * _satisfaction) * _population);
            if (newPopulation > MaxPopulation) return MaxPopulation;
            else return newPopulation;
        }

        int SatisfactionUpdate()
        {
            int newSatisfaction = (int)(_satisfaction - 2 + 0.02f * _ceremony + 0.01f * Politics);
            if (_food > 0 && _food < (int)(0.05f * _population)) {
                newSatisfaction -= 3;
            } else if (_food == 0) {
                newSatisfaction -= 15;
            }
            return newSatisfaction;
        }

        int FoodUpdate()
        {
            float[] Harvest = { 0.0f, 1.0f, 1.2f, 1.2f, 1.2f, 1.1f, 1.3f, 1.8f, 2.4f, 2.0f, 1.5f, 1.1f, 1.0f };
            int newFood = (int)(_food - 0.01f * _population + 0.01f * (0.6f * _army + 0.6f * _population) * 0.01f * _satisfaction * (1.0f + 0.01f * Politics) * Harvest[Game.CurrentEntities.Month]);
            return newFood;
        }

        int ArmyUpdate()
        {
            if (_army < StandingArmy) {
                int newArmy = _army + (int)(0.02f * _population * 0.01f * _satisfaction);
                return newArmy;
            }
            return _army;
        }


        /// <summary>
        /// ??????????????????????????????<strong>??????</strong>?????????
        /// </summary>
        /// <param name="war">??????????????????????????????</param>
        /// <param name="asAttacker">???????????????????????????</param>
        /// <returns>????????????????????????????????????</returns>
        public bool WouldEnterWar(War war, bool asAttacker)
        {
            if (_name == war.Declarer._name || _name == war.Declaree._name) {
                return false;
            }
            float possibility = 0.2f;
            if (asAttacker) {
                possibility += 0.005f * (_attitudeTowards[war.Declarer] - _attitudeTowards[war.Declaree]);
            } else {
                possibility += 0.005f * (_attitudeTowards[war.Declaree] - _attitudeTowards[war.Declarer]);
            }
            return Random.Range(0.0f, 1.0f) < possibility;
        }
        /// <summary>
        /// ??????????????????????????????<strong>?????????</strong>?????????
        /// </summary>
        /// <param name="war">?????????????????????????????????</param>
        /// <param name="asAttacker">???????????????????????????</param>
        /// <returns>???????????????????????????????????????</returns>
        public bool WouldEnterAggressiveWar(AggressiveWar war, bool asAttacker)
        {
            float possibility = 0.2f;
            if (asAttacker) {
                possibility += 0.005f * (_attitudeTowards[war.Declarer] - _attitudeTowards[war.Declaree]);
            } else {
                possibility += 0.005f * (_attitudeTowards[war.Declaree] - _attitudeTowards[war.Declarer]);
            }
            return Random.Range(0.0f, 1.0f) < possibility;
        }


        /// <summary>
        /// ??????
        /// </summary>
        /// <param name="destination">?????????????????????????????????null??????????????????????????????????????????????????????</param>
        public void MoveCapital(Town destination = null)
        {
            List<Town> ter = Territory;
            if (destination is null) {
                Town maxDev = ter[0];
                for (int i = 1; i < ter.Count; i++) {
                    if (ter[i].Development > maxDev.Development)
                        maxDev = ter[i];
                }
                Capital.IsCapital = false;
                maxDev.IsCapital = true;
            } else {
                Capital.IsCapital = false;
                destination.IsCapital = true;
            }
        }

        public void AIPlan()
        {
            if (ActionQueue.Count != 0) {
                ActionQueue.Sort((StateAction x, StateAction y) => {
                    float ax = x.Assess(), ay = y.Assess();
                    if (ax > ay) return 1;
                    return ax == ay ? 0 : -1;
                });
                _selectedAction = ActionQueue[0];
            } else {
                _selectedAction = null;
            }
        }


        /// <summary>
        /// ??????????????????
        /// </summary>
        public void NextTurn()
        {
            AIPlan();
            if (!(_selectedAction is null))
                _selectedAction.Act();
            _formerAction = _selectedAction;
            ActionQueue.Clear();

            _population = PopulationUpdate();
            _satisfaction = SatisfactionUpdate();
            _food = FoodUpdate();
            _army = ArmyUpdate();
        }

        public override string ToString() => _name;

        public override bool Equals(object o) => o is State && this == o as State;
        public override int GetHashCode() => _name.GetHashCode();

        public static bool operator ==(State a, State b) => !(a is null || b is null) && a._name == b._name;
        public static bool operator !=(State a, State b) => !(a == b);

        public State() { }
        public State(Package pkg)
        {
            Unpack(pkg);
        }
    }

}