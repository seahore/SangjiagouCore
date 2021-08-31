using UnityEngine;

namespace SangjiagouCore
{

    /// <summary>
    /// 赈粮的行动
    /// </summary>
    public class RelievePoorAction : StateAction, IReportable<RelievePoorAction.Report>
    {
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            int _changeOfInfluence;
            public int ChangeOfInfluence => _changeOfInfluence;
            int _foodConsumption;
            public int FoodConsumption => _foodConsumption;
            int _satisfactionIncrease;
            public int SatisfactionIncrease => _satisfactionIncrease;
            public Report(bool successful, int changeOfInfluence,int foodConsumption,  int satisfactionIncrease)
            {
                _successful = successful;
                _changeOfInfluence = changeOfInfluence;
                _foodConsumption = foodConsumption;
                _satisfactionIncrease = satisfactionIncrease;
            }
        }
        Report _report;

        public RelievePoorAction(State actor, School proposer) :
            base(actor, proposer)
        { }

        public override float Assess()
        {
            return 1.0f;
        }

        public override void Act()
        {
            int consumption = _actor.Population * 3;
            int increase = Random.Range(3, 8);

            _actor.Satisfaction += increase;
            _actor.Food -= _actor.Population * 3;

            _report = new Report(true, 0, consumption, increase);
        }

        public Report GetReport() => _report;

        public override string Name => "赈粮";

        public override string ToString() => $"<b>{_actor.Name}</b>开仓赈粮";
    }

}
