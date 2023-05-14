using Microsoft.Data.SqlClient;
using System.Data;

namespace AlarmModule.Models
{
    public class Alarms
    {
        public int AlarmId { get; set; }
        public DateTime ActivationTime { get; set; }
        public string AlarmType { get; set; }
        public string Information { get; set; }
        public DateTime? AcknowledgeTime { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Alarms> GetAlarmData(string connectionString)
        {
            SqlConnection con = new SqlConnection(connectionString);
            string sqlQuery = "SELECT * FROM GetAlarmInformation ORDER BY ActivationTime DESC;";
            con.Open();
            SqlCommand cmd = new SqlCommand(sqlQuery, con);

            SqlDataReader dr = cmd.ExecuteReader();

            List<Alarms> alarmList = new List<Alarms>();

            while (dr.Read())
            {
                Alarms alarms = new Alarms();

                alarms.AlarmId = Convert.ToInt32(dr["AlarmId"]);
                alarms.ActivationTime = Convert.ToDateTime(dr["ActivationTime"]);
                alarms.AlarmType = dr["AlarmType"].ToString();
                alarms.Information = dr["Information"].ToString();
                if (dr["AcknowledgeTime"] == DBNull.Value)
                {
                    alarms.AcknowledgeTime = null; ;
                }
                else
                {
                    alarms.AcknowledgeTime = Convert.ToDateTime(dr["AcknowledgeTime"]);
                }
                alarms.Name = dr["Name"].ToString();
                alarms.Description = dr["Description"].ToString();

                alarmList.Add(alarms);
            }
            con.Close();
            return alarmList;
        }

        public void AcknowledgeAlarm(string connectionString, Alarms alarm, string Name)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("AcknowledgeAlarm", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@AlarmId", alarm.AlarmId));
                    cmd.Parameters.Add(new SqlParameter("@Name", Name));

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}