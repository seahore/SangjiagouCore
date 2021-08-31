using UnityEngine;

namespace SangjiagouCore {

    /// <summary>
    /// 强征行动
    /// </summary>
    public class CommandeerAction : StateAction, IReportable<CommandeerAction.Report>
    {
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            int _changeOfInfluence;
            public int ChangeOfInfluence => _changeOfInfluence;
            int _foodIncrease;
            public int FoodIncrease => _foodIncrease;
            int _satisfactionReduction;
            public int SatisfactionReduction => _satisfactionReduction;
            public Report(bool successful, int changeOfInfluence, int fooodIncrease, int satisfactionReduction)
            {
                _successful = successful;
                _changeOfInfluence = changeOfInfluence;
                _foodIncrease = fooodIncrease;
                _satisfactionReduction = satisfactionReduction;
            }
        }
        Report _report;

        public CommandeerAction(State actor, School proposer):
            base(actor, proposer)
        { }
    
        public override float Assess()
        {
            return 1.0f;
        }

        public override void Act() {
            int increase =  (int)(_actor.Population * Random.Range(1.0f, 2.0f));
            _actor.Food += increase;
            _actor.Satisfaction -= 10;

            _report = new Report(true, 0, increase, 10);
        }

        public Report GetReport() => _report;

        public override string Name => "强征";

        public override string ToString() => $"<b>{_actor.Name}</b>强征租赋";
    }

}
