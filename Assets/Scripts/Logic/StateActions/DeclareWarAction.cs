using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore
{

    /// <summary>
    /// 发动义战行动
    /// </summary>
    public class DeclareWarAction : StateAction, IReportable<DeclareWarAction.Report>
    {
        public struct Report
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

        protected State _declaree;
        public State Declaree => _declaree;

        const int INFLUENCE_INCREASE_IF_WON = 10;
        const int INFLUENCE_DECREASE_IF_LOST = 20;

        public DeclareWarAction(State actor, School proposer, State declaree) :
            base(actor, proposer)
        {
            _declaree = declaree;
        }

        public override float Assess()
        {
            if (!_declaree.Monarch.IsInvader)
                return CANNOT_ACT;

            return 0.0f;
        }

        public override void Act()
        {
            War war = new War(_actor, _declaree);
            war.Settle();
            War.Report report = war.GetReport();

            if (report.AttackerWon) {
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

        public Report GetReport() => _report;
    }

}