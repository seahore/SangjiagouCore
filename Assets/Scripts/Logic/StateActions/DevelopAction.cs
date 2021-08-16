using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SangjiagouCore {

    /// <summary>
    /// 针对城郭进行营造的行动
    /// </summary>
    public class DevelopAction : StateAction, IReportable<DevelopAction.Report>
    {
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            int _changeOfInfluence;
            public int ChangeOfInfluence => _changeOfInfluence;
            int _developmentIncrease;
            public int DevelopmentIncrease => _developmentIncrease;
            public Report(bool successful, int changeOfInfluence, int developmentIncrease)
            {
                _successful = successful;
                _changeOfInfluence = changeOfInfluence;
                _developmentIncrease = developmentIncrease;
            }
        }
        Report _report;


        protected Town _target;
        public Town Target => _target;

        public DevelopAction(State actor, School proposer, Town target):
            base(actor, proposer)
        {
            if (!(actor is null || target is null) && !actor.Territory.Contains(target)) {
                Debug.LogWarning("不能在别国领土进行营造");
            }
            _target = target;
        }
    
        public override float Assess()
        {
            #region 统计必要条件

            bool targetIsUnderControl = false;
            foreach (var t in _actor.Territory) {
                if (_target == t) {
                    targetIsUnderControl = true;
                    break;
                }
            }
            if(!targetIsUnderControl)
                return CANNOT_ACT;
            #endregion

            #region 统计促进条件

            float result = 0.0f;

            // 该国人口占天下人口较小
            int t1 = Game.CurrentEntities.TotalPopulation / (int)Game.CurrentEntities.States.Count;
            if (_actor.Population < t1)
                result += 10.0f;
            else if (_actor.Population < 1.5 * t1)
                result += 5.0f;
            else if (_actor.Population < 2 * t1)
                result += 2.0f;

            // 该城发展度低于平均值
            if (_target.Development < _actor.TotalDevelopment / (0.6f * (int)_actor.Territory.Count))
                result += 5.0f;
            else if (_target.Development < _actor.TotalDevelopment / (int)_actor.Territory.Count)
                result += 2.0f;

            #endregion

            #region 统计不利条件

            // 如果要执行营造的城郭是边境
            foreach (var t in Game.CurrentEntities.Towns) {
                if (Game.CurrentEntities.Roads.Contains((t, _target)) && t.Controller != _actor) {
                    result -= 5.0f;
                }
            }

            #endregion

            return result;
        }

        public override void Act() {
            int increase = (int)(Random.Range(50, 150) * (1 + (_actor.PoliTech * 0.1f)));
            _target.Development += increase;

            _report = new Report(true, 0, increase);
        }

        public Report GetReport() => _report;

        public override string ToString() => $"<b>{_actor.Name}</b>在<b>{_target.Name}</b>进行营造";
    }

}
