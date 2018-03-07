using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DiaggregatorApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DiaggregatorApp.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ILogger _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login, string returnUrl = null)
        {
            returnUrl = returnUrl ?? "/";

            const string Issuer = "https://contoso.com";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "some user", ClaimValueTypes.String, Issuer)
            };

            if (login.TrustLevel > 9000)
            {
                _logger.LogCritical("ITS OVER 9000!!!!!!!!!!!");
                claims.Add(new Claim(ClaimTypes.Role, "admin", ClaimTypes.Role, Issuer));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "member", ClaimTypes.Role, Issuer));
            }

            var identity = new ClaimsIdentity("user");
            identity.AddClaims(claims);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal, new AuthenticationProperties()
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                IsPersistent = false,
                AllowRefresh = false
            });

            return Url.IsLocalUrl(returnUrl) ? (ActionResult)Redirect(returnUrl) : Redirect("/");
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        public IActionResult Forbidden()
        {
            return View();
        }
    }
}
