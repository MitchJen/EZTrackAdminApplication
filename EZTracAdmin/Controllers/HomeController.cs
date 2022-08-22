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
        
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        // GET: ReOpenSRO View
        [HttpGet]
        public IActionResult ReOpenSRO()
        {
            /*
            if ((TempData["AlertMessage"] != null) || (TempData["ErrorMessage"] != null))
            {
                TempData.Keep();
            }
            */
            SRO sro = new SRO();
            return View(sro);
        }

        // POST: ReOpenSRO get the sro id's and pass them to the BLL Layer
        [HttpPost]
        public IActionResult ReOpenSRO(List<string> listKey, SRO sRO)
        {
            // If there are rows of input
            if (listKey != null && listKey.Count > 0)
            {
                // initialize the variables going to be used within this method
                String inputs = "";
                SRO sroId = new SRO();
                bool flag = false;

                // Loop through each sro ID input to the form
                for (int i = 0; i < listKey.Count; i++)
                {
                    int num;
                    Int32.TryParse(listKey[i], out num);
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
                    else
                    {
                        flag = true;
                    }
                }
                // Create BLLSRO and pass the list through the method to reopen the SRO's
                BLLSRO open_sro = new BLLSRO();
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
            // TempData["AlertMessage"] = "SRO Opened Successfully...!";
            return View(sRO);
        }

        // GET: Delete Eligibility Applied And Reset Status
        [Authorize]
        public ActionResult DeleteEligibilityAppliedAndResetStatus()
        {
            return View();
        }

        // GET: AddInspector; create inspector item and pass to view to get filled out
        [HttpGet]
        // [Authorize]
        public ActionResult AddInspector()
        {
            Inspector inspector = new Inspector();
            return View(inspector);
        }

        // POST: AddInspector; take in the form info validate it and pass to BLL layer
        [HttpPost]
        public ActionResult AddInspector(Inspector inspector)
        {
            string[] inspector_info = { inspector.initials, inspector.firstName, inspector.lastName, inspector.location.ToString() };

            // make sure the input is valid if not return the error messages
            if (!ModelState.IsValid)
            {
                return View(inspector);
            }

            int return_value;
            // now create BLL element and Call the BLL method to execute query
            BLLInspector data = new BLLInspector();
            ExistsTag flag = new ExistsTag();
            data.AddInspector(inspector_info, out return_value, out flag);
            if (flag.Equals(ExistsTag.Exists))
            {
                TempData["AlertMessage"] = "Inspector Already Exists! Operator ID Is: " + return_value.ToString() + ".";
            }
            else if (flag.Equals(ExistsTag.NotFound))
            {
                TempData["AlertMessage"] = "Inspector Added Successfully! Operator ID Is: " + return_value.ToString() + ".";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Add and Find given inspector.";
            }
            // TempData["AlertMessage"] = "Inspector Added Successfully...! Operator ID Is: " + return_value.ToString() + ".";
            return Redirect("/Home/AddInspector");
        }

        [Authorize]
        public ActionResult AddNewVendorAndVendorPool()
        {
            return View();
        }

        [Authorize]
        public ActionResult ChangeCRAStatusToInspecting()
        {
            return View();
        }

        [Authorize]
        public ActionResult AddVendorCodeToVendorsTable()
        {
            return View();
        }

        /*
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        */
        #region Private

        private async Task<List<UserApp>> GetAppsForUser(string userId)
        {
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GetAccessToken());

                var res = await client.GetAsync(String.Format("https://sso.dev.paccar.net/oauth/v2/token", userId));

                var json = await res.Content.ReadAsStringAsync();

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<UserApp>>>(json);

                return apiResponse.Data;
            }
        }

        private string GetAccessToken()
        {
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", String.Format(
                    "client_id:{0}, client_secret:{1}", _appSettings.clientID, _appSettings.clientSecret));

                var body = JsonConvert.SerializeObject(new
                {
                    grant_type = "client_credentials"
                });

                var req = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(String.Format("https://sso.dev.paccar.net/oauth/v2/token")),
                    Content = new StringContent(body)
                };

                req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var res = client.SendAsync(req).Result;

                var json = res.Content.ReadAsStringAsync().Result;

                var tokenReponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(json);

                return tokenReponse.AccessToken;
            }
        }

        #endregion
    }

 

}
