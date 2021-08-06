using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore
{

    /// <summary>
    /// 游历行动，转移到另一个城郭
    /// </summary>
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

        public override string ToString() => "准备前往<b>" + _destination.Controller.Name + "</b>的<b>" + _destination.Name + "</b>。";
    }

}
