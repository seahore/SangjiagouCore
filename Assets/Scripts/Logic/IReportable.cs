using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SangjiagouCore
{
    public interface IReportable<ReportT>
    {
        /// <summary>
        /// 获取报告
        /// </summary>
        /// <returns>一份报告。如未生成应当返回null</returns>
        public ReportT GetReport();
    }
}
