using System;
using System.IO;
using System.Net;
using System.Net.Json;
using System.Text;


namespace libMyUtil
{
  /// <summary>
  /// URLshorten메소드는 JSON for .NET 필요
  /// </summary>
  public class clsWeb
    {
        /// <summary>
        /// URL전송용 텍스트 인코딩
        /// </summary>
        /// <param name="EncType">EUC-KR, UTF-8</param>
        public static string encURL(string text, string EncType)
        {
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding(EncType.ToUpper());
            byte[] bytesData = enc.GetBytes(text);
            return System.Web.HttpUtility.UrlEncode(bytesData, 0, bytesData.Length);
        }

        /// <summary>
        /// 특수기호를 변환(&lt;, &gt;, &quot;)
        /// </summary>
        public static string toHTML(string text)
        {
            return text.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }
        
        /// <summary>
        /// 웹페이지 일부 화면 캡쳐, 저장된 파일 경로 반환
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="filename">경로를 포함한 파일 이름</param>
        /// <param name="x">좌상단 X좌표</param>
        /// <param name="y">좌상단 Y좌표</param>
        public static string WebpageCapture(string URL, string filename, int x, int y, int width, int height, System.Drawing.Imaging.ImageFormat objImgFormat)
        {
            clsWebpageCapture objCapture = new clsWebpageCapture(URL, filename, x, y, width, height, objImgFormat);
            return objCapture.WebpageCapture();
        }
        
        /// <summary>
        /// 웹페이지 전체화면 캡쳐, 저장된 파일 경로 반환
        /// </summary>
        /// <param name="filename">경로를 포함한 파일 이름</param>
        public static string WebpageCapture(string URL, string filename, System.Drawing.Imaging.ImageFormat objImgFormat)
        {
            clsWebpageCapture objCapture = new clsWebpageCapture(URL, filename, objImgFormat);
            return objCapture.WebpageCapture();
        }
        
        /// <summary>
        /// 웹페이지 일부 화면 캡쳐
        /// </summary>
        /// <param name="filename">경로를 포함한 파일 이름</param>
        public static void WebpageCapture_noReturn(string URL, string filename, int x, int y, int width, int height, System.Drawing.Imaging.ImageFormat objImgFormat)
        {
            clsWebpageCapture objCapture = new clsWebpageCapture(URL, filename, x, y, width, height, objImgFormat);
            objCapture.WebpageCapture_noReturn();
        }
        
        /// <summary>
        /// 웹페이지 전체화면 캡쳐
        /// </summary>
        /// <param name="filename">경로를 포함한 파일 이름</param>
        public static void WebpageCapture_noReturn(string URL, string filename, System.Drawing.Imaging.ImageFormat objImgFormat)
        {
            clsWebpageCapture objCapture = new clsWebpageCapture(URL, filename, objImgFormat);
            objCapture.WebpageCapture_noReturn();
        }

        /// <summary>
        /// Google URL 줄임 서비스, 
        /// </summary>
        public static string URLshorten(string longurl)
        {
            WebRequest webreq;
            
            JsonTextParser objParser = new JsonTextParser();//json 파서
            JsonObjectCollection SentJson = new JsonObjectCollection();//보낼 json객체 생성
            JsonObjectCollection objRcvJson = new JsonObjectCollection();//받을 json객체 생성
            string jsonData;
            string rtnJson;
            byte[] bytebuffer;
            
            SentJson.Add(new JsonStringValue("longUrl", longurl));//json항목 생성
            jsonData = SentJson.ToString();//json객체를 string으로 변환
            bytebuffer = Encoding.UTF8.GetBytes(jsonData);//string json객체를 바이트코드로 변환

            webreq = WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url");//google URL shorten API
            webreq.Method = WebRequestMethods.Http.Post;
            webreq.ContentType = "application/json";
            webreq.ContentLength = bytebuffer.Length;

            //리퀘스트
            using (Stream RequestStream = webreq.GetRequestStream())
            {
                RequestStream.Write(bytebuffer, 0, bytebuffer.Length);
                RequestStream.Close();
            }

            //리스폰스
            using (HttpWebResponse webresp = (HttpWebResponse)webreq.GetResponse())
            {
                using (Stream ResponseStream = webresp.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(ResponseStream))
                    {
                        rtnJson = sr.ReadToEnd();
                    }
                }
            }

