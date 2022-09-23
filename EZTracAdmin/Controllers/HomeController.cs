using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using EZTracAdminRSC.Models;
using Serilog;
using Serilog.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using BLLEZtracAdmin;


namespace EZTracAdminRSC.Controllers
{
    public class ClaimsResponse
    {
        public string ClaimsText { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppSettings _appSettings;
        public List<Claim> _Claims;

        public HomeController(ILogger<HomeController> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
        }
        
        /****************************************************
        * This method Handles the Index request which returns the
        * default view of the application.
        ****************************************************/
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        // GET: ReOpenSRO View
        /****************************************************
        * This method Handles the Re-Open SRO request which returns
        * the correct view with a passed SRO variable to be used.
        ****************************************************/
        // [Authorize]
        [HttpGet]
        public IActionResult ReOpenSRO()
        {
            SRO sro = new();
            return View(sro);
        }

        // POST: ReOpenSRO get the sro id's and pass them to the BLL Layer
        /****************************************************
        * This method Handles the post method for sending sro's to
        * be opened. It takes in either 1 or a list of SRO's that
        * then will call the BLL layer to execute the SQL query to
        * perform all database actions needed. It then will return
        * a success message back to the user if successful.
        ****************************************************/
        // [Authorize]
        [HttpPost]
        public IActionResult ReOpenSRO(List<string> listKey, SRO sRO)
        {
            // If there are rows of input
            if (listKey != null && listKey.Count > 0)
            {
                // initialize the variables going to be used within this method
                String inputs = "";
                SRO sroId = new();
                bool flag = false;

                // Loop through each sro ID input to the form
                for (int i = 0; i < listKey.Count; i++)
                {
                    _ = int.TryParse(listKey[i], out int num);
                    // Create string of the given values
                    if (num > sroId.minId && num < sroId.maxId)
                    {
                        if (i == 0)
                        {
                            inputs += listKey[i];
                        }
                        else
                        {
                            inputs += "," + listKey[i];
                        }
                    }
                    // else flag that the values are wrong
                    else
                    {
                        flag = true;
                    }
                }
                // Create BLLSRO and pass the list through the method to reopen the SRO's
                BLLSRO open_sro = new();
                if (!flag)
                {
                    open_sro.ReOpenSRO(inputs);
                    // open_sro.ReOpenSRO(sro.IdList);
                    TempData["AlertMessage"] = "SRO Opened Successfully...!";
                }
                else
                {
                    TempData["ErrorMessage"] = "SRO Failed to Open Input Invalid.";
                }
                return View(sroId);
            }
            TempData["AlertMessage"] = "SRO Opened Successfully...!";
            return View(sRO);
        }
        
        // GET: AddInspector; create inspector item and pass to view to get filled out
        /****************************************************
        * This method Handles the Add Inspector request which 
        * returns the Add Inspector view with inspector variable.
        ****************************************************/
        // [Authorize]
        [HttpGet]
        public ActionResult AddInspector()
        {
            Inspector inspector = new();
            BLLInspector inspectorLogic = new();
            inspector.location = inspectorLogic.GetLocation();
            return View(inspector);
        }
        
        // POST: AddInspector; take in the form info validate it and pass to BLL layer
        /****************************************************
        * This method Handles the post request for the add inspector
        * method making sure the given input is valid and if so
        * it will send along a request to the BLL layer to execute
        * the SQL query to add the inspector to the proper table.
        * If the inspector doesn't already exist then it will return
        * the existing Id otherwise if an error occurrs it will show an
        * error message.
        ****************************************************/
        // [Authorize]
        [HttpPost]
        public ActionResult AddInspector(Inspector inspector)
        {
            string[] inspector_info = { 
                inspector.initials, 
                inspector.firstName, 
                inspector.lastName, 
                inspector.location[0].ToString() };

            // make sure the input is valid if not return the error messages
            if (!ModelState.IsValid)
            {
                return View(inspector);
            }

            // now create BLL element and Call the BLL method to execute query
            BLLInspector data = new();
            data.AddInspector(inspector_info, out int return_value, out ExistsTag flag);

            // if exists then return the operator ID otherwise add the current one
            if (flag.Equals(ExistsTag.Exists))
            {
                TempData["AlertMessage"] = "Inspector Already Exists! Operator ID Is: " 
                                            + return_value.ToString() + ".";
            }
            else if (flag.Equals(ExistsTag.NotFound))
            {
                TempData["AlertMessage"] = "Inspector Added Successfully! Operator ID Is: " 
                                            + return_value.ToString() + ".";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Add and Find given inspector.";
            }
            // TempData["AlertMessage"] = "Inspector Added Successfully...! Operator ID Is: " + return_value.ToString() + ".";
            return Redirect("/Home/AddInspector");
        }

        // GET: Add New Vendor
        /****************************************************
        * This method Handles the Add New Vendor request which
        * returns the add new vendor with vendor variable.
        ****************************************************/
        // [Authorize]
        [HttpGet]
        public ActionResult AddNewVendor()
        {
            Vendor vendor = new();
            return View(vendor);
        }

        // POST: Add New Vendor
        /****************************************************
        * This method Handles the post add new vendor case which
        * takes in a vendor variable and a string from a submission
        * button. Then checks that the vendor is valid and if the
        * confirm button or the submit button was pressed. If the
        * confirm button is pressed then it will add that vendor to
        * the table by calling the AddVendor method in the BLL layer.
        * If the submit button is pressed then the method will check if
        * the vendor exists and if it does then it will ask for confirmation
        * and if it doesn't then it will add the vendor to the table.
        *
        * Upon successful addition it will return the Vendor Id of
        * the vendor added to the table.
        ****************************************************/
        // [Authorize]
        [HttpPost]
        public ActionResult AddNewVendor(Vendor vendor, string btnSubmit)
        {
            // Verify the model state to make sure values are correct
            if (!ModelState.IsValid)
            {
                return View(vendor);
            }

            // Create vendor logic to be able to use the BLL layer
            BLLVendor vendorLogic = new();
            int returnValue;
            ExistsTag flag;

            // if they confirm the add then add to table else check add
            if (btnSubmit == "Confirm")
            {
                // add vendor using BLL layer
                vendorLogic.AddVendor(vendor.vendorCode, 
                                        vendor.vendorName, 
                                        vendor.vendorPoolId, 
                                        out returnValue, 
                                        out flag);

                if (flag.Equals(ExistsTag.NotFound))
                {
                    TempData["AlertMessage"] = "Vendor Added Successfully! Vendor Id is: " 
                                                + returnValue.ToString() + ".";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to Add and Find given vendor.";
                }
            }
            // else then check the pools if they already exist otherwise add them
            else
            {
                vendorLogic.CheckPool(vendor.vendorPoolId, out flag);

                // if exists return a warning string asking to confirm
                if (!flag.Equals(ExistsTag.Exists))
                {
                    TempData["ErrorMessage"] = "Vendor Pool: " + vendor.vendorPoolId 
                                                + " Does Not Exist In Vendor Pool Table.";
                    return View(vendor);
                }
                
                // if doesn't exist then add the vendor
                vendorLogic.CheckVendor(vendor.vendorCode, out returnValue, out flag);

                // vendorLogic.AddVendor(vendor.vendorCode, vendor.vendorName, vendor.vendorPoolId, out returnValue, out flag);
                if (flag.Equals(ExistsTag.Exists))
                {
                    TempData["ConfirmMessage"] = "Vendor Already Exists! Vendor ID Is: " 
                                                    + returnValue.ToString() + ". With Vendor Code: " 
                                                    + vendor.vendorCode + ". Confirm to add: ";
                    return View(vendor);
                }
                else
                {
                    vendorLogic.AddVendor(vendor.vendorCode, 
                                            vendor.vendorName, 
                                            vendor.vendorPoolId, 
                                            out returnValue, 
                                            out flag);

                    TempData["AlertMessage"] = "Vendor Added Successfully! Vendor Id is: " 
                                                + returnValue.ToString() + ".";
                }
            }
            return Redirect("/Home/AddNewVendor");
        }

        // GET: Add Vendo to Vendor Pool
        /****************************************************
        * This Handles the get request for the Add Vendor Pool
        * method. It will collect all possible currencies and countries
        * to send to the view to populate the drop down selection boxes
        * and will also send a vendorPool variable to the view.
        ****************************************************/
        // [Authorize]
        [HttpGet]
        public ActionResult AddVendorPool()
        {
            // initialize the data input space and populate dropdown menus
            BLLVendor vendorLogic = new();
            VendorPool vendorPool = new();
            vendorPool.countryId = vendorLogic.GetCountry();
            vendorPool.currencyId = vendorLogic.GetCurrency();

            // pass to post method
            return View(vendorPool);
        }

        // POST: Add Vendor to Vendor Pool
        /****************************************************
        * This method Handles the POST request for the add vendor pool
        * feature where it will take in a vendorPool variable of the
        * VendorPool class. It will then check that that variable is valid
        * and if it is it will then add the vendor pool to the respective
        * table using the addvendortopool function within the BLL layer.
        * 
        * Exists:
        *   If it exists it will return the ID
        * Not Exists:
        *   If not it will add it and return the id
        * 
        ****************************************************/
        // [Authorize]
        [HttpPost]
        public ActionResult AddVendorPool(VendorPool vendorPool)
        {
            // Check that the input values are valid
            if (!ModelState.IsValid)
            {
                return View(vendorPool);
            }

            // initialize a new vendorPool opbject to compare the selected items in droplists
            BLLVendor vendorLogic = new();
            VendorPool pool = new();
            pool.currencyId = vendorLogic.GetCurrency();
            pool.countryId = vendorLogic.GetCountry();

            // convert the selected values to their integer values in tables
            int currency = pool.currencyId.IndexOf(vendorPool.currencyId[0]) + 1;
            int country = pool.countryId.IndexOf(vendorPool.countryId[0]) + 1;

            // interpret the expiration day to be an int
            _ = int.TryParse(vendorPool.eligibilityExpirationDay, out int expDay);
            vendorLogic.AddVendorToPool(vendorPool.description, 
                                        currency, 
                                        country, 
                                        expDay, 
                                        out int poolId, 
                                        out ExistsTag flag);
            
            if (flag.Equals(ExistsTag.Exists))
            {
                TempData["AlertMessage"] = "Vendor Pool Already Exists! Vendor Pool ID Is: " 
                                            + poolId.ToString() + ".";
            }
            else if (flag.Equals(ExistsTag.NotFound))
            {
                TempData["AlertMessage"] = "Vendor Pool Added Successfully! Vendor Pool ID Is: " 
                                            + poolId.ToString() + ".";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Add and Find given vendor pool.";
            }
            return Redirect("/Home/AddVendorToVendorPool");
        }

        [Authorize]
        public ActionResult Login(ClaimsResponse resp)
        {
            var EmpowerIDLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewData["Username"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string result = string.Empty;
            if (User.Identity.IsAuthenticated)
            {
                //string.Join($"{System.Environment.NewLine}", _Claims.Select(x => $"{x.Type} : {x.Value}"));
                foreach (var claims in User.Claims)
                {
                    result += "Claim type: " + claims.Type + " Claims value: " + claims.Value + "\n";
                }
            }
            else
            {
                result = "Failed";
            }
            resp.ClaimsText = result;
            ModelState.Clear();

            return View("Index", resp);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }
}
