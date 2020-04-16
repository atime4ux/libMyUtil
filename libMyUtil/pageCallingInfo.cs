using System;
using System.Collections.Generic;
using System.Text;

namespace libMyUtil
{
    /// <summary>
    /// 웹페이지 호출 정보 클래스
    /// </summary>
    public class pageCallingInfo
    {
        public string FKEY;
        public string aURLset_Idx;

        public string url;
        public string postData;
        public string callresult;
        public string httpMethod;

        /// <summary>
        /// 밀리세컨드(ms)
        /// </summary>
        public int timeOut;
        public int failCnt;

        public System.DateTime lastTry;

        /// <summary>
        /// url, postData는 "", result는 RESULT:00, 타임아웃은 5000
        /// </summary>
        public pageCallingInfo()
        {
            this.FKEY = "";
            this.aURLset_Idx = "";

            this.url = "";
            this.postData = "";
            this.callresult = "RESULT:00";
            
            this.timeOut = 5;
            this.failCnt = 0;
        }
        /// <summary>
        /// 웹페이지 호출 정보 설정
        /// </summary>
        /// <param name="Result">호출 성공 시 받을 결과</param>
        /// <param name="TimeOut">타임 아웃 시간(초)</param>
        /// <param name="FailCnt">재시도 횟수</param>
        public void setInfo(string fkey, string aURLset_idx, string URL, string PostData, string Result, string HttpMethod, int TimeOut)
        {
            this.FKEY = fkey;
            this.aURLset_Idx = aURLset_idx;

            this.url = URL;
            this.postData = PostData;
            this.callresult = Result;
            this.httpMethod = HttpMethod;

            this.timeOut = TimeOut;
        }
        /// <summary>
        /// 로그 기록
        /// </summary>
        /// <param name="Result">에러 결과</param>
        public void writeLog(string Result)
        {
            libCommon.clsUtil objUtil = new libCommon.clsUtil();

            string logStr = string.Format("FINALLY FAILED CALLING URL : {0}\r\nFKEY:{1}\r\nAUTO URL IDX:{2}\r\nPOSTDATA:{3}\r\nFAIL COUNT:{4}\r\nLAST TRY:{5}\r\nTIME OUT:{6}\r\nRESULT:{7}\r\nRECEIVED RESULT:{8}"
                , this.url
                , this.FKEY
                , this.aURLset_Idx
                , this.postData
                , this.failCnt
                , this.lastTry
                , this.timeOut
                , this.callresult
                , Result);
            
            objUtil.writeLog(logStr);
        }
    }
}