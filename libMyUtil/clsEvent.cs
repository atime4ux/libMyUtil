using System;
using System.Collections.Generic;
using System.Text;

namespace libMyUtil
{
    public class clsEvent
    {
        public static bool isTestPhone(string CTN)
        {
            if (CTN.Equals("01054949033"))
                return true;//SKT
            if (CTN.Equals("01072079033"))
                return true;//KTF
            if (CTN.Equals("01022389033"))
                return true;//LGT

            return false;
        }
    }
}
