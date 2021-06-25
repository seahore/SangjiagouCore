using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore {

    public interface IAction
    {
        /// <summary>
        /// 执行该行动，返回一个结果报告
        /// </summary>
        public void Act();
    }

}
