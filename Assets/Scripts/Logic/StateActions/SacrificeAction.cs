using UnityEngine;

namespace SangjiagouCore {

    /// <summary>
    /// 祭祀行动
    /// </summary>
    public class SacrificeAction : StateAction, IReportable<SacrificeAction.Report>
    {
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            int _changeOfInfluence;
            public int ChangeOfInfluence => _changeOfInfluence;
            int _foodConsumption;
            public int FoodConsumption => _foodConsumption;
            int _ceremonyIncrease;
            public int CeremonyIncrease => _ceremonyIncrease;
            public Report(bool successful, int changeOfInfluence, int foodConsumption, int ceremonyIncrease)
            {
                _successful = successful;
                _changeOfInfluence = changeOfInfluence;
                _foodConsumption = foodConsumption;
                _ceremonyIncrease = ceremonyIncrease;
            }
        }
        Report _report;

        public SacrificeAction(State actor, School proposer):
            base(actor, proposer)
        { }
    
        public override float Assess()
        {
            return 1.0f;
        }

        public override void Act() {
            int consumption = Random.Range(2000, 10000);
            int increase = Random.Range(1, 4);

            _actor.Food -= consumption;
            _actor.Ceremony += increase;

            _report = new Report(true, 0, consumption, increase);
        }

        public Report GetReport() => _report;

        public override string Name => "祭祀";

        public override string ToString() => $"<b>{_actor.Name}</b>君封禅，以致太平";
    }

}
