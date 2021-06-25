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
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            int _changeOfInfluence;
            public int ChangeOfInfluence => _changeOfInfluence;

            public Report(bool successful, int changeOfInfluence)
            {
                _successful = successful;
                _changeOfInfluence = changeOfInfluence;
            }
        }
        Report _report;

        const int INFLUENCE_INCREASE_WHEN_SUCCEEDS = 20;
        const int INFLUENCE_DECREASE_WHEN_FAILS = 10;

        public override void Act()
        {
            if (Random.Range(0.0f, 1.0f) < 0.8f + _actor.Sophistry * 0.01f) {
                StateAt.InfluenceOfSchools[ActSchool] += INFLUENCE_INCREASE_WHEN_SUCCEEDS;
                _report = new Report(true, INFLUENCE_INCREASE_WHEN_SUCCEEDS);
            } else {
                StateAt.InfluenceOfSchools[ActSchool] -= INFLUENCE_DECREASE_WHEN_FAILS;
                _report = new Report(true, -INFLUENCE_DECREASE_WHEN_FAILS);
            }
        }

        public Report GetReport() => _report;

        public DiscussWithMonarchAction(Scholar actor, Town place)
        : base(actor, place)
        { }
    }

}
