using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore
{
    /// <summary>
    /// 说理行动，提升学家影响力
    /// </summary>
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
