
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiaggregatorApp.Controllers
{
    [Route("[controller]/[action]")]
    public class InfoController : Controller
    {
        [Authorize]
        public IActionResult ForMembers()
        {
            return View();
        }

        [Authorize("Admins")]
        public IActionResult ForAdmins()
        {
            return View();
        }
    }
}
