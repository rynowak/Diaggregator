
using Microsoft.AspNetCore.Mvc;

namespace DiaggregatorApp
{
    public class TestController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return Content("Hello, world!", "text/plain");
        }

        [HttpGet("/Admin")]
        public IActionResult AdminOnly()
        {
            return Content("Secret stuff!!! SHHHHHHH!", "text/plain");
        }
    }
}