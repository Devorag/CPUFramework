using System.Data;
using System.Data.SqlClient;

namespace CPUFramework
{
    public class bizUser : bizObject<bizUser>
    {
        //u.userid, u.roleid, u.username, u.sessionkey, r.rolename, r.rolerank

        public void Login()
        {
            SqlCommand cmd = SQLUtility.GetSQLCommand("UserLogin");
            SQLUtility.SetParamValue(cmd, "username", UserName);
            SQLUtility.SetParamValue(cmd, "password", Password);
            this.Password = "";
            DataTable dt = SQLUtility.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                this.LoadProps(dt.Rows[0]);
            }
        }

        public void Logout()
        {
            SqlCommand cmd = SQLUtility.GetSQLCommand("UserLogout");
            SQLUtility.SetParamValue(cmd, "username", UserName);
            DataTable dt = SQLUtility.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                this.LoadProps(dt.Rows[0]);
            }
        }

        public void LoadBySessionKey()
        {
            SqlCommand cmd = SQLUtility.GetSQLCommand("Users2Get");
            SQLUtility.SetParamValue(cmd, "sessionkey", SessionKey);
            DataTable dt = SQLUtility.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                this.LoadProps(dt.Rows[0]);
            }
        }

        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string SessionKey { get; set; } = "";
        public string RoleName { get; set; } = "";
        public int RoleRank { get; set; }
    }
}
