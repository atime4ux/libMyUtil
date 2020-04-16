using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Web;

namespace libMyUtil
{
    public static class clsExcel
    {
        /// <summary>
        /// 엑셀2007 이상의 파일(프로퍼티 엑셀 12.0)은 외부엔진 설치 필요, 실패시 빈 DataSet 반환
        /// </summary>
        public static DataSet readExcel(string strFilePath)
        {
            return readExcelData(strFilePath, "", 0);
        }

        /// <summary>
        /// 엑셀2007 이상의 파일(프로퍼티 엑셀 12.0)은 외부엔진 설치 필요, 실패시 빈 DataSet 반환
        /// </summary>
        /// <param name="maxRow">0이면 전체 행 가져오기</param>
        public static DataSet readExcel(string strFilePath, int maxRow)
        {
            return readExcelData(strFilePath, "", maxRow);
        }

        /// <summary>
        /// 엑셀2007 이상의 파일(프로퍼티 엑셀 12.0)은 외부엔진 설치 필요, 실패시 빈 DataSet 반환
        /// </summary>
        public static DataSet readExcel(string strFilePath, string QUERY)
        {
            return readExcelData(strFilePath, QUERY, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFilePath">엑셀 파일 경로</param>
        /// <param name="QUERY"></param>
        /// <param name="maxRow">0이면 전체 행 가져오기</param>
        private static DataSet readExcelData(string strFilePath, string QUERY, int maxRow)
        {
            libCommon.clsUtil objUtil = new libCommon.clsUtil();

            OleDbConnection oleDBCon = null;

            StringBuilder strBuilder = new StringBuilder();
            DataSet DS = new DataSet();

            string strProvider;
            string[] Temp;
            string[] sheetName;
            string[] columnName;
            string strQuery;

            int i;
            int j;

            Temp = objUtil.Split(strFilePath, ".");

            //파일경로가 올바르면 실행
            if (Temp.Length > 0)
            {
                

                if (Temp[Temp.Length - 1].Equals("xlsx"))
                    strProvider = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + strFilePath + "; Extended Properties=Excel 12.0";
                else
                    strProvider = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + strFilePath + "; Extended Properties=Excel 8.0";

                try
                {
                    oleDBCon = new OleDbConnection(strProvider);

                    //접속
                    oleDBCon.Open();

                    //쉬트 네임 가져오기
                    sheetName = getSheetName(oleDBCon);
                    if (sheetName == null)
                        return DS;

                    if (QUERY.Length > 0)
                    {
                        strQuery = QUERY;

                        using (OleDbCommand oleDBCom = new OleDbCommand(strQuery, oleDBCon))
                        {
                            using (OleDbDataReader oleReader = oleDBCom.ExecuteReader())
                            {
                                DS.Tables.Add();
                                DS.Tables[0].Load(oleReader);
                            }
                        }
                    }
                    else
                    {
                        for (i = 0; i < sheetName.Length; i++)
                        {
                            //컬럼명 가져오기
                            columnName = getColumnName(oleDBCon, sheetName[i]);
                            if (columnName == null)
                                return DS;

                            if (maxRow > 0)
                                strQuery = "SELECT TOP " + maxRow.ToString() + " * FROM [" + sheetName[i] + "]";
                            else
                                strQuery = "SELECT * FROM [" + sheetName[i] + "]";

                            //값이 없는 행은 제거하는 조건 추가
                            for (j = 0; j < columnName.Length; j++)
                            {
                                if (j == 0)
                                    strBuilder.Append(" WHERE");
                                else
                                    strBuilder.Append(" OR");

                                strBuilder.Append(string.Format(" [{0}.{1}] IS NOT NULL", sheetName[i], columnName[0]));
                            }
                            strQuery += strBuilder.ToString();

                            using (OleDbCommand oleDBCom = new OleDbCommand(strQuery, oleDBCon))
                            {
                                using (OleDbDataReader oleReader = oleDBCom.ExecuteReader())
                                {
                                    DS.Tables.Add();
                                    DS.Tables[i].Load(oleReader);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    objUtil.writeLog("ERR READ EXCEL" + "\r\n" + ex.ToString());
                }
                
                if (oleDBCon != null)
                {
                    oleDBCon.Close();
                    oleDBCon.Dispose();
                }
            }

            return DS;
        }
        /// <summary>
        /// 엑셀 쉬트명 구하기, 실패시 null 반환
        /// </summary>
        public static string[] getSheetName(OleDbConnection oleDBCon)
        {
            libCommon.clsUtil objUtil = new libCommon.clsUtil();
            DataTable DT_sheetName = null;

            string[] sheetName = null;

            try
            {
                //쉬트명 구하기
                DT_sheetName = oleDBCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });

                if (DT_sheetName.Rows.Count > 0)
                {
                    sheetName = new string[DT_sheetName.Rows.Count];

                    for (int i = 0; i < DT_sheetName.Rows.Count; i++)
                        sheetName[i] = DT_sheetName.Rows[i]["TABLE_NAME"].ToString();
                }
            }
            catch (Exception ex)
            {
                objUtil.writeLog("ERR GET SHEETS NAME" + "\r\n" + ex.ToString());
            }

            return sheetName;
        }

        /// <summary>
        /// 엑셀 컬럼명 구하기, 실패시 null 반환
        /// </summary>
        public static string[] getColumnName(OleDbConnection oleDBCon, string TableName)
        {
            libCommon.clsUtil objUtil = new libCommon.clsUtil();
            DataTable DT_ColumnName = null;

            System.Collections.ArrayList arrList = new System.Collections.ArrayList();
            string[] ColumnName = null;

            try
            {
                //컬럼명 구하기
                DT_ColumnName = oleDBCon.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new Object[] { null, null, TableName, null });
                if (DT_ColumnName.Rows.Count > 0)
                {
                    for (int i = 0; i < DT_ColumnName.Rows.Count; i++)
                    { 
                        if(DT_ColumnName.Rows[i]["TABLE_NAME"].ToString().Equals(TableName))
                            arrList.Add(DT_ColumnName.Rows[i]["COLUMN_NAME"].ToString());
                    }

                    if (arrList.Count > 0)
                    {
                        ColumnName = new string[arrList.Count];

                        for (int i = 0; i < arrList.Count; i++)
                            ColumnName[i] = (string)arrList[i];
                    }
                }
            }
            catch (Exception ex)
            {
                objUtil.writeLog("ERR GET SHEETS NAME" + "\r\n" + ex.ToString());
            }

            return ColumnName;
        }

        /// <summary>
        /// DataTable을 Excel로 변환해서 Response.Write로 기록
        /// </summary>
        public static string DS2Excel(DataTable DT)
        {
            StringBuilder strBuilder = new StringBuilder();

            int i;
            int j;

            //화면 출력할 table 만들기
            strBuilder.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
            strBuilder.Append("     <head>");
            strBuilder.Append("         <meta http-equiv='Content-Type' content = 'text/html;charset=utf-8' />");
            strBuilder.Append("     </head>");
            strBuilder.Append("     <body>");
            strBuilder.Append("         <table border='1'>");
            strBuilder.Append("             <thead>");
            strBuilder.Append("                 <tr>");
            //제목줄 출력
            for (j = 0; j < DT.Columns.Count; j++)
            {
                strBuilder.Append("                 <th bgcolor = '#90BFC6'>");
                strBuilder.Append(DT.Columns[j].ColumnName);
                strBuilder.Append("                 </th>");
            }

            strBuilder.Append("                 </tr'>");
            strBuilder.Append("             </thead>");

            strBuilder.Append("             <tbody>");
            //table 새로운 열이 시작 될때(tr)
            for (i = 0; i < DT.Rows.Count; i++)
            {
                strBuilder.Append("             <tr>");

                //행 출력(td)
                for (j = 0; j < DT.Columns.Count; j++)
                {
                    strBuilder.Append("             <td style ='mso-number-format:\\@;'>");
                    strBuilder.Append(DT.Rows[i][j].ToString());
                    strBuilder.Append("             </td>");
                }

                strBuilder.Append("             </tr>");
            }
            strBuilder.Append("             </tbody>");

            strBuilder.Append("         </table>");
            strBuilder.Append("     </body>");
            strBuilder.Append("</html>");

            return strBuilder.ToString();
        }

        /// <summary>
        /// DataTable을 Excel로 변환해서 Response.Write로 기록
        /// </summary>
        public static void DS2Excel(System.Web.HttpResponse Response, DataTable DT, string FileName)
        {
            StringBuilder strBuilder = new StringBuilder();

            int i;
            int j;

            //화면 출력할 table 만들기
            strBuilder.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
            strBuilder.Append("     <head>");
            strBuilder.Append("         <meta http-equiv='Content-Type' content = 'text/html;charset=utf-8' />");
            strBuilder.Append("     </head>");
            strBuilder.Append("     <body>");
            strBuilder.Append("         <table border='1'>");
            strBuilder.Append("             <thead>");
            strBuilder.Append("                 <tr>");
            //제목줄 출력
            for (j = 0; j < DT.Columns.Count; j++)
            {
                strBuilder.Append("                 <th bgcolor = '#90BFC6'>");
                strBuilder.Append(DT.Columns[j].ColumnName);
                strBuilder.Append("                 </th>");
            }

            strBuilder.Append("                 </tr'>");
            strBuilder.Append("             </thead>");

            strBuilder.Append("             <tbody>");
            //table 새로운 열이 시작 될때(tr)
            for (i = 0; i < DT.Rows.Count; i++)
            {
                strBuilder.Append("             <tr>");

                //행 출력(td)
                for (j = 0; j < DT.Columns.Count; j++)
                {
                    strBuilder.Append("             <td style ='mso-number-format:\\@;'>");
                    strBuilder.Append(DT.Rows[i][j].ToString());
                    strBuilder.Append("             </td>");
                }

                strBuilder.Append("             </tr>");
            }
            strBuilder.Append("             </tbody>");

            strBuilder.Append("         </table>");
            strBuilder.Append("     </body>");
            strBuilder.Append("</html>");

            Response.Clear();
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName);
            Response.Write(strBuilder.ToString());
            Response.End();
        }
    }
}
