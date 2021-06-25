using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore
{

    /// <summary>
    /// 访贤行动，在一城内寻找新的成员加入学家
    /// </summary>
    public class RecruitAction : ScholarAction, IReportable<RecruitAction.Report>
    {
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            Scholar _scholarRecruited;
            public Scholar ScholarRecruited => _scholarRecruited;
            public Report(bool successful, Scholar scholarRecruited)
            {
                _successful = successful;
                _scholarRecruited = scholarRecruited;
            }
        }
        Report _report;

        Town _target;
        public Town Target => _target;

        public override void Act()
        {
            float possibility = 0.1f + _actor.Sophistry * 0.005f;

            if(Random.Range(0.0f, 1.0f) < possibility) {
                Scholar scholar = null;
                if (_target.Recruits[ActSchool].Count != 0) {
                    int i = Random.Range(0, _target.Recruits[ActSchool].Count);
                    scholar = _target.Recruits[ActSchool][i];
                    _target.Recruits[ActSchool].RemoveAt(i);
                } else {
                    scholar = Scholar.CreateRandomScholar();
                }
                scholar = new Scholar(scholar.Pack(), ActSchool);
                ActSchool.Members.Add(scholar);
                _report = new Report(true, scholar);
            } else {
                _report = new Report(false, null);
            }
        }

        public Report GetReport() => _report;

        public RecruitAction(Scholar actor, Town place, Town target)
        : base(actor, place)
        {
            _target = target;
        }
    }

}
