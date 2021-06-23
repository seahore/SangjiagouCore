using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SangjiagouCore
{

    public class AggressiveWar : War
    {
        /// <summary>
        /// 不义战结算
        /// </summary>
        public new struct Report
        {
            bool _attackerWon;
            public bool AttackerWon => _attackerWon;
            uint _attackerLoss;
            public uint AttackerLoss => _attackerLoss;
            uint _defenderLoss;
            public uint DefenderLoss => _defenderLoss;

            public Report(bool attackerWon, uint attackerLoss, uint defenderLoss)
            {
                _attackerWon = attackerWon;
                _attackerLoss = attackerLoss;
                _defenderLoss = defenderLoss;
            }
        }
        Town _targetTown;
        /// <summary>
        /// 本次义战所争夺的城郭
        /// </summary>
        public Town TargetTown { get => _targetTown; }

        /// <summary>
        /// 创建一个新的不义战，并自动根据宣战双方自动询问他国是否参加战争，并自动将愿意加入战争某一方的国家加入队列
        /// </summary>
        /// <param name="declarer">宣战国</param>
        /// <param name="declaree">被宣战国</param>
        /// <param name="targetTown">所争夺的城郭</param>
        public AggressiveWar(State attacker, State defender, Town targetTown) :
            base(attacker, defender)
        {
            _targetTown = targetTown;
            foreach (var s in _attackers) {
                s.Monarch.IsInvader = true;
            }
        }

        /// <summary>
        /// 进行不义战的模拟，返回该次战争的结算报告
        /// </summary>
        /// <returns>该次战争的结算报告</returns>
        public new AggressiveWar.Report Settle()
        {
            uint attackerArmy = _initialAttackerArmy;
            uint defenderArmy = _initialDefenderArmy;
            bool toMuchLoss() => attackerArmy < (0.75f * _initialAttackerArmy) || defenderArmy < (0.75f * _initialDefenderArmy);
            while (true) {
                defenderArmy -= (uint)(0.02f * attackerArmy * Random.Range(0.5f, 1.5f) * _attackerEnhancement);
                attackerArmy -= (uint)(0.02f * defenderArmy * Random.Range(0.6f, 1.6f) * _defenderEnhancement);
                // 如果某一方损失太多，那么每一轮结束战役的机会提高到0.4
                if (toMuchLoss()) {
                    if (Random.Range(0.0f, 1.0f) < 0.4f) break;
                } else {
                    if (Random.Range(0.0f, 1.0f) < 0.1f) break;
                }
            }
            float attackerWonPossibility;
            if (attackerArmy > defenderArmy) {
                attackerWonPossibility = 1.0f - defenderArmy / (2.0f * attackerArmy);
            } else {
                attackerWonPossibility = attackerArmy / (2.0f * defenderArmy);
            }
            bool attackerWon = Random.Range(0.0f, 1.0f) < attackerWonPossibility;
            return new AggressiveWar.Report(attackerWon, _initialAttackerArmy - attackerArmy, _initialDefenderArmy - defenderArmy);
        }
    }

}