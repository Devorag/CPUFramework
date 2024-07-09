using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace CPUFramework
{
    public class SQLUtility
    {
        public static string ConnectionString = ""; // Ensure this is properly initialized

        public static SqlCommand GetSQLCommand(string sprocname)
        {
            SqlCommand cmd = new SqlCommand(sprocname);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@RecipeId", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@Message", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }); // Add @Message parameter with Output direction

            using (SqlConnection conn = new SqlConnection(SQLUtility.ConnectionString))
            {
                conn.Open();
                cmd.Connection = conn;
                SqlCommandBuilder.DeriveParameters(cmd);
            }
            return cmd;
        }



        public static void SetParamValue(SqlCommand cmd, string paramName, object value)
        {
            try
            {
                if (cmd.Parameters.Contains(paramName))
                {
                    cmd.Parameters[paramName].Value = value ?? DBNull.Value;
                }
                else
                {
                    throw new ArgumentException($"Parameter '{paramName}' not found in SqlCommand.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(cmd.CommandText + ": " + ex.Message, ex);
            }
        }

        public static void ExecuteSQL(SqlCommand cmd)
        {
            DoExecuteSQL(cmd, false);
        }

        public static DataTable GetDataTable(SqlCommand cmd)
        {
            return DoExecuteSQL(cmd, true);
        }

        public static DataTable GetDataTable(string sqlstatement)
        {
            SqlCommand cmd = new SqlCommand(sqlstatement);
            return GetDataTable(cmd);
        }

        public static void ExecuteSQL(string sqlstatement)
        {
            SqlCommand cmd = new SqlCommand(sqlstatement);
            ExecuteSQL(cmd);
        }

        private static DataTable DoExecuteSQL(SqlCommand cmd, bool loadTable)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(SQLUtility.ConnectionString))
            {
                conn.Open();
                cmd.Connection = conn;
                Debug.Print(GetSQL(cmd));
                try
                {
                    if (cmd.CommandType == CommandType.StoredProcedure)
                    {
                        if (loadTable)
                        {
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            adapter.Fill(dt);
                        }
                        else
                        {
                            cmd.ExecuteNonQuery();
                            CheckReturnValue(cmd);
                        }
                    }
                    else
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }
                catch (SqlException ex)
                {
                    string msg = ParseConstraintMsg(ex.Message);
                    throw new Exception(msg);
                }
                catch (InvalidCastException ex)
                {
                    throw new Exception(cmd.CommandText + ": " + ex.Message, ex);
                }
            }
            SetAllColumnsAllowNull(dt);
            return dt;
        }

        private static void CheckReturnValue(SqlCommand cmd)
        {
            int returnValue = 0;
            string msg = "";
            foreach (SqlParameter p in cmd.Parameters)
            {
                if (p.Direction == ParameterDirection.ReturnValue && p.Value != null)
                {
                    returnValue = (int)p.Value;
                }
                if (p.ParameterName.ToLower() == "@message" && p.Value != null)
                {
                    msg = p.Value.ToString();
                }
            }
            if (returnValue != 0)
            {
                if (msg == "")
                {
                    msg = $"{cmd.CommandText} did not perform the requested action.";
                }
                throw new Exception(msg);
            }
        }

        public static string ParseConstraintMsg(string msg)
        {
            string origmsg = msg;
            string prefix = "ck_";
            string msgEnd = "";

            if (!msg.Contains(prefix))
            {
                if (msg.Contains("u_"))
                {
                    prefix = "u_";
                    msgEnd = " must be unique.";
                }
                else if (msg.Contains("F_"))
                {
                    prefix = "F_";
                }
            }
            if (msg.Contains(prefix))
            {
                msg = msg.Replace("\"", "'");
                int pos = msg.IndexOf(prefix) + prefix.Length;
                msg = msg.Substring(pos);
                pos = msg.IndexOf("'");
                if (pos == -1)
                {
                    msg = origmsg;
                }
                else
                {
                    msg = msg.Substring(0, pos);
                    msg = msg.Replace("_", " ");
                    msg = msg + msgEnd;

                    if (prefix == "F_")
                    {
                        var words = msg.Split(" ");
                        if (words.Length > 1)
                        {
                            msg = $"Cannot delete {words[0]} because it has a related {words[1]} record";
                        }
                    }
                }
            }

            if (msg.Contains("547"))
            {
                msg = "Cannot delete recipe because it is part of a meal or cookbook.";
            }

            return msg;
        }


        public static int GetFirstCFirstRValue(string sql)
        {
            int n = 0;
            DataTable dt = GetDataTable(sql);
            if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
            {
                if (dt.Rows[0][0] != DBNull.Value)
                {
                    int.TryParse(dt.Rows[0][0].ToString(), out n);
                }
            }
            return n;
        }

        private static void SetAllColumnsAllowNull(DataTable dt)
        {
            foreach (DataColumn c in dt.Columns)
            {
                c.AllowDBNull = true;
            }
        }

        public static string GetSQL(SqlCommand cmd)
        {
            string val = "";
#if DEBUG
            StringBuilder sb = new StringBuilder();

            if (cmd.Connection != null)
            {
                sb.AppendLine($"--{cmd.Connection.DataSource}-");
                sb.AppendLine($"use {cmd.Connection.Database}");
                sb.AppendLine("go");
            }

            if (cmd.CommandType == CommandType.StoredProcedure)
            {
                sb.AppendLine($"exec {cmd.CommandText}");
                int paramcount = cmd.Parameters.Count - 1;
                int paramnum = 0;
                string comma = ", ";
                foreach (SqlParameter p in cmd.Parameters)
                {
                    if (p.Direction != ParameterDirection.ReturnValue)
                    {
                        if (paramnum == paramcount)
                        {
                            comma = "";
                        }
                        sb.AppendLine($"{p.ParameterName} = {(p.Value == null ? "null" : p.Value.ToString())} {comma} ");
                    }
                    paramnum++;
                }
            }
            else
            {
                sb.AppendLine(cmd.CommandText);
            }

            val = sb.ToString();
#endif
            return val;
        }

        public static void DebugPrintDataTable(DataTable dt)
        {
            foreach (DataRow r in dt.Rows)
            {
                foreach (DataColumn c in dt.Columns)
                {
                    Debug.Print(c.ColumnName + " = " + r[c.ColumnName].ToString());
                }
            }
        }
    }
}
