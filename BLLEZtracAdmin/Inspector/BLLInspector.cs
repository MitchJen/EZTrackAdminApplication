using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using DataAccessLayer;

namespace BLLEZtracAdmin
{
    [Flags]
    public enum ExistsTag
    {
        NotFound = 1,
        Exists = 2
    }
    public class BLLInspector
    {
        DBManager db = new DBManager(DataProvider.SqlServer,
            "Data Source = sql-t-ppdeztrac.na.paccar.com; Initial Catalog = EZTracTest; User Id = appEZTracUser; Password=eZ@pp073109; MultipleActiveResultSets=True;");
        public void AddInspector(string[] inspector, out int OperatorId, out ExistsTag exists)
        {
            // Check inspector. 
            // if exist then return ID. 
            int result = 0;
            ExistsTag flag = new ExistsTag();
            try
            {
                db.CreateParameters(4);
                db.AddParameters(0, "@initials", inspector[0]);
                db.AddParameters(1, "@firstName", inspector[1]);
                db.AddParameters(2, "@lastName", inspector[2]);
                db.AddParameters(3, "@location", inspector[3]);

                string query = "  select TOP 1 OperatorId from OperatorVoiceFunctions where OperatorId = (select id from VoiceOperators where InspectorId = (select id from Inspectors where FirstName = @firstName and LastName = @lastName and Initials = @initials and Location = @location))";
                db.Open();
                object response = db.ExecuteScalar(CommandType.Text, query);
                if (response != null)
                {

                    result = Int32.Parse(response.ToString());
                    flag = ExistsTag.Exists;
                }
                else
                {
                    query = File.ReadAllText(@"..\BLLEZtracAdmin\Scripts\AddInspector.sql");

                    db.Open();
                    db.BeginTransaction();
                    db.ExecuteNonQuery(CommandType.Text, query);
                    db.CommitTransaction();

                    query = "select TOP 1 OperatorId from OperatorVoiceFunctions where OperatorId = (select id from VoiceOperators where InspectorId = (select id from Inspectors where FirstName = @firstName and LastName = @lastName and Initials = @initials and Location = @location))";
                    db.Open();
                    response = db.ExecuteScalar(CommandType.Text, query);
                    if (response != null)
                    {
                        result = Int32.Parse(response.ToString());
                        flag = ExistsTag.NotFound;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                db.Close();
            }
            OperatorId = result;
            exists = flag;
        }
    }
}
