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
        DBManager db = new DBManager(DataProvider.SqlServer,
            "Data Source = sql-t-ppdeztrac.na.paccar.com; Initial Catalog = EZTracTest; User Id = appEZTracUser; Password=eZ@pp073109; MultipleActiveResultSets=True;");
        public void CheckVendor(string vendorCode, out int VendorId, out ExistsTag exists)
        {
            int result = 0;
            ExistsTag flag = new ExistsTag();
            try
            {
                db.CreateParameters(1);
                db.AddParameters(0, "@vendorCode", vendorCode);
                string query = "Select TOP 1 Id from Vendors where VendorCode = @vendorCode";

                db.Open();
                object response = db.ExecuteScalar(CommandType.Text, query);
                if (response != null)
                {
                    result = Int32.Parse(response.ToString());
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
            VendorId = result;
            exists = flag;
        }

        public List<String> GetVendorPool()
        {
            List<string> result = new List<string>();
            try
            {
                string names = "select Description, CountryId from VendorPool order by Id";
                string country = "select Description from Country where Id = @countryCode";
                db.Open();
                DataTable response = db.ExecuteDataTable(CommandType.Text, names);
                foreach (DataRow row in response.Rows)
                {
                    string cur_val = row["Description"].ToString();
                    db.CreateParameters(1);
                    db.AddParameters(0, "@countryCode", row["CountryId"]);
                    object countryName = db.ExecuteScalar(CommandType.Text, country);
                    cur_val += ", " + countryName.ToString();
                    result.Add(cur_val);
                    // result.Add(row["CurrencyCode"].ToString());
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
            return result;
        }
        
        
        public void CheckPool(string vendorPoolId, out ExistsTag exists)
        {
            ExistsTag flag = new ExistsTag();
            try
            {
                db.CreateParameters(1);
                db.AddParameters(0, "@vendorPoolId", vendorPoolId);
                string query = "Select TOP 1 Id from VendorPool where Id = @vendorPoolId";

                db.Open();
                object response = db.ExecuteScalar(CommandType.Text, query);
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
            exists = flag;
        }
        

        public void AddVendor(string vendorCode, string vendorName, string vendorPoolId, out int VendorId, out ExistsTag exists)
        {
            int result = 0;
            ExistsTag flag = new ExistsTag();
            try
            {
                db.CreateParameters(3);
                db.AddParameters(0, "@vendorCode", vendorCode);
                db.AddParameters(1, "@vendorName", vendorName);
                db.AddParameters(2, "@vendorPoolId", vendorPoolId);

                
                string query = File.ReadAllText(@"..\BLLEZtracAdmin\Scripts\AddVendor.sql");
                
                db.Open();
                db.BeginTransaction();
                db.ExecuteNonQuery(CommandType.Text, query);
                db.CommitTransaction();

                query = "select TOP 1 Id from Vendors where VendorCode = @vendorCode and VendorName = @vendorName and VendorPoolId = @vendorPoolId order by Id desc";
                
                object response = db.ExecuteScalar(CommandType.Text, query);
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
            VendorId = result;
            exists = flag;
        }

        public void AddVendorToPool(string vendorName, int currencyId, int countryId, int expDay, out int poolId, out ExistsTag exists)
        {
            int result = 0;
            ExistsTag flag = new ExistsTag();
            try
            {
                db.CreateParameters(4);
                db.AddParameters(0, "@description", vendorName);
                db.AddParameters(1, "@currencyId", currencyId);
                db.AddParameters(2, "@countryId", countryId);
                db.AddParameters(3, "@eligibilityExpirationDay", expDay);

                string query = "select TOP 1 Id from VendorPool where Description = @description and CurrencyId = @currencyId and CountryId = @countryId and EligibilityExpirationDay = @eligibilityExpirationDay";
                db.Open();
                object response = db.ExecuteScalar(CommandType.Text, query);
                if (response != null)
                {

                    result = Int32.Parse(response.ToString());
                    flag = ExistsTag.Exists;
                }
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
            poolId = result;
            exists = flag;
        }

        public List<string> GetCurrency()
        {
            List<string> result = new List<string>();
            try
            {
                string query = "select CurrencyCode from Currency order by Id";
                db.Open();
                DataTable response = db.ExecuteDataTable(CommandType.Text, query);
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
            return result;
        }
        public List<string> GetCountry()
        {
            List<string> result = new List<string>();
            try
            {
                string query = "select Description from Country order by Id";
                db.Open();
                DataTable response = db.ExecuteDataTable(CommandType.Text, query);
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
            return result;
        }

    }
}
