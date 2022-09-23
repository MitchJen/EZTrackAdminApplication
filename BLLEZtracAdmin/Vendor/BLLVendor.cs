using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace BLLEZtracAdmin
{
    public class BLLVendor
    {
        // Connection to the database
        string connectionString = "here";
        DBManager db = new DBManager(DataProvider.SqlServer,
            connectionString);
        


        /*************************************************************
        * This method takes in the vendnor code as a string to check against the vendor
        * table. If the vendor already exists then the method will return the vendor id
        * and an exists tag that is marked as exists.
        *************************************************************/
        public void CheckVendor(string vendorCode, out int VendorId, out ExistsTag exists)
        {
            // initialize the output fields
            int result = 0;
            ExistsTag flag = new ExistsTag();
            try
            {
                // Create parameters (vendor code)
                db.CreateParameters(1);
                db.AddParameters(0, "@vendorCode", vendorCode);

                // Query string to check the vendor code
                string query = "Select TOP 1 Id from Vendors where VendorCode = @vendorCode";

                // open and execute the check query
                db.Open();
                object response = db.ExecuteScalar(CommandType.Text, query);

                // if it exists then return the id and flag it
                if (response != null)
                {
                    result = Int32.Parse(response.ToString());
                    flag = ExistsTag.Exists;
                }
            }
            // Throw exception if the query fails
            catch (Exception)
            {
                throw;
            }
            // Close connection after everything
            finally
            {
                db.Close();
            }
            // Update the output variables
            VendorId = result;
            exists = flag;
        }

        /*************************************************************
        * The Get Vendor Pool function will query the vendor pool table and return all
        * the existing vendor pools in a list of pool id's.
        *************************************************************/
        public List<String> GetVendorPool()
        {
            // initialize the output variable
            List<string> result = new List<string>();
            try
            {
                // set the two string variables to get the country Id's and descriptions from tables
                string names = "select Description, CountryId from VendorPool order by Id";
                string country = "select Description from Country where Id = @countryCode";
                db.Open();
                DataTable response = db.ExecuteDataTable(CommandType.Text, names);

                // for each row returned interpret their values and input them into the return list to be passed out
                foreach (DataRow row in response.Rows)
                {
                    string cur_val = row["Description"].ToString();
                    db.CreateParameters(1);
                    db.AddParameters(0, "@countryCode", row["CountryId"]);
                    object countryName = db.ExecuteScalar(CommandType.Text, country);
                    cur_val += ", " + countryName.ToString();
                    result.Add(cur_val);
                }
            }
            // Throw exception if the process fails
            catch (Exception)
            {
                throw;
            }
            finally
            {
                db.Close();
            }
            return result;
        }
        
        // Check Pool takes in the pool Id and returns whether it exists or not
        /*************************************************************
        * This function takes in the vendor pool id and then checks whether the given
        * vendor pool id already exists. If the pool does already exist the function
        * will return an exists tag marked as exists.
        *************************************************************/
        public void CheckPool(string vendorPoolId, out ExistsTag exists)
        {
            // Initialize the output variable
            ExistsTag flag = new ExistsTag();
            try
            {
                // Create the vendor Pool Id parameter
                db.CreateParameters(1);
                db.AddParameters(0, "@vendorPoolId", vendorPoolId);

                // Check query to check the vendor pools
                string query = "Select TOP 1 Id from VendorPool where Id = @vendorPoolId";

                db.Open();
                object response = db.ExecuteScalar(CommandType.Text, query);

                // if the pool Id exists then flag it
                if (response != null)
                {
                    flag = ExistsTag.Exists;
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
            // Update the outgoing variable
            exists = flag;
        }
        
        /*************************************************************
        * The add vendor function takes in the vendor code, vendor name, vendor pool id
        * to add the vendor to the table. It will add the vendor to the vendor table
        * and  return the vendor id of the added vendor and an exists tag marked as not found.
        *************************************************************/
        public void AddVendor(string vendorCode, 
                                string vendorName, 
                                string vendorPoolId, 
                                out int VendorId, 
                                out ExistsTag exists)
        {
            // Initialize the outgoing variables
            int result = 0;
            ExistsTag flag = new ExistsTag();
            try
            {
                // Create parameters (vendor code, vendor name, vendor pool id)
                db.CreateParameters(3);
                db.AddParameters(0, "@vendorCode", vendorCode);
                db.AddParameters(1, "@vendorName", vendorName);
                db.AddParameters(2, "@vendorPoolId", vendorPoolId);

                // Set the query to be the SQL file provided
                string query = File.ReadAllText(@"..\BLLEZtracAdmin\Scripts\AddVendor.sql");
                
                // Execute the query
                db.Open();
                db.BeginTransaction();
                db.ExecuteNonQuery(CommandType.Text, query);
                db.CommitTransaction();

                // Query to check tthat the query was successful and return the Id
                query = "select TOP 1 Id from Vendors where VendorCode = @vendorCode and VendorName = @vendorName and VendorPoolId = @vendorPoolId order by Id desc";
                
                object response = db.ExecuteScalar(CommandType.Text, query);

                // return the resulting Id and flag that it did not previously exist
                if (response != null)
                {
                    result = Int32.Parse(response.ToString());
                    flag = ExistsTag.NotFound;
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
            // Update the outgoing variables
            VendorId = result;
            exists = flag;
        }

        /*************************************************************
        * The add vendor to pool function takes in the vendor namme, currency id, country id, and expiration day
        * then if it exists it will return the existing pool Id and ann exists tag marked as existing.
        * If the vendor pool does not exist then it will add it and return the new vendor pool id.
        * It will also return a exists tag marked as not found.
        *************************************************************/
        public void AddVendorToPool(string vendorName, 
                                    int currencyId, 
                                    int countryId, 
                                    int expDay, 
                                    out int poolId, 
                                    out ExistsTag exists)
        {
            // Initialize the outgoing variables
            int result = 0;
            ExistsTag flag = new ExistsTag();
            try
            {
                // Create the parameters (Description, CurrencyId, CountryId, Eligibility Expiration Day)
                db.CreateParameters(4);
                db.AddParameters(0, "@description", vendorName);
                db.AddParameters(1, "@currencyId", currencyId);
                db.AddParameters(2, "@countryId", countryId);
                db.AddParameters(3, "@eligibilityExpirationDay", expDay);

                // Query to check if the given pool already exists
                string query = "select TOP 1 Id from VendorPool where Description = @description and CurrencyId = @currencyId and CountryId = @countryId and EligibilityExpirationDay = @eligibilityExpirationDay";
                db.Open();
                object response = db.ExecuteScalar(CommandType.Text, query);

                // if it does exist then return the Id and flag that it does
                if (response != null)
                {
                    result = Int32.Parse(response.ToString());
                    flag = ExistsTag.Exists;
                }
                // Else add it to the table
                else
                {
                    query = File.ReadAllText(@"..\BLLEZtracAdmin\Scripts\AddVendorToPool.sql");

                    db.Open();
                    db.BeginTransaction();
                    db.ExecuteNonQuery(CommandType.Text, query);
                    db.CommitTransaction();

                    query = "select TOP 1 Id from VendorPool where Description = @description and CurrencyId = @currencyId and CountryId = @countryId and EligibilityExpirationDay = @eligibilityExpirationDay";
                    db.Open();
                    response = db.ExecuteScalar(CommandType.Text, query);
                    
                    // return the new Id and flag that it did not already exist
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
            // Update the outgoing variables
            poolId = result;
            exists = flag;
        }

        /*************************************************************
        * This function will query the currency table to get each of the currencies
        * in the database and return a list of those currencies to
        * populate the currency drop down input box.
        *************************************************************/
        public List<string> GetCurrency()
        {
            // Initialize the currency list to return
            List<string> result = new List<string>();
            try
            {
                // Select a list of currencies
                string query = "select CurrencyCode from Currency order by Id";
                db.Open();
                DataTable response = db.ExecuteDataTable(CommandType.Text, query);

                // Add each row to the list
                foreach (DataRow row in response.Rows)
                {
                    result.Add(row["CurrencyCode"].ToString());
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

        /*************************************************************
        * This function will query the Country table for a list of all the
        * existing countries that already exist in the database and return
        * those in a list to populate the country drop down input box.
        *************************************************************/
        public List<string> GetCountry()
        {
            // Initialize the country list to return
            List<string> result = new List<string>();
            try
            {
                // Select a list of countries
                string query = "select Description from Country order by Id";
                db.Open();
                DataTable response = db.ExecuteDataTable(CommandType.Text, query);

                // Add each row to the list
                foreach (DataRow row in response.Rows)
                {
                    result.Add(row["Description"].ToString());
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
