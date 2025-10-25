using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;


namespace SmartReadmeBuilder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _context;
        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var username = User.Identity.Name; // GitHub login name
                var claims = User.Claims.ToList(); // All available claims

                // You can also extract specific claims like:
                var githubId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var githubLogin = User.FindFirst(ClaimTypes.Name)?.Value;

                TempData["GitHubLogin"] = true;
                return View(); // or redirect to dashboard
            }

            return View();
        }

        public IActionResult Authorize()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/Log" // or wherever you want to send the user after login
            }, "GitHub");
        }

    }
}