            objRcvJson = (JsonObjectCollection)objParser.Parse(rtnJson);

            return objRcvJson["id"].GetValue().ToString();
        }

        /// <summary>
        /// DataSet(첫번째 DataTable)을 JSON 배열로 리턴 cmnfunc.js의 setOpenerfield_2 참조
        /// </summary>
        public static string DS2JsonString(System.Data.DataSet DS)
        {
            /*형식
            json_obj = 
            {   //JsonObjectCollection
                "list":
                    [   //JsonArrayCollection
                        {   //JsonObjectCollection
                           "elem":
                            [   //JsonObjectCollection
                                { "e_id": "comp_id", "e_value": "000001" }, //JsonObjectCollection
                                { "e_id": "comp_name", "e_value": "호미_1" },
                                { "e_id": "part_id", "e_value": "00000a" },
                                { "e_id": "part_name", "e_value": "개발_1" },
                                { "e_id": "user_id", "e_value": "abcd" }
                                { "e_id": "user_name", "e_value": "현석_1" }
                            ]
                        },
                        {
                           "elem":
                            [
                                { "e_id": "comp_id", "e_value": "000002" },
                                { "e_id": "comp_name", "e_value": "호미_2" },
                                { "e_id": "part_id", "e_value": "00000b" },
                                { "e_id": "part_name", "e_value": "개발_2" },
                                { "e_id": "user_id", "e_value": "qwer" }
                                { "e_id": "user_name", "e_value": "현석_2" }
                            ]
                        }
                    ]
            }
            */
            string Col = string.Empty;
            string Val = string.Empty;

            int colCnt;
            int rowCnt;
            
            JsonObjectCollection ReturnJson = new JsonObjectCollection();
            JsonArrayCollection jsonList = new JsonArrayCollection();
            
            if (clsCmnDB.validateDS(DS))
            {
                
                colCnt = DS.Tables[0].Columns.Count;
                rowCnt = DS.Tables[0].Rows.Count;

                for (int i = 0; i < rowCnt; i++)
                {
                    JsonObjectCollection jsonElem = new JsonObjectCollection();
                    JsonArrayCollection jsonElem_Array = new JsonArrayCollection();
                    
                    for (int j = 0; j < colCnt; j++)
                    {
                        JsonObjectCollection IDnValue = new JsonObjectCollection();

                        Col = DS.Tables[0].Columns[j].ColumnName;
                        Val = DS.Tables[0].Rows[i][j].ToString();

                        IDnValue.Add(new JsonStringValue("e_id", Col));
                        IDnValue.Add(new JsonStringValue("e_value", Val));

                        jsonElem_Array.Add(IDnValue);
                    }

                    jsonElem.Add(new JsonArrayCollection("elem", jsonElem_Array));
                    jsonList.Add(jsonElem);
                }
            }

            ReturnJson.Add(new JsonArrayCollection("list", jsonList));

            return ReturnJson.ToString();
        }

        /// <summary>
        /// DS2JsonString으로 변환된 json문자열을 Dataset으로 변환
        /// </summary>
        public static System.Data.DataSet JsonString2DS(string json_str)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Net.Json.JsonTextParser objParser = new JsonTextParser();

            JsonObjectCollection dataset = null;
            bool parsingSuccess;

            try
            {
                dataset = (JsonObjectCollection)objParser.Parse(json_str);//데이터셋
                parsingSuccess = true;
            }
            catch(Exception ex)
            { 
                libMyUtil.clsFile.writeLog("ERR PARSING JSON : " + ex.ToString());
                parsingSuccess = false;
            }

            if (parsingSuccess)
            {
                JsonArrayCollection datatable;
                JsonObjectCollection datarow;
                JsonArrayCollection datacolumn;
                JsonObjectCollection cell;

                string columnName;
                string columnValue;
                int tableIdx;
                int rowIdx;
                int columnIdx;

                for (tableIdx = 0; tableIdx < dataset.Count; tableIdx++)
                {
                    datatable = (JsonArrayCollection)dataset[tableIdx];//데이터테이블
                    DS.Tables.Add();
                    for (rowIdx = 0; rowIdx < datatable.Count; rowIdx++)
                    {
                        datarow = (JsonObjectCollection)datatable[rowIdx];//데이터로우
                        datacolumn = (JsonArrayCollection)datarow[0];//데이터컬럼 구조

                        DS.Tables[tableIdx].Rows.Add(DS.Tables[tableIdx].NewRow());
                        for (columnIdx = 0; columnIdx < datacolumn.Count; columnIdx++)
                        {
                            cell = (JsonObjectCollection)datacolumn[columnIdx];//셀
                            
                            if (rowIdx == 0)
                            {
                                columnName = cell["e_id"].GetValue().ToString();
                                DS.Tables[tableIdx].Columns.Add();
                                DS.Tables[tableIdx].Columns[columnIdx].ColumnName = columnName;
                            }

                            columnValue = cell["e_value"].GetValue().ToString();
                            DS.Tables[tableIdx].Rows[rowIdx][columnIdx] = columnValue;
                        }
                    }
                }
            }

