using UnityEngine;


namespace SangjiagouCore
{

    /// <summary>
    /// 发动义战行动
    /// </summary>
    public class ImproveRelationshipsAction : StateAction, IReportable<ImproveRelationshipsAction.Report>
    {
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            int _changeOfInfluence;
            public int ChangeOfInfluence => _changeOfInfluence;
            int _relationshipIncrease;
            public int RelationshipIncrease => _relationshipIncrease;
            public Report(bool successful, int changeOfInfluence, int relationshipIncrease)
            {
                _successful = successful;
                _changeOfInfluence = changeOfInfluence;
                _relationshipIncrease = relationshipIncrease;
            }
        }
        Report _report;

        protected State _counterpart;
        public State Counterpart => _counterpart;

        const int INFLUENCE_INCREASE_IF_WON = 10;
        const int INFLUENCE_DECREASE_IF_LOST = 20;

        public ImproveRelationshipsAction(State actor, School proposer, State counterpart) :
            base(actor, proposer)
        {
            if(!(actor is null || counterpart is null) && actor == counterpart) {
                Debug.LogWarning("不能和自己改善关系");
            }
            _counterpart = counterpart;
        }

        public override float Assess()
        {
            return 1.0f;
        }

        public override void Act()
        {
            int relationshipIncrease = Random.Range(4, 10);
            _counterpart.AttitudeTowards[_actor] += relationshipIncrease;
            _report = new Report(false, 0, relationshipIncrease);
        }

        public Report GetReport() => _report;

        public override string Name => "交好";

        public override string ToString() => $"<b>{_actor.Name}</b>讨伐<b>{_counterpart.Name}</b>的义战";
    }

}