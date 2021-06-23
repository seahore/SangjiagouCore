using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SangjiagouCore
{
    /// <summary>
    /// 描述两国及各自盟友之间的<strong>义战</strong>，及战争结果的演算
    /// </summary>
    public class War
    {
        /// <summary>
        /// 义战结算
        /// </summary>
        public struct Report
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

        protected List<State> _attackers;
        /// <summary>
        /// 本次义战的攻击国，首元素是宣战国
        /// </summary>
        public List<State> Attackers { get => _attackers; }
        /// <summary>
        /// 本次义战的宣战国
        /// </summary>
        public State Declarer { get => _attackers[0]; }

        protected uint _initialAttackerArmy;
        /// <summary>
        /// 攻击方的初始兵力，宣战者出全部兵力，其他参战者出其20%兵力
        /// </summary>
        public uint InitialAttackerArmy { get => _initialAttackerArmy; }
        protected float _attackerEnhancement;
        /// <summary>
        /// 攻击方的加成
        /// </summary>
        public float AttackerEnhancement { get => _attackerEnhancement; }

        protected List<State> _defenders;
        /// <summary>
        /// 本次义战的防御国，首元素是被宣战国
        /// </summary>
        public List<State> Defenders { get => _defenders; }
        /// <summary>
        /// 本次义战的防御国
        /// </summary>
        public State Declaree { get => _defenders[0]; }
        protected uint _initialDefenderArmy;
        /// <summary>
        /// 攻击方的初始兵力，被宣战者出全部兵力，其他参战者出其二成兵力
        /// </summary>
        public uint InitialDefenderArmy { get => _initialDefenderArmy; }
        protected float _defenderEnhancement;
        /// <summary>
        /// 防御方的加成
        /// </summary>
        public float DefenderEnhancement { get => _defenderEnhancement; }

        /// <summary>
        /// 创建一个新的义战，并自动根据宣战双方自动询问他国是否参加战争，并自动将愿意加入战争某一方的国家加入队列
        /// </summary>
        /// <param name="declarer">宣战国</param>
        /// <param name="declaree">被宣战国</param>
        public War(State declarer, State declaree)
        {
            _attackers = new List<State>();
            _defenders = new List<State>();
            _attackers.Add(declarer);
            _defenders.Add(declaree);

            foreach (var s in Game.CurrentEntities.States) {
                if (s.WouldEnterWar(this, true)) _attackers.Add(s);
                else if (s.WouldEnterWar(this, false)) _defenders.Add(s);
            }

            _initialAttackerArmy = 0;
            foreach (var s in _attackers) {
                _initialAttackerArmy += (uint)(0.2f * s.Army);
            }
            _initialAttackerArmy += (uint)(0.8f * Declarer.Army);

            _initialDefenderArmy = 0;
            foreach (var s in _defenders) {
                _initialDefenderArmy += (uint)(0.2f * s.Army);
            }
            _initialDefenderArmy += (uint)(0.8f * Declaree.Army);

            _attackerEnhancement = 1.0f;
            _defenderEnhancement = 1.0f;
        }

        /// <summary>
        /// 进行义战的模拟，返回该次战争的结算报告
        /// </summary>
        /// <returns>该次战争的结算报告</returns>
        public War.Report Settle()
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
            float adRatio = (float)attackerArmy / (float)defenderArmy;
            if (attackerArmy > defenderArmy) {
                attackerWonPossibility = 1.0f - 1.0f / (2.0f * Mathf.Sqrt(adRatio));
            } else {
                attackerWonPossibility = Mathf.Sqrt(adRatio) / 2.0f;
            }
            bool attackerWon = Random.Range(0.0f, 1.0f) < attackerWonPossibility;
            return new War.Report(attackerWon, _initialAttackerArmy - attackerArmy, _initialDefenderArmy - defenderArmy);
        }

    }

}
