using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using DataAccessLayer;

namespace BLLEZtracAdmin
{
    // Exist flag to notify of status of query results
    [Flags]
    public enum ExistsTag
    {
        NotFound = 1,
        Exists = 2
    }
    public class BLLInspector
    {
        // Connection to Database
        DBManager db = new DBManager(DataProvider.SqlServer,
            "Data Source = sql-t-ppdeztrac.na.paccar.com; Initial Catalog = EZTracTest; User Id = appEZTracUser; Password=eZ@pp073109; MultipleActiveResultSets=True;");
        

        
        /*************************************************************
        * The add inspector method takes in the following inputs:
        *   - Inspector array
        * From this array we get the initials, firstName, last name, and
        * location of the inspector to be added to the table. This method will
        * then check the table to see if the inspector already exists within the
        * table and if so it will return the operator id of that inspector and turn the
        * exists tag to be exists. If the inspector does not exist then it will add it,
        * return the new operator Id and turn the exists tag to not found.
        *************************************************************/
        public void AddInspector(string[] inspector, out int OperatorId, out ExistsTag exists)
        {
            // Check inspector. 
            // if exist then return ID. 
            int result = 0;
            ExistsTag flag = new ExistsTag();
            try
            {
                // Add parameters into their given values in the SQL string
                db.CreateParameters(4);
                db.AddParameters(0, "@initials", inspector[0]);
                db.AddParameters(1, "@firstName", inspector[1]);
                db.AddParameters(2, "@lastName", inspector[2]);
                db.AddParameters(3, "@location", inspector[3]);

                // Query string for checking whether it already exists
                string query = "  select TOP 1 OperatorId from OperatorVoiceFunctions where OperatorId = (select id from VoiceOperators where InspectorId = (select id from Inspectors where FirstName = @firstName and LastName = @lastName and Initials = @initials and Location = @location))";
                
                // open and execute the check query
                db.Open();
                object response = db.ExecuteScalar(CommandType.Text, query);
                
                // if the inspector exists then note it by flagging it
                if (response != null)
                {
                    result = Int32.Parse(response.ToString());
                    flag = ExistsTag.Exists;
                }
                // otherwise add the inspector into the table by using SQL query provided
                else
                {
                    query = File.ReadAllText(@"..\BLLEZtracAdmin\Scripts\AddInspector.sql");

                    // open and begin transaction to execute the query
                    db.Open();
                    db.BeginTransaction();
                    db.ExecuteNonQuery(CommandType.Text, query);
                    db.CommitTransaction();

                    // Now check that the inspector has been added correctly
                    query = "select TOP 1 OperatorId from OperatorVoiceFunctions where OperatorId = (select id from VoiceOperators where InspectorId = (select id from Inspectors where FirstName = @firstName and LastName = @lastName and Initials = @initials and Location = @location))";
                    db.Open();
                    response = db.ExecuteScalar(CommandType.Text, query);

                    // if the qury is found state flag as not found to indicate it was added correctly
                    if (response != null)
                    {
                        result = Int32.Parse(response.ToString());
                        flag = ExistsTag.NotFound;
                    }
                }
            }
            // Throw exception if it fails at any point
            catch (Exception)
            {
                throw;
            }
            // Close the db connection after done no matter what
            finally
            {
                db.Close();
            }
            // update the output fields so that the results are passed out
            OperatorId = result;
            exists = flag;
        }

        // Get Location collects each Location in the table and returns the list
        /*************************************************************
        * This method queries the Inspectors table to retrieve all the locations
        * that exist already and return each of those locations in a list.
        * This is used to populate the dropdown box of the location field in the view.
        *************************************************************/
        public List<string> GetLocation()
        {
            // Initialize the location list to return
            List<string> result = new List<string>();
            try
            {
                // Select a list of location by popularity
                string query = "SELECT Location FROM Inspectors GROUP BY Location ORDER BY COUNT(Id) DESC";
                db.Open();
                DataTable response = db.ExecuteDataTable(CommandType.Text, query);

                // Add each row to the list
                foreach (DataRow row in response.Rows)
                {
                    result.Add(row["Location"].ToString());
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

            // Return the list
            return result;
        }
    }
}
