using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore {

    /// <summary>
    /// 发动不义战行动
    /// </summary>
    public class DeclareAggressiveWarAction : DeclareWarAction, IReportable<DeclareAggressiveWarAction.Report>
    {
        public new struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            int _changeOfInfluence;
            public int ChangeOfInfluence => _changeOfInfluence;
            int _attackerLoss;
            public int AttackerLoss => _attackerLoss;
            int _defenderLoss;
            public int DefenderLoss => _defenderLoss;
            public Report(bool successful, int changeOfInfluence, int attackerLoss, int defenderLoss)
            {
                _successful = successful;
                _changeOfInfluence = changeOfInfluence;
                _attackerLoss = attackerLoss;
                _defenderLoss = defenderLoss;
            }
        }
        Report _report;

        Town _target;
        public Town Target => _target;

        const int INFLUENCE_INCREASE_IF_WON = 20;
        const int INFLUENCE_DECREASE_IF_LOST = 40;

        public DeclareAggressiveWarAction(State actor, School proposer, State declaree, Town target):
            base(actor, proposer, declaree)
        {
            _target = target;
        }

        public override float Assess() {
            return 0.0f;
        }
    

        public override void Act() {
            AggressiveWar war = new AggressiveWar(_actor, _declaree, _target);
            war.Settle();
            AggressiveWar.Report report = war.GetReport();
            if(report.AttackerWon) {
                _target.CedeTo(_actor);
                _actor.InfluenceOfSchools[_proposer] += INFLUENCE_INCREASE_IF_WON;
                _report = new Report(true, INFLUENCE_INCREASE_IF_WON, report.AttackerLoss, report.DefenderLoss);
            } else {
                int changeOfInfluence;
                if (_actor.InfluenceOfSchools[_proposer] < INFLUENCE_DECREASE_IF_LOST) {
                    changeOfInfluence = _actor.InfluenceOfSchools[_proposer];
                    _actor.InfluenceOfSchools[_proposer] = 0;
                } else {
                    changeOfInfluence = -INFLUENCE_DECREASE_IF_LOST;
                    _actor.InfluenceOfSchools[_proposer] -= INFLUENCE_DECREASE_IF_LOST;
                }
                _report = new Report(false, changeOfInfluence, report.AttackerLoss, report.DefenderLoss);
            }
        }

        public new Report GetReport() => _report;
    }

}
