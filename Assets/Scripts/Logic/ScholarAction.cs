using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore {

    public abstract class ScholarAction : IAction
    {
        protected Scholar _actor;
        public Scholar Actor => _actor;
        public School ActSchool => _actor.BelongTo;
        protected Town _place;
        public Town Place => _place;
        public State StateAt => _place.Controller;

        public abstract void Act();

        public ScholarAction(Scholar actor, Town place) {
            _actor = actor;
            _place = place;
        }
    }

}
