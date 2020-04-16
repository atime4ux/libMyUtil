using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace libMyUtil
{
    public static class clsCmnDB
    {
        static libCommon.clsDB objDB = new libCommon.clsDB();
        static libCommon.clsUtil objUtil = new libCommon.clsUtil();

        /// <summary>
        /// SqlConnection 리턴
        /// </summary>
        /// <param name="initialCatalogue">초기 접속될 DB</param>
        /// <param name="dataSource">DB서버 주소</param>
        public static System.Data.SqlClient.SqlConnection getConnection(string userID, string password, string initialCatalogue, string dataSource)
        {
            string conStr = "Password=" + password + ";Persist Security Info=True;User ID=" + userID + ";Initial Catalog=" + initialCatalogue + ";Data Source=" + dataSource;
            System.Data.SqlClient.SqlConnection dbCon = new System.Data.SqlClient.SqlConnection(conStr);
            
            return dbCon;
        }
        /// <summary>
        /// 실패시 FAIL 리턴
        /// </summary>
        public static string INSERT_DB(System.Data.SqlClient.SqlConnection dbCon, System.Data.SqlClient.SqlTransaction TRX, string tbName, string cols, string vals)
        {
            ArrayList arrCols = new ArrayList();
            ArrayList arrVals = new ArrayList();

            StringBuilder strBuilder = new StringBuilder();
            
            string Result = "";
            int i;

            arrCols.AddRange(objUtil.Split(cols, "|"));
            arrVals.AddRange(objUtil.Split(vals, "|"));

            strBuilder.Append("INSERT INTO " + tbName);
            strBuilder.Append(" (");
            for (i = 0; i < arrCols.Count; i++)
            {
                if (i > 0)
                {
                    strBuilder.Append(", ");
                }
                strBuilder.Append(objUtil.toDb(arrCols[i].ToString()));

            }
            strBuilder.Append(") VALUES (");
            for (i = 0; i < arrVals.Count; i++)
            {
                if (i > 0)
                {
                    strBuilder.Append(", ");
                }
                strBuilder.Append("'");
                strBuilder.Append(objUtil.toDb(arrVals[i].ToString().Replace("%/", "|")));
                strBuilder.Append("'");

            }
            strBuilder.Append(")");

            objUtil.writeLog("INSERT_DB QUERY : " + strBuilder.ToString());

            try
            {
                Result = objDB.ExecuteNonQuery(dbCon, TRX, strBuilder.ToString());
            }
            catch (Exception e)
            {
                objUtil.writeLog("ERR CMN INSERT [" + tbName + "] " + "[" + cols + "] " + "[" + vals + "]");
                objUtil.writeLog("ERR CMN INSERT QUERY : " + strBuilder.ToString());
                objUtil.writeLog("ERR CMN INSERT MSG : " + e.ToString());
                Result = "FAIL";
            }

            return Result;

        }
        /// <summary>
        /// 실패시 FAIL 리턴
        /// </summary>
        public static string UPDATE_DB(System.Data.SqlClient.SqlConnection dbCon, System.Data.SqlClient.SqlTransaction TRX, string tbName, string cols, string vals, string Wcols, string Wvals)
        {
            ArrayList arrCols = new ArrayList();
            ArrayList arrVals = new ArrayList();
            ArrayList arrWCols = new ArrayList();
            ArrayList arrWVals = new ArrayList();

            StringBuilder strBuilder = new StringBuilder();

            string Result = "";
            int i;

            arrCols.AddRange(objUtil.Split(cols, "|"));
            arrVals.AddRange(objUtil.Split(vals, "|"));
            arrWCols.AddRange(objUtil.Split(Wcols, "|"));
            arrWVals.AddRange(objUtil.Split(Wvals, "|"));

            strBuilder.Append("UPDATE " + tbName);
            strBuilder.Append(" SET ");

            for (i = 0; i < arrCols.Count; i++)
            {
                if (i > 0)
                {
                    strBuilder.Append(", ");
                }
                strBuilder.Append("[");
                strBuilder.Append(objUtil.toDb(arrCols[i].ToString()));
                strBuilder.Append("]");
                strBuilder.Append(" = ");
                strBuilder.Append("'");
                strBuilder.Append(objUtil.toDb(arrVals[i].ToString()).Replace("%/", "|"));
                strBuilder.Append("'");

            }
            strBuilder.Append(" WHERE 1=1");
            for (i = 0; i < arrWCols.Count; i++)
            {
                if (arrWCols[i].ToString().Length > 0 && arrWVals[i].ToString().Length > 0)
                {
                    strBuilder.Append(" AND ");
                    strBuilder.Append("[");
                    strBuilder.Append(objUtil.toDb(arrWCols[i].ToString()));
                    strBuilder.Append("]");
                    strBuilder.Append(" = ");
                    strBuilder.Append("'");
                    strBuilder.Append(objUtil.toDb(arrWVals[i].ToString()));
                    strBuilder.Append("'");
                }
                else
                    continue;

            }

            objUtil.writeLog("UPDATE_DB QUERY : " + strBuilder.ToString());

            try
            {
                Result = objDB.ExecuteNonQuery(dbCon, TRX, strBuilder.ToString());
            }
            catch (Exception e)
            {
                objUtil.writeLog("ERR CMN UPDATE [" + tbName + "] " + "[" + cols + "] " + "[" + vals + "]");
                objUtil.writeLog("ERR CMN UPDATE QUERY : " + strBuilder.ToString());
                objUtil.writeLog("ERR CMN UPDATE MSG : " + e.ToString());
                Result = "FAIL";
            }

            return Result;
        }
        /// <summary>
        /// 실패시 FAIL 리턴
        /// </summary>
        public static string DELETE_DB(System.Data.SqlClient.SqlConnection dbCon, System.Data.SqlClient.SqlTransaction TRX, string tbName, string Wcols, string Wvals)
        {
            ArrayList arrWCols = new ArrayList();
            ArrayList arrWVals = new ArrayList();

            StringBuilder strBuilder = new StringBuilder();

            string Result = "";
            int i;

            arrWCols.AddRange(objUtil.Split(Wcols, "|"));
            arrWVals.AddRange(objUtil.Split(Wvals, "|"));

            strBuilder.Append("DELETE FROM " + tbName);
            strBuilder.Append(" WHERE ");
            for (i = 0; i < arrWCols.Count; i++)
            {
                if (i > 0)
                {
                    strBuilder.Append(" AND ");
                }
                strBuilder.Append("[");
                strBuilder.Append(objUtil.toDb(arrWCols[i].ToString()));
                strBuilder.Append("]");
                strBuilder.Append(" = ");
                strBuilder.Append("'");
                strBuilder.Append(objUtil.toDb(arrWVals[i].ToString()));
                strBuilder.Append("'");

            }


            try
            {
                Result = objDB.ExecuteNonQuery(dbCon, TRX, strBuilder.ToString());
            }
            catch (Exception e)
            {
                objUtil.writeLog("ERR CMN DELETE [" + tbName + "] " + "[" + Wcols + "] " + "[" + Wvals + "]");
                objUtil.writeLog("ERR CMN DELETE QUERY : " + strBuilder.ToString());
                objUtil.writeLog("ERR CMN DELETE MSG : " + e.ToString());
                Result = "FAIL";
            }

            return Result;
        }
        /// <summary>
        /// 실패시 FAIL 리턴
        /// </summary>
        public static string getNewID(System.Data.SqlClient.SqlConnection dbCon, System.Data.SqlClient.SqlTransaction TRX, string tbName, string kcol, string Wcols, string Wvals, int len)
        {
            StringBuilder strBuilder = new StringBuilder();
            DataSet DS = new DataSet();

            string Result = "FAIL";
            string dKey = "".PadLeft(len, '0');
            int i;

            ArrayList arrWCols = new ArrayList();
            ArrayList arrWVals = new ArrayList();

            arrWCols.AddRange(objUtil.Split(Wcols, "|"));
            arrWVals.AddRange(objUtil.Split(Wvals, "|"));

            strBuilder.Append("SELECT ISNULL(MAX(" + kcol + "), '" + dKey + "') FROM " + tbName);
            strBuilder.Append(" WHERE 1 = 1");
            for (i = 0; i < arrWCols.Count; i++)
            {
                if (arrWCols[i].ToString().Length > 0)
                {
                    strBuilder.Append(" AND ");
                    strBuilder.Append("[");
                    strBuilder.Append(objUtil.toDb(arrWCols[i].ToString()));
                    strBuilder.Append("]");
                    strBuilder.Append(" = ");
                    strBuilder.Append("'");
                    strBuilder.Append(objUtil.toDb(arrWVals[i].ToString()));
                    strBuilder.Append("'");
                }
            }

            objUtil.writeLog("GET NEW ID QUERY : " + strBuilder.ToString());

            DS = objDB.ExecuteDSQuery(dbCon, TRX, strBuilder.ToString());

            if (validateDS(DS))
            {
                Result = (objUtil.ToInt32(DS.Tables[0].Rows[0][0].ToString()) + 1).ToString().PadLeft(len, '0');
            }

            objUtil.writeLog("GET NEW ID : " + Result);

            return Result;
        }

        /// <summary>
        /// 중복된 값이 없으면 "OK", 중복된 값이 있으면 "DUPE" 리턴
        /// </summary>
        public static string chk_Duplicate(System.Data.SqlClient.SqlConnection dbCon, System.Data.SqlClient.SqlTransaction TRX, string Col, string tbName, string wCols, string wVals)
        {
            StringBuilder strBuilder = new StringBuilder();
            DataSet DS = new DataSet();

            string[] arr_wCols = objUtil.Split(wCols, "|");
            string[] arr_wVals = objUtil.Split(wVals, "|"); ;

            string Result = "";

            int i;

            strBuilder.Append("SELECT");
            strBuilder.Append(" COUNT(" + Col + ")");
            strBuilder.Append(" FROM " + tbName);
            strBuilder.Append(" WHERE 1=1");

            if (wCols.Length > 0)
            {
                for (i = 0; i < arr_wCols.Length; i++)
                {
                    if (arr_wCols[i].Length > 0)
                    {
                        strBuilder.Append(" AND");
                        strBuilder.Append(" " + arr_wCols[i] + "=");
                        strBuilder.Append("'" + arr_wVals[i] + "'");
                    }
                }
            }

            DS = objDB.ExecuteDSQuery(dbCon, TRX, strBuilder.ToString());

            if (DS.Tables[0].Rows[0][0].ToString().Equals("0"))
            {
                Result = "OK";
            }
            else
            {
                Result = "DUPE";
            }

            return Result;
        }
        /// <summary>
        /// DATASET에 Table과 Rows가 존재하면 true
        /// </summary>
        public static bool validateDS(System.Data.DataSet DS)
        {
            bool Result = false;
            
            if (DS != null)
            {
                if (DS.Tables.Count > 0)
                {
                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        Result = true;
                    }
                }
            }

            return Result;
        }
    }
}
