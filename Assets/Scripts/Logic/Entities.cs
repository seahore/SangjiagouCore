using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SangjiagouCore.Utilities.ChineseNumerals;


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
            public struct _Vector2Int { public int x, y; }
            public int TotalMonth;
            public List<School.Package> Schools;
            public List<State.Package> States;
            public List<Town.Package> Towns;
            public _Vector2Int MapSize;
            public List<List<string>> Roads; // ==> HashSet<(Town, Town)> _roads;
        }
        public Package Pack()
        {
            Package pkg = new Package {
                TotalMonth = _totalMonth,
                Schools = new List<School.Package>(),
                States = new List<State.Package>(),
                Towns = new List<Town.Package>(),
                MapSize = new Package._Vector2Int { x = _mapSize.x, y = _mapSize.y },
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
            foreach (var r in _roads) {
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
            _mapSize = new Vector2Int(pkg.MapSize.x, pkg.MapSize.y);
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
            foreach (var r in _pkg.Roads) {
                Town town1 = null, town2 = null;
                foreach (var t in _towns) {
                    if (t.Name == r[0]) {
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
        public int Year => (_totalMonth - 1) / 12 + 1;
        /// <summary>
        /// 用自然语言表示当前日期
        /// </summary>
        /// <returns>日期的表示</returns>
        public string DateToString() => $"昭公{(Game.CurrentEntities.Year == 1 ? "元" : Int2Chinese(Game.CurrentEntities.Year, false, true))}年{Int2Chinese(Game.CurrentEntities.Month, false, true)}月";

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

        Vector2Int _mapSize;
        /// <summary>
        /// 地图大小
        /// </summary>
        public Vector2Int MapSize => _mapSize;

        /// <summary>
        /// 进行下一回合
        /// </summary>
        public void NextTurn()
        {
            ++_totalMonth;
            foreach (var s in _schools) {
                s.NextTurn();
            }
            List<State> subjugated = new List<State>();
            foreach (var s in _states) {
                if (s.Territory.Count == 0) {
                    subjugated.Add(s);
                    continue;
                }
                s.NextTurn();
            }
            foreach (var s in subjugated) {
                _states.Remove(s);
            }
        }

        public string GetMonthlyReport()
        {
            string text = "";
            foreach (var s in _states) {
                if (s.FormerAction is null) {
                    text += $"<b>{s.Name}</b>前月无所用事。\n\n";
                    continue;
                }
                switch (s.FormerAction.GetType().Name) {
                    case "DeclareWarAction": {
                        var action = (DeclareWarAction)s.FormerAction;
                        var report = action.GetReport();
                        text += $"<b>{action.Actor}</b>在讨伐<b>{action.Declaree}</b>的义战中";
                        text += report.Successful ? "<b><color=maroon>胜利</color></b>" : "<b><color=maroon>败北</color></b>";
                        text += $"，<b>{action.Actor}</b>损失{report.AttackerLoss}人，<b>{action.Declaree}</b>损失{report.DefenderLoss}人。";
                        break;
                    }
                    case "DeclareAggressiveWarAction": {
                        var action = (DeclareAggressiveWarAction)s.FormerAction;
                        var report = action.GetReport();
                        text += $"<b>{action.Actor}</b>在侵略<b>{action.Declaree}</b>并索取<b>{action.Target}</b>的不义战中";
                        text += report.Successful ? "<b><color=maroon>胜利</color></b>" : "<b><color=maroon>败北</color></b>";
                        text += $"，<b>{action.Actor}</b>损失{report.AttackerLoss}人，<b>{action.Declaree}</b>损失{report.DefenderLoss}人。";
                        if (action.Declaree.Territory.Count == 0) {
                            text += $"<b>{action.Declaree}</b>经此一役终告<b><color=maroon>亡国</color></b>。";
                        }
                        break;
                    }
                    case "DevelopAction": {
                        var action = (DevelopAction)s.FormerAction;
                        var report = action.GetReport();
                        text += $"<b>{action.Actor}</b>在<b>{action.Target}</b>进行营造，当地发展度提高了{report.DevelopmentIncrease}。";
                        break;
                    }
                }
                text += "\n\n";
            }
            return text;
        }

        public School GetPlayerSchool(int playerID)
        {
            foreach (var s in _schools) {
                if (s.PlayerID == playerID)
                    return s;
            }
            return null;
        }

        public void AnnounceIndependenceOf(State newState, List<Town> territory)
        {
            Town maxDev = territory[0];
            foreach (var t in territory) {
                t.CedeTo(newState);
                if (t.Development > maxDev.Development) maxDev = t;
            }
            maxDev.IsCapital = true;
            newState.Territory.Clear();
            newState.Territory.AddRange(territory);
        }

        public Entities()
        {
            _totalMonth = 1;
            _schools = new List<School>();
            _states = new List<State>();
            _towns = new List<Town>();
            _roads = new HashSet<(Town, Town)>();
            _mapSize = new Vector2Int(10, 10);
        }
    }
}