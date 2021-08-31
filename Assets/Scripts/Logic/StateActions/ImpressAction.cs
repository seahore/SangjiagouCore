using UnityEngine;

namespace SangjiagouCore {

    /// <summary>
    /// 徭役行动
    /// </summary>
    public class ImpressAction : StateAction, IReportable<ImpressAction.Report>
    {
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            int _changeOfInfluence;
            public int ChangeOfInfluence => _changeOfInfluence;
            int _foodConsumption;
            public int FoodConsumption => _foodConsumption;
            int _satisfactionReduction;
            public int SatisfactionReduction => _satisfactionReduction;
            int _armyIncrease;
            public int ArmyIncrease => _armyIncrease;
            public Report(bool successful, int changeOfInfluence, int foodConsumption, int satisfactionReduction, int armyIncrease)
            {
                _successful = successful;
                _changeOfInfluence = changeOfInfluence;
                _foodConsumption = foodConsumption;
                _satisfactionReduction = satisfactionReduction;
                _armyIncrease = armyIncrease;
            }
        }
        Report _report;

        public ImpressAction(State actor, School proposer):
            base(actor, proposer)
        { }
    
        public override float Assess()
        {
            return 1.0f;
        }

        public override void Act() {
            int oldArmy = _actor.Army;
            _actor.Army += (int)(Random.Range(0.02f, 0.04f) * _actor.Population);
            int increase = _actor.Army - oldArmy;
            int consumption = (int)(increase * Random.Range(8.0f, 12.0f));
            _actor.Food -= consumption;
            _actor.Satisfaction -= 5;

            _report = new Report(true, 0, consumption, 5, increase);
        }

        public Report GetReport() => _report;

        public override string Name => "徭役";

        public override string ToString() => $"<b>{_actor.Name}</b>抓壮丁充军";
    }

}
