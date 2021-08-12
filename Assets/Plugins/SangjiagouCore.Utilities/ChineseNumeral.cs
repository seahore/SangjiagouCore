using System.Collections.Generic;

namespace SangjiagouCore.Utilities
{
    public static class ChineseNumeral
    {
        static char[] num = { '\0', '一', '二', '三', '四', '五', '六', '七', '八', '九' };
        static Dictionary<int, char> lSep = new Dictionary<int, char> { { 1000, '千' }, { 100, '百' }, { 10, '十' } };
        static char[] sep = { '\0', '万', '亿', '兆', '京', '垓' };



        static string TenThousand2Chinese(int src)
        {
            int t = src;
            string s = "";
            int i;
            for (i = 1000; t / i == 0; t %= i, i /= 10) ;
            for (; t > 0; t %= i, i /= 10) {
                if (t / i == 0) {
                    for (; t / i == 0; t %= i, i /= 10) ;
                    if (i >= 1) {
                        s += "零";
                    }
                }
                s += num[t / i];
                if (i >= 10)
                    s += lSep[i];
            }
            return s;
        }


        /// <summary>
        /// 将int类型数值转换成汉语数字字符串。
        /// </summary>
        /// <param name="src">数值</param>
        /// <param name="useLiang">是否在习惯使用“两”的场合使用“两”</param>
        /// <param name="traditional">是否输出繁体</param>
        /// <returns>相应的汉语数字字符串</returns>
        public static string Int2Chinese(int src, bool useLiang = false, bool traditional = false)
        {
            if (src == 0)
                return "零";
            if (src < 10000)
                return TenThousand2Chinese(src);

            List<int> l = new List<int>();
            while (src > 0) {
                l.Add(src % 10000);
                src /= 10000;
            }
            string result = "";
            for (int i = 0; i < l.Count - 1; ++i) {
                if (l[i] == 0) {
                    if (i > 0 && l[i - 1] / 1000 != 0) {
                        result = "零" + result;
                    }
                    continue;
                }
                if (i > 0)
                    result = TenThousand2Chinese(l[i]) + sep[i] + result;
                else
                    result = TenThousand2Chinese(l[i]);

                if (l[i] < 1000) result = "零" + result;
            }
            result = TenThousand2Chinese(l[l.Count - 1]) + sep[l.Count - 1] + result;
            return result;
        }
    }
}
