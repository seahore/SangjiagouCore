using UnityEngine;

namespace SangjiagouCore
{

    /// <summary>
    /// 政治研究的行动
    /// </summary>
    public class PoliticsResearchAction : StateAction, IReportable<PoliticsResearchAction.Report>
    {
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            int _changeOfInfluence;
            public int ChangeOfInfluence => _changeOfInfluence;
            int _politechIncrease;
            public int PolitechIncrease => _politechIncrease;
            public Report(bool successful, int changeOfInfluence, int politechIncrease)
            {
                _successful = successful;
                _changeOfInfluence = changeOfInfluence;
                _politechIncrease = politechIncrease;
            }
        }
        Report _report;

        public PoliticsResearchAction(State actor, School proposer) :
            base(actor, proposer)
        { }

        public override float Assess()
        {
            return 1.0f;
        }

        public override void Act()
        {
            int increase = Random.Range(1, 2);

            _actor.Politech += increase;

            _report = new Report(true, 0, increase);
        }

        public Report GetReport() => _report;

        public override string Name => "政治研究";

        public override string ToString() => $"<b>{_actor.Name}</b>研究政治";
    }

}
