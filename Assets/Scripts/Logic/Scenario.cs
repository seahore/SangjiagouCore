using UnityEngine;

namespace SangjiagouCore
{
    /// <summary>
    /// 开场的描述结构
    /// </summary>
    public struct Scenario
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Introduction { get; set; }
        public string GameFilePath { get; set; }
    }
}
