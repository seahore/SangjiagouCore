using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore
{

    public class DiscussWithMonarchAction : ScholarAction
    {


        public override void Act()
        {
            _place.Controller.InfluenceOfSchools[_actor.BelongTo] += 20;
        }


        public DiscussWithMonarchAction(Scholar actor, Town place)
        : base(actor, place)
        {

        }
    }

}
