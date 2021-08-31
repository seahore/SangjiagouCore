using UnityEngine;

namespace SangjiagouCore {

    /// <summary>
    /// 大狩行动
    /// </summary>
    public class HuntAction : StateAction, IReportable<HuntAction.Report>
    {
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            int _changeOfInfluence;
            public int ChangeOfInfluence => _changeOfInfluence;
            int _foodIncrease;
            public int FoodIncrease => _foodIncrease;
            int _militechIncrease;
            public int MilitechIncrease => _militechIncrease;
            public Report(bool successful, int changeOfInfluence, int foodIncrease, int militechIncrease)
            {
                _successful = successful;
                _changeOfInfluence = changeOfInfluence;
                _foodIncrease = foodIncrease;
                _militechIncrease = militechIncrease;
            }
        }
        Report _report;

        public HuntAction(State actor, School proposer):
            base(actor, proposer)
        { }
    
        public override float Assess()
        {
            return 1.0f;
        }

        public override void Act()
        {
            int foodIncrease = Random.Range(4000, 10000);
            _actor.Food += foodIncrease;

            ++_actor.Militech;

            _report = new Report(true, 0, foodIncrease, 1);
        }

        public Report GetReport() => _report;

        public override string Name => "大狩";

        public override string ToString() => $"<b>{_actor.Name}</b>君率军大狩";
    }

}
