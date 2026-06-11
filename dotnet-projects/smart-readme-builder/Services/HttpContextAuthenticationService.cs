namespace SmartReadmeBuilder.Services
{
    public class HttpContextAuthenticationService
    {
        private readonly IHttpContextAccessor _context;
        public HttpContextAuthenticationService(IHttpContextAccessor context)
        {
            _context = context;
        }
        public bool IsUserAuthenticated()
        {
            //var user = _context.HttpContext?.User;
            //var username = user?.Identity?.Name; // GitHub login name
            //var claims = user?.Claims.ToList(); // All available claims

            //// You can also extract specific claims like:
            //var githubId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var githubLogin = user?.FindFirst(ClaimTypes.Name)?.Value;
            return _context.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }
}
