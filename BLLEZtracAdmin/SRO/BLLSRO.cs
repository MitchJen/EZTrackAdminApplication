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
        // Connection to the database
        static string _connectionString = "Data Source = sql-t-ppdeztrac.na.paccar.com; Initial Catalog = EZTracTest; User Id = appEZTracUser; Password=eZ@pp073109; MultipleActiveResultSets=True;";
        DBManager db = new DBManager(DataProvider.SqlServer, _connectionString
            );



        /*************************************************************
        * This method takes in a String that contains a list of SROId's to then
        * be passed into the sql string to run and re-open them within their
        * related tables. Once completed it will terminate. If it fails it will
        * throw an error.
        *************************************************************/
        public void ReOpenSRO(String SROIdString)
        {
            try
            {
                // If there are Id's then open them
                if (SROIdString != null)
                {
                    // add the parameters (or rather the string of Id's)
                    db.CreateParameters(1);
                    db.AddParameters(0, "@SROIDList", SROIdString);

                    // Use query provided in the SQL file
                    string query = File.ReadAllText(@"..\BLLEZtracAdmin\Scripts\ReOpenSRO.sql");
                    
                    db.Open();
                    db.BeginTransaction();
                    int response = db.ExecuteNonQuery(CommandType.Text, query);
                    db.CommitTransaction();
                }
            }
            // Throw exception if the process fails
            catch (Exception)
            {
                throw;
            }
            // Close the connection after everything
            finally
            {
                db.Close();
            }
        }
    }
}
