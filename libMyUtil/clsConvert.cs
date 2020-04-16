using System;
using System.Collections.Generic;
using System.Text;

namespace libMyUtil
{
    public class clsConvert
    {
        /// <summary>
        /// <, >, &를 &lt;, &gt;, &amp;로 변환
        /// </summary>
        public static string HTMLtagToHTMLentity(string str)
        {
            return str.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
        }
    }
}
