using System;
using System.Collections.Generic;
using System.Text;

namespace libMyUtil
{
    public static class clsDate
    {
        /// <summary>
        /// yyyyMMddHHmmss, yyyyMMdd를 Datetime형식으로 변환
        /// </summary>
        public static DateTime StringToDatetime(string DateTimeString)
        {
            DateTime RtnDt = new DateTime(0);
            libCommon.clsUtil objUtil = new libCommon.clsUtil();

            try
            {
                if (DateTimeString.Length == 8)
                {
                    RtnDt = new DateTime(
                        objUtil.ToInt32(DateTimeString.Substring(0, 4)),
                        objUtil.ToInt32(DateTimeString.Substring(4, 2)),
                        objUtil.ToInt32(DateTimeString.Substring(6))
                        );
                }
                else if (DateTimeString.Length == 14)
                {
                    RtnDt = new DateTime(
                        objUtil.ToInt32(DateTimeString.Substring(0, 4)),
                        objUtil.ToInt32(DateTimeString.Substring(4, 2)),
                        objUtil.ToInt32(DateTimeString.Substring(6, 2)),
                        objUtil.ToInt32(DateTimeString.Substring(8, 2)),
                        objUtil.ToInt32(DateTimeString.Substring(10, 2)),
                        objUtil.ToInt32(DateTimeString.Substring(12))
                        );
                }
            }
            catch (Exception ex)
            {
                objUtil.writeLog(ex.Message);
            }

            return RtnDt;
        }
    }
}
