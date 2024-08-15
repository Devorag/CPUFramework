using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUFramework
{
    public class bizObject
    {
        string _tablename = ""; string _getsproc = ""; string _updatesproc = ""; string _deletesproc = "";
        string _primarykeyname = ""; string _primarykeyparamname = ""; string _messageparam = "";
        DataTable _dataTable = new();
        public bizObject(string tablename)
        {
            _tablename = tablename;
            _getsproc = tablename + "Get";
            _updatesproc = tablename + "Update";
            _deletesproc = tablename + "Delete";
            _primarykeyname = tablename + "Id";
            _primarykeyparamname = "@" + _primarykeyparamname;
            _messageparam = "@" + "Message";
        }
        public DataTable Load(int primarykeyvalue)
        {
            DataTable dt = new();
            SqlCommand cmd = SQLUtility.GetSQLCommand(_getsproc);
            SQLUtility.SetParamValue(cmd, _primarykeyparamname, primarykeyvalue);
            dt = SQLUtility.GetDataTable(cmd);
            _dataTable = dt;
            return dt;
        }

        public string Delete(DataTable dataTable)
        {
            int id = (int)dataTable.Rows[0][_primarykeyname];
            SqlCommand cmd = SQLUtility.GetSQLCommand(_deletesproc);
            SQLUtility.SetParamValue(cmd, _primarykeyparamname, id);
            SQLUtility.SetParamValue(cmd, _messageparam, DBNull.Value);
            SQLUtility.ExecuteSQL(cmd);

            string message = Convert.ToString(cmd.Parameters[_messageparam].Value);
            return message;
        }

        public void Save(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
            {
                throw new Exception($"Cannot call {_tablename } save method because there are no rows in the table");
            }
            DataRow r = dataTable.Rows[0];
            SQLUtility.SaveDataRow(r, _updatesproc);
        }


    }
}
