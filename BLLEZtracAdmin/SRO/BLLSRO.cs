using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using DataAccessLayer;

namespace BLLEZtracAdmin
{
    public class BLLSRO
    {
        static string _connectionString = "Data Source = sql-t-ppdeztrac.na.paccar.com; Initial Catalog = EZTracTest; User Id = appEZTracUser; Password=eZ@pp073109; MultipleActiveResultSets=True;";
        DBManager db = new DBManager(DataProvider.SqlServer, _connectionString
            );
        public void ReOpenSRO(String SROIdString)
        {
            try
            {
                if (SROIdString != null)
                {
                    db.CreateParameters(1);
                    db.AddParameters(0, "@SROIDList", SROIdString);
                    string query = File.ReadAllText(@"..\BLLEZtracAdmin\Scripts\ReOpenSRO.sql");

                    db.Open();
                    db.BeginTransaction();
                    int response = db.ExecuteNonQuery(CommandType.Text, query);
                    db.CommitTransaction();
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
        }
    }
}
