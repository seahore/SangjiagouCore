using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore {

    public class DeclareWarAction : StateAction
    {
        protected State _declaree;

        const uint INFLUENCE_INCREASE_IF_WON = 10;
        const uint INFLUENCE_DECREASE_IF_LOST = 20;

        public DeclareWarAction(State actor, School proposer, State declaree):
            base(actor, proposer)
        {
            _declaree = declaree;
        }
    
        public override float Assess() {
            return 0.0f;
        }

        public override void Act() {
            if(!_declaree.Monarch.IsInvader)
                Debug.LogWarning("向没有发动过不义战的君主发动了义战");

            War war = new War(_actor, _declaree);
            War.Report report = war.Settle();

            if(report.AttackerWon) {
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