            return DS;
        }

        /// <summary>
        /// POST방식으로 데이터 전송
        /// </summary>
        /// <param name="timeOut">밀리세컨드</param>
        /// <param name="EncodingType">ex) EUC-KR, UTF-8</param>
        /// <returns>URL의 응답</returns>
        public static string SendPostData(string SendData, string URL, string EncodingType, int timeOut)
        {
            System.Diagnostics.Stopwatch objStopWatch = new System.Diagnostics.Stopwatch();
            libCommon.clsUtil objUtil = new libCommon.clsUtil();

            HttpWebRequest httpWebRequest;
            HttpWebResponse httpWebResponse;
            Stream requestStream;
            StreamReader streamReader;
            byte[] Data;
            string Result;
            string encType = EncodingType.ToLower();

            if (!encType.Equals("euc-kr"))
                encType = "utf-8";//euc-kr이 아닌 기타 입력은 utf-8로 처리

            Data = System.Text.Encoding.GetEncoding(encType).GetBytes(SendData);

            try
            {
                objStopWatch.Start();
                httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=" + EncodingType;
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = Data.Length;
                
                //타임아웃 설정
                httpWebRequest.Timeout = timeOut;
                httpWebRequest.ReadWriteTimeout = timeOut;

                requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(Data, 0, Data.Length);
                requestStream.Close();

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                streamReader = new StreamReader(httpWebResponse.GetResponseStream());

                Result = streamReader.ReadToEnd();
                streamReader.Close();
                httpWebResponse.Close();
            }
            catch (Exception ex)
            {
                objStopWatch.Stop();
                objUtil.writeLog("FAIL SEND POST TRANSFER : " + ex.ToString() + "\r\nELAPSED TIME:" + objStopWatch.Elapsed.ToString());
                Result = "FAIL";
            }
            
            if (objStopWatch.IsRunning)
                objStopWatch.Stop();

            return Result;
        }
        /// <summary>
        /// GET방식으로 데이터 전송
        /// </summary>
        /// <param name="timeOut">밀리세컨드</param>
        /// <returns>URL의 응답</returns>
        public static string SendQueryString(string SendData, string URL, int timeOut)
        {
            System.Diagnostics.Stopwatch objStopWatch = new System.Diagnostics.Stopwatch();
            libCommon.clsUtil objUtil = new libCommon.clsUtil();
            MyWebClient WC = new MyWebClient(timeOut);
            
            string Result;

            try
            {
                objStopWatch.Start();
                Result = WC.DownloadString(URL + "?" + SendData);
            }
            catch (Exception ex)
            {
                objStopWatch.Stop();
                objUtil.writeLog("FAIL SEND GET TRANSFER : " + ex.ToString() + "\r\nELAPSED TIME:" + objStopWatch.Elapsed.ToString());
                Result = "FAIL";
            }

            if (objStopWatch.IsRunning)
                objStopWatch.Stop();

            return Result;
        }
        class MyWebClient : WebClient
        {
            protected int timeOut;
            public MyWebClient(int TimeOut)
            {
                this.timeOut = TimeOut;
            }
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                request.Timeout = this.timeOut;
                return request;
            }
        }
        /// <summary>
        /// iphone, android는 true
        /// </summary>
        public static bool isMobileDevice(System.Web.HttpRequest objRequest)
        {
            string userAgent = objRequest.UserAgent.ToLower();
            
            libCommon.clsUtil objUtil = new libCommon.clsUtil();
            if (userAgent.IndexOf("iphone") > -1 || userAgent.IndexOf("android") > -1)
                return true;
            else
                return false;
        }
    }
}
