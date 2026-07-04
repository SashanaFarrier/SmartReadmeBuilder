using Microsoft.AspNetCore.Mvc;
using SmartReadmeBuilder.Models;
using SmartReadmeBuilder.Services;

namespace SmartReadmeBuilder.Controllers
{
    public class LogController : Controller
    {
        private readonly LogService _logService;
        private readonly HttpContextAuthenticationService _authService;
        public LogController(LogService logService, HttpContextAuthenticationService authService)
        {
            _logService = logService;
            _authService = authService;
        }
        public IActionResult Index()
        {
            var log = _logService.GetSessionLogHistory();

            ViewBag.Log = log;
            
            if(_authService.IsUserAuthenticated())
            {
                TempData["GitHubLogin"] = true;
            }  else
            {
                TempData["GitHubLogin"] = false;
            }
          
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrompt(Prompt prompt)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home", prompt);
            }

            if(!await _logService.CreateNewMarkdownText(prompt))
            {
                TempData["API_Error"] = $"Error generating response. Please try again later.";
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> RegenerateMarkdown(Guid id)
        {
           await _logService.RegeneratePromptResult(id);
         
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditPrompt(Guid id)
        {
           var existingPrompt = await _logService.GetExistingPrompt(id);
           
            if (existingPrompt is null) return NotFound("Prompt not found");

            return View(existingPrompt);
        }

        [HttpPost]
        public async Task<IActionResult> EditPrompt(Prompt prompt)
        {

            await _logService.UpdateExistingPrompt(prompt);
            
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> DeletePrompt(Guid id)
        {
            if(await _logService.DeleteExistingPrompt(id)) 
            {
               TempData["DeleteSuccess"] = "Prompt deleted successfully.";

                TempData["DeletedAt"] = DateTime.UtcNow;
                return RedirectToAction("Index");
            }
          
            return RedirectToAction("Index");
        }
    }
}
