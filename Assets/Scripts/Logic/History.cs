using System.Collections.Generic;

namespace SangjiagouCore
{
    /// <summary>
    /// 历史的描述结构，包含数个在不同时间上的开场
    /// </summary>
    public struct History
    {
        public string Name { get; set; }
        public List<Scenario> Scenarios { get; set; }
    }
}
