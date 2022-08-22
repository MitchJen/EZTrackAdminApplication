using System;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmpowerIDOAuthNetCoreSample.Controllers
{
    public class ReturnController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SignInCallback()
        {
            //var token = Request.Form["id_token"];
            //var state = Request.Form["state"];

            //var claims = await ValidateIdentityTokenAsync(token, state);

            //var id = new ClaimsIdentity(claims, "Cookies");
            //Request.GetOwinContext().Authentication.SignIn(id);

            //return Redirect("/");
            return;
        }

    }
}
