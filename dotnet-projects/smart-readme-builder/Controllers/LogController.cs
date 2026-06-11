using Esprima.Ast;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Octokit;
using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
//using SmartReadmeBuilder.Repositories;
using SmartReadmeBuilder.Services;
using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text.Json;
using System.Text.RegularExpressions;

namespace SmartReadmeBuilder.Controllers
{
    public class LogController : Controller
    {
        //private readonly IMarkdownRepository _markdownRepository;
        //private readonly IHttpContextAccessor _context;
        //public LogController(IMarkdownRepository markdownRepository, IHttpContextAccessor context)
        //{
        //    _markdownRepository = markdownRepository;
        //    _context = context;
        //}
        private readonly LogService _logService;
        private readonly HttpContextAuthenticationService _authService;
        public LogController(LogService logService, HttpContextAuthenticationService authService)
        {
            _logService = logService;
            _authService = authService;
        }
        public IActionResult Index()
        {
           
            //var prompts = _markdownRepository.GetAllPrompts().OrderByDescending(p => p.CreatedOn).ToList();
            //var markdowns = _markdownRepository.GetAllMarkdowns().OrderByDescending(m => m.CreatedOn).ToList();
            
            //Log log = new Log
            //{
            //    Prompts = prompts,
            //    Markdowns = markdowns
            //};
            var log = _logService.GetSessionLogHistory();

            ViewBag.Log = log;
            
            if(_authService.IsUserAuthenticated())
            {
                //ViewBag.IsAuthenticated = true;
                TempData["GitHubLogin"] = true;
            }  else
            {
                //ViewBag.IsAuthenticated = false;
                TempData["GitHubLogin"] = false;
            }
            //if (User?.Identity?.IsAuthenticated == true)
            //{
            //    var username = User.Identity.Name; // GitHub login name
            //    var claims = User.Claims.ToList(); // All available claims

            //    // You can also extract specific claims like:
            //    var githubId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //    var githubLogin = User.FindFirst(ClaimTypes.Name)?.Value;

            //    TempData["GitHubLogin"] = true;
            //    return View(); // or redirect to dashboard
            //}


            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> DisconnectGitHub()
        //{
        //    await HttpContext.SignOutAsync(); // Clears the auth cookie
        //    HttpContext.Session.Clear();      // Optional: clears session data

        //    return Redirect("https://github.com/settings/applications");

        //    //return RedirectToAction("Index", "Log"); // Redirect to a confirmation page
        //}


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

            //try
            //{

            //    AIClient api = new AIClient();
            //    var response = await api.GetResponseAsync(prompt.Text);

            //    Markdown markdown = new Markdown
            //    {
            //        Text = response,
            //        PromptId = prompt.Id
            //    };

            //    _logService.AddPrompt(prompt);
            //    _markdownRepository.AddMarkdown(markdown);
            //    _markdownRepository.SaveChanges();
            //}
            //catch (Exception ex)
            //{
            //    TempData["API_Error"] = $"Error generating response. Please try again later.";
            //    //prompt.Text = $"Error generating response : {ex.Message}";

            //    return RedirectToAction("Index", "Home");
            //}

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> RegenerateMarkdown(Guid id)
        {
           await _logService.RegeneratePromptResult(id);
            //var existingPrompt = _markdownRepository.GetPromptById(id);
            //var existingMarkdown = _markdownRepository.GetAllMarkdowns().ToList()
            //    .Find(m => m.PromptId == existingPrompt.Id);

            //if (existingPrompt is null) return NotFound("Prompt not found");

            //try
            //{
            //    AIClient api = new AIClient();

            //    var response = await api.GetResponseAsync(existingPrompt.Text);

            //    existingMarkdown.Text = response;

            //    _markdownRepository.SaveChanges();
            //    //return RedirectToAction("Index");
            //}
            //catch (Exception ex)
            //{
            //    existingPrompt.Text = $"Error generating response from OpenAI: {ex.Message}";
            //}

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditPrompt(Guid id)
        {
           var existingPrompt = await _logService.GetExistingPrompt(id);
            //var existingPrompt = _markdownRepository.GetPromptById(id);

            if (existingPrompt is null) return NotFound("Prompt not found");

            return View(existingPrompt);
        }

        [HttpPost]
        public async Task<IActionResult> EditPrompt(Prompt prompt)
        {

            await _logService.UpdateExistingPrompt(prompt);
            //var existingPrompt = _markdownRepository.GetAllPrompts().FirstOrDefault(p => p.Id == prompt.Id);

            //if (string.IsNullOrEmpty(prompt.Text)) return RedirectToAction("Index", new { error = "Prompt text cannot be empty." });

            //if (existingPrompt.Text == prompt.Text) return RedirectToAction("Index");

            //else
            //{
            //    //check if closing punctuation in the form text and prompt text are the only diferences

            //    if (Regex.Replace(existingPrompt.Text, @"[\p{P}]+$", "") == Regex.Replace(prompt.Text, @"[\p{P}]+$", ""))
            //    {
            //        _markdownRepository.UpdatePrompt(prompt);
            //        _markdownRepository.SaveChanges();

            //        return RedirectToAction("Index");

            //    } else
            //    {
            //        try
            //        {

            //            AIClient api = new AIClient();
            //            var existingMarkdown = _markdownRepository.GetAllMarkdowns().ToList()
            //                .Find(m => m.PromptId == existingPrompt.Id);
            //            var response = await api.GetResponseAsync(prompt.Text);
            //            existingPrompt.Text = prompt.Text;
            //           existingMarkdown.Text = response;

            //            _markdownRepository.SaveChanges();

            //        }
            //        catch (Exception ex)
            //        {
            //            ModelState.AddModelError("Text", $"Error updating prompt: {ex.Message}");
            //            return View(prompt);
            //        }
            //    }

            //}

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
            //try
            //{
            //    var existingMarkdown = _markdownRepository.GetAllMarkdowns().ToList()
            //                            .Find(m => m.PromptId == id);
            //    _markdownRepository.DeletePrompt(id);
            //    _markdownRepository.DeleteMarkdown(existingMarkdown.Id);
            //    _markdownRepository.SaveChanges();

            //    TempData["DeleteSuccess"] = "Prompt deleted successfully.";
            //    TempData["DeletedAt"] = DateTime.UtcNow;
            //    return RedirectToAction("Index");
            //}
            //catch (Exception ex)
            //{
            //    ModelState.AddModelError("DeleteError", $"Error deleting prompt: {ex.Message}");
            //    return RedirectToAction("Index");
            //}
            return RedirectToAction("Index");
        }
    }
}
