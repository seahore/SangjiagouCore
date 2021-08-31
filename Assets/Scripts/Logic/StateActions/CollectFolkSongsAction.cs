using UnityEngine;

namespace SangjiagouCore {

    /// <summary>
    /// 采风行动
    /// </summary>
    public class CollectFolkSongsAction : StateAction, IReportable<CollectFolkSongsAction.Report>
    {
        public struct Report
        {
            bool _successful;
            public bool Successful => _successful;
            int _changeOfInfluence;
            public int ChangeOfInfluence => _changeOfInfluence;
            int _satisfactionIncrease;
            public int SatisfactionIncrease => _satisfactionIncrease;
            int _politechIncrease;
            public int PolitechIncrease => _politechIncrease;
            public Report(bool successful, int changeOfInfluence,int satisfactionIncrease, int politechIncrease)
            {
                _successful = successful;
                _changeOfInfluence = changeOfInfluence;
                _satisfactionIncrease = satisfactionIncrease;
                _politechIncrease = politechIncrease;
            }
        }
        Report _report;

        public CollectFolkSongsAction(State actor, School proposer):
            base(actor, proposer)
        { }
    
        public override float Assess()
        {
            return 1.0f;
        }

        public override void Act() {
            int satisIncrease = Random.Range(1, 4);
            _actor.Satisfaction += satisIncrease;

            ++_actor.Politech;

            _report = new Report(true, 0, satisIncrease, 1);
        }

        public Report GetReport() => _report;

        public override string Name => "采风";

        public override string ToString() => $"<b>{_actor.Name}</b>采集风颂于朝野";
    }

}
