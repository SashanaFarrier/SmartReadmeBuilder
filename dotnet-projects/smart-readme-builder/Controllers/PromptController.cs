using Esprima.Ast;
using Microsoft.AspNetCore.Mvc;
using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
using SmartReadmeBuilder.Repositories;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SmartReadmeBuilder.Controllers
{
    public class PromptController : Controller
    {
        private readonly IMarkdownRepository _markdownReopsitory;
     
        public PromptController(IMarkdownRepository markdownRepository)
        {
            _markdownReopsitory = markdownRepository;
        }

        public IActionResult Index()
        {
            ViewBag.Prompts = _markdownReopsitory.GetAllPrompts().OrderByDescending(p => p.CreatedOn).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrompt(Prompt prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt.Text) || prompt.Text.Length < 20)
            {
                ModelState.AddModelError("Text", "Please provide a more detailed description of your project. A minimum of 20 characters is required to generate a meaningful README. ");

                //return View("~/Views/Home/Index.cshtml");
               RedirectToAction("Index");
            }

            try
            {

                AIClient api = new AIClient();
                var response = await api.GetResponseAsync(prompt.Text);
                prompt.MarkdownText = response;
                
                _markdownReopsitory.AddPrompt(prompt);
                _markdownReopsitory.SaveChanges();
            }
            catch (Exception ex)
            {
                prompt.Text = $"Error generating response from OpenAI: {ex.Message}";
            }

            return RedirectToAction("Index", "Prompt");
        }


        [HttpPost]
        public async Task<IActionResult> RegenerateMarkdown(Guid id)
        {
            var existingPrompt = _markdownReopsitory.GetPromptById(id);

            if(existingPrompt is null) return NotFound("Prompt not found");

            try
            {
                AIClient api = new AIClient();

                var response = await api.GetResponseAsync(existingPrompt.Text);
                existingPrompt.MarkdownText = response;

                _markdownReopsitory.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                existingPrompt.Text = $"Error generating response from OpenAI: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditPrompt(Guid id)
        {

            var existingNote = _markdownReopsitory.GetPromptById(id);

            if (existingNote is null) return NotFound("Note not found");

            return View(existingNote);
        }

        [HttpPost]
        public async Task<IActionResult> EditPrompt(Prompt prompt)
        {
            var existingPrompt = _markdownReopsitory.GetAllPrompts().FirstOrDefault(p => p.Id == prompt.Id);

            if (string.IsNullOrEmpty(prompt.Text)) return RedirectToAction("Index", new { error = "Prompt text cannot be empty." });

            if (existingPrompt.Text == prompt.Text) return RedirectToAction("Index");
            
            else
            {
                //check if closing punctuation in the form text and prompt text are the only diferences

                if (Regex.Replace(existingPrompt.Text, @"[\p{P}]+$", "") == Regex.Replace(prompt.Text, @"[\p{P}]+$", ""))
                {
                    _markdownReopsitory.UpdatePrompt(prompt);
                    _markdownReopsitory.SaveChanges();

                    return RedirectToAction("Index");

                } else
                {
                    try
                    {

                        AIClient api = new AIClient();
                        var response = await api.GetResponseAsync(prompt.Text);
                        existingPrompt.Text = prompt.Text;
                        existingPrompt.MarkdownText = response;

                        _markdownReopsitory.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("Text", $"Error updating prompt: {ex.Message}");
                        return View(prompt);
                    }
                }

            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult DeletePrompt(Guid id)
        {
            try
            {
                _markdownReopsitory.DeletePrompt(id);
                _markdownReopsitory.SaveChanges();

                TempData["DeleteSuccess"] = "Note deleted successfully.";
                TempData["DeletedAt"] = DateTime.UtcNow;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("DeleteError", $"Error deleting note: {ex.Message}");
                return RedirectToAction("Index");
            }
            
        }
    }
}
