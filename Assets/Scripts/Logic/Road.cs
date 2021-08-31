namespace SangjiagouCore
{
    /// <summary>
    /// 描述道路的类，道路之间可以通过两个道路节点不论顺序地比较。
    /// </summary>
    public class Road
    {
        Town _town1, _town2;
        public Town Town1 => _town1;
        public Town Town2 => _town2;

        public Road(Town town1, Town town2)
        {
            _town1 = town1;
            _town2 = town2;
        }

        public override bool Equals(object obj) => (_town1 == (obj as Road)._town1 && _town2 == (obj as Road)._town2) || (_town1 == (obj as Road)._town2 && _town2 == (obj as Road)._town1);
        public override int GetHashCode() => _town1.GetHashCode() ^ _town2.GetHashCode();
        public static bool operator ==(Road a, Road b) => a.Equals(b);
        public static bool operator !=(Road a, Road b) => !(a == b);
    }
}
