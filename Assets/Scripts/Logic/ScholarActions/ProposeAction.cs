using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore
{

    /// <summary>
    /// 对策行动，向某国的行动候选队列加入一个国家行动
    /// </summary>
    public class ProposeAction : ScholarAction, IReportable<ProposeAction.Report>
    {
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;

            public Report(bool successful)
            {
                _successful = successful;
            }
        }
        Report _report;


        StateAction _proposition;
        public StateAction Proposition => _proposition;

        public override void Act()
        {
            float possibility = 0.5f + StateAt.InfluenceOfSchools[ActSchool] * 0.01f;

            if (Random.Range(0.0f, 1.0f) < possibility) {
                _report = new Report(true);
                StateAt.ActionQueue.Add(_proposition);
            } else {
                _report = new Report(false);
            }
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

        public Report GetReport() => _report;
    }

}
