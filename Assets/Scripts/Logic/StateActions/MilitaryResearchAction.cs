using UnityEngine;

namespace SangjiagouCore
{

    /// <summary>
    /// 军事研究的行动
    /// </summary>
    public class MilitaryResearchAction : StateAction, IReportable<MilitaryResearchAction.Report>
    {
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            int _changeOfInfluence;
            public int ChangeOfInfluence => _changeOfInfluence;
            int _militechIncrease;
            public int MilitechIncrease => _militechIncrease;
            public Report(bool successful, int changeOfInfluence, int militechIncrease)
            {
                _successful = successful;
                _changeOfInfluence = changeOfInfluence;
                _militechIncrease = militechIncrease;
            }
        }
        Report _report;

        public MilitaryResearchAction(State actor, School proposer) :
            base(actor, proposer)
        { }

        public override float Assess()
        {
            return 1.0f;
        }

        public override void Act()
        {
            int increase = Random.Range(1, 2);

            _actor.Militech += increase;

            _report = new Report(true, 0, increase);
        }

        public Report GetReport() => _report;

        public override string Name => "军事研究";

        public override string ToString() => $"<b>{_actor.Name}</b>研究军事";
    }

}
