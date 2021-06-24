using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore {

    /// <summary>
    /// 发动不义战行动
    /// </summary>
    public class DeclareAggressiveWarAction : DeclareWarAction
    {
        Town _target;

        const uint INFLUENCE_INCREASE_IF_WON = 20;
        const uint INFLUENCE_DECREASE_IF_LOST = 40;

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
            AggressiveWar.Report report = war.Settle();
            if(report.AttackerWon) {
                _target.CedeTo(_actor);
                _actor.InfluenceOfSchools[_proposer] += INFLUENCE_INCREASE_IF_WON;
            } else {
                if (_actor.InfluenceOfSchools[_proposer] < INFLUENCE_DECREASE_IF_LOST) {
                    _actor.InfluenceOfSchools[_proposer] = 0;
                } else {
                    _actor.InfluenceOfSchools[_proposer] -= INFLUENCE_DECREASE_IF_LOST;
                }
            }
        }
    }

}
