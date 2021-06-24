using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore
{

    public class ProposeAction : ScholarAction
    {
        StateAction _proposition;

        public override void Act()
        {
            _place.Controller.ActionQueue.Add(_proposition);
        }



        public ProposeAction(Scholar actor, Town place, StateAction proposition)
        : base(actor, place)
        {
            if (!actor.BelongTo.AllowedPropositionTypes.Contains(_proposition.GetType()))
                Debug.LogWarning($"{actor.BelongTo.Name}的{actor.FullName}提出了不符合其学家立场的对策: {proposition.GetType().Name}");
            if (!place.IsCapital)
                Debug.LogWarning($"{actor.BelongTo.Name}的{actor.FullName}在{place.Name}提出了对策，但{place.Name}不是京城");

            _proposition = proposition;
        }
    }

}
