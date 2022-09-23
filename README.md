# CONTENTS OF THIS FILE

 * Introduction
 * Requirements
 * Installation
 * Configuration
 * To-Do's
 * Troubleshooting
 * Maintainers

 ## Introduction
 This project is intended to be an EZtrak admin resource center along with an
 Ecommerce Dealers Table editor. The program has a few features of note; Re-Open SRO,
 Add Vendor, Add Vendor Pool, Add Inspector, and Dealer List. These features are intended
 to take the place of the reoccurring Service Requests sent to IT. This program implements
 SQL Server connections, AWS dynamoDB connections, .NET CORE MVC, and PASS Authentication
 (PACCAR's internal credential system). This System is modeled to be of MVC framework design.

 ## Requirements
 This project requires both a Microsoft SQL Server Connection along with AWS DynamoDB
 connection to operate properly. It will also need to be held on a server somewhere to be used.
 
 The Application uses the followint packages (Not all are necessarily used):
 * ADONetHelper.Odbc
 * AspNetCore.Security.Jwt
 * AWSSDK.DynamoDBV2
 * Icpi.data.oledb
 * Microsoft.AspNetCore.Authentication.OpenIdConnect
 * Microsoft.AspNetCore.Http.Extensions
 * Microsoft.AspNetCore.Identity
 * Microsoft.AspNetCore.Identity.UI
 * Microsoft.AspNetCore.Session
 * Microsoft.IdentityModel.Protocols.OpenIdConnect
 * Microsoft.Owin
 * Microsoft.VisualStudio.Web.CodeGeneration
 * Microsoft.VisualStudio.Web.CodeGeneration.Design
 * Microsoft.VisualStudio.Web.CodeGeneration.Utils
 * Microsoft.VisualStudio.Web.CodeGenerators.Mvc
 * Npgsql
 * Serilog
 * Serilog.Extensions.Logging
 * System.Data.Common
 * System.Data.SqlClient
 * xunit.extensibility.core
 * xunit.extensibility.execution

 ## Installation
 Just install as a normal .NET Core App. Run off of server
 or local host.

 ## Configuration
 * Configure user permissions to be limited to EZTrak Admins
	- This is within the SQL Server as a table of Roles to
	  be limited as only a few valid roles have access.
	- Make sure that the SQL server connection string is correct
	  with proper password and username as well as path to connect
	  to the server the correct way.
	- Pass Authentication Configuration Still Needed to pass
	  the authentication cookie to employ proper security.
 * Configure AWS connection with proper user credentials and data
   table client connected to the correct region and user account.

## To Do's Left
 * Configure the PASS authentication system so the users can use their pass
   credentials to access the application.
 * Set up connection to the AWS dynamoDB databse with the connection string
   within the BLLDealers file at the top by configuring the client variable.
    - Make sure to update the _tableName variable with the actual table name.
    - NOTE: The BLLDealers file contains all different table operation methods.
	  Based on my research these should hopefully work but I have not tested them.
	  So make sure to double check them.
	- NOTE: Within the BLLDealers file these functions are mainly dependent on the DealerId
	  being the Partition Key and the DbsId being the Sort Key. I asked for clarification if
	  this was the case but received no response so make sure to double check that fact and
	  re-work accordingly.
   	- (Option) set the AWS profile and key values within the APPSettings.Json file
   	  under AWS.
	- (Option) build the client variabl similar to how we implemented SQL
	  server within the Startup.cs file.
 * Beware that the Dealers List index table may have a bug when a value is searched
   if there are more than 2 pages. I don't know if it will go to the propper 3rd page.
   Keep that in mind when testing, a fix for this could be to in the DealerList Post
   method to pass along a continuing variable or message to the view that indicates
   there has been a search done.
 * Test the Dealers Table to make sure it works as expected.
 * To update the connections to different data tables like Cert or Prod make sure to change
   each connectionString each class in the BLLEZtracAdmin has one of these strings.


## Troubleshooting
* If the PASS authentication fails, check the following:
	- Is the Configuration of the authentication process correct.
	- Don't worry it hasn't been implemented yet. :'(

# Maintainers
Author:
	* Mitchell D. Jensen (Summer 2022)
