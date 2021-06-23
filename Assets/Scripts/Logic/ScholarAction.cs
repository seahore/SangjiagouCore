using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore {

    public abstract class ScholarAction : IAction
    {
        protected Scholar _actor;
        protected Town _place;

        public abstract void Act();
    
        public ScholarAction(Scholar actor, Town place) {
            _actor = actor;
            _place = place;
        }
    }

}
