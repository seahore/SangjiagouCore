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
            public struct _Color { public float r, g, b; }
            public _Color PrimaryColor;
            public _Color SecondaryColor;
            public Monarch.Package Monarch;
            public Monarch.Package CrownPrince;
            public int PoliTech;
            public int MiliTech;
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
                PrimaryColor = new Package._Color(),
                SecondaryColor = new Package._Color(),
                Monarch = _monarch.Pack(),
                PoliTech = _poliTech,
                MiliTech = _miliTech,
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
            _monarch = new Monarch(_pkg.Monarch);
            if (pkg.CrownPrince is null) {
                _crownPrince = null;
            } else {
                _crownPrince = new Monarch(_pkg.CrownPrince);
            }
            _poliTech = _pkg.PoliTech;
            _miliTech = _pkg.MiliTech;
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
        /// 该国之名
        /// </summary>
        public string Name => _name;

        Color _primaryColor;
        Color _secondaryColor;
        /// <summary>
        /// 该国的主要颜色
        /// </summary>
        public Color PrimaryColor => _primaryColor;
        /// <summary>
        /// 该国的次要颜色
        /// </summary>
        public Color SecondaryColor => _secondaryColor;

        Monarch _monarch;
        /// <summary>
        /// 该国之君主
        /// </summary>
        public Monarch Monarch => _monarch;

        Monarch _crownPrince;
        /// <summary>
        /// 该国之太子
        /// </summary>
        public Monarch CrownPrince => _crownPrince;

        /// <summary>
        /// 该国之所有城郭
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
        /// 该国京城
        /// </summary>
        public Town Capital {
            get {
                foreach (var t in Territory) {
                    if (t.IsCapital) return t;
                }
                Debug.LogWarning(_name + " 没有找到境内的京城");
                return null;
            }
        }

        int _poliTech;
        public int PoliTech => _poliTech;
        public int Politics => _poliTech + _monarch.PoliticsAbility;


        int _miliTech;
        public int MiliTech => _miliTech;
        public int Military => MiliTech + _monarch.MilitaryAbility;



        int _population;
        /// <summary>
        /// 该国人口
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
        /// 该国之总发展度
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
        /// 该国之最大人口，即总发展度
        /// </summary>
        public int MaxPopulation => TotalDevelopment;

        int _army;
        /// <summary>
        /// 该国之兵士数
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
        /// 该国对其他各国的态度
        /// </summary>
        public Dictionary<State, int> AttitudeTowards => _attitudeTowards;
        /// <summary>
        /// 该国兵士数自然恢复的上限
        /// </summary>
        public int StandingArmy => (int)(0.15f * (float)_population);
        /// <summary>
        /// 该国兵士数可强制徭役的上限
        /// </summary>
        public int MaxArmy => (int)(0.3f * (float)_population);

        int _satisfaction;
        /// <summary>
        /// 该国民生
        /// </summary>
        public int Satisfaction => _satisfaction;

        int _ceremony;
        /// <summary>
        /// 该国礼乐
        /// </summary>
        public int Ceremony => _ceremony;

        int _food;
        /// <summary>
        /// 该国粮食储量
        /// </summary>
        public int Food => _food;

        Dictionary<School, int> _influenceOfSchools;
        /// <summary>
        /// 各家在该国的影响力
        /// </summary>
        public Dictionary<School, int> InfluenceOfSchools => _influenceOfSchools;

        List<Scholar> _scholars;
        /// <summary>
        /// 在该国朝的诸子
        /// </summary>
        public List<Scholar> Scholars => _scholars;
        // 由于Scholar亦有一个所在国属性，所以State类是否需要另外维护一个Scholar的列表，仍旧应该讨论

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
        /// 该国是否愿意加入指定<strong>义战</strong>的某方
        /// </summary>
        /// <param name="war">欲询问是否参与的义战</param>
        /// <param name="asAttacker">是否作为攻击方参与</param>
        /// <returns>该国是否决定参与指定义战</returns>
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
        /// 该国是否愿意加入指定<strong>不义战</strong>的某方
        /// </summary>
        /// <param name="war">欲询问是否参与的不义战</param>
        /// <param name="asAttacker">是否作为攻击方参与</param>
        /// <returns>该国是否决定参与指定不义战</returns>
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
        /// 迁都
        /// </summary>
        /// <param name="destination">要迁都的目的城郭，若为null，则选取领地内发展度对大的一个为京城</param>
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
                static int ActionCompare(StateAction x, StateAction y)
                {
                    float ax = x.Assess(), ay = y.Assess();
                    if (ax > ay) return 1;
                    return ax == ay ? 0 : -1;
                }
                ActionQueue.Sort(ActionCompare);
                _selectedAction = ActionQueue[0];
            } else {
                _selectedAction = null;
            }
        }


        /// <summary>
        /// 进行下一回合
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



        public State() { }
        public State(Package pkg)
        {
            Unpack(pkg);
        }
    }

}