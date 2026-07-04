using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SmartReadmeBuilder.Services;

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
