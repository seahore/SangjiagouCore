using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore
{

    public class TravelAction : ScholarAction
    {
        Town _destination;

        public override void Act()
        {
            _actor.Location = _destination;
        }


        public TravelAction(Scholar actor, Town place, Town destination)
        : base(actor, place)
        {
            _destination = destination;
        }
    }

}
