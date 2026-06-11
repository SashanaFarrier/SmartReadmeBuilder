using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
using SmartReadmeBuilder.Services;
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
        private readonly HttpContextAuthenticationService _authService;
        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor context, HttpContextAuthenticationService authService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            if (_authService.IsUserAuthenticated())
            {
                TempData["GitHubLogin"] = true;
            }   

            //if (User?.Identity?.IsAuthenticated == true)
            //{
            //    var username = User.Identity.Name; // GitHub login name
            //    var claims = User.Claims.ToList(); // All available claims
               
            //    // You can also extract specific claims like:
            //    var githubId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //    var githubLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            //    TempData["GitHubLogin"] = true;
            //    //return View(); // or redirect to dashboard
            //}

            return View();
        }


        //redirect user to the log page after successful Github login, where they can see their log history. 
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/Log" 
            }, "GitHub");
        }

    }
}
