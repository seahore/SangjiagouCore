using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore {

    public abstract class StateAction : IAction
    {
        protected State _actor;
        protected School _proposer;

        public abstract void Act();

        /// <summary>
        /// 由电脑评估此行动的可行性
        /// </summary>
        /// <returns>可行性</returns>
        public abstract float Assess();
    
        public StateAction(State actor, School proposer) {
            _actor = actor;
            _proposer = proposer;
        }
    }

}
