using Esprima.Ast;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
using SmartReadmeBuilder.Repositories;
using SmartReadmeBuilder.ViewModels;
using System.Text.Json;

namespace SmartReadmeBuilder.Controllers
{
    public class MarkdownController : Controller
    {
        private readonly IMarkdownRepository _markdownRepository;

        GithubClient_API GitHubAPI = new GithubClient_API();
        public MarkdownController(IMarkdownRepository markdownRepository)
        {
            _markdownRepository = markdownRepository; 
        }

        
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var prompts = _markdownRepository.GetAllPrompts().ToList();
            var existingMarkdown = prompts?.Find(m => m.MarkdownId.Equals(id));

            if(existingMarkdown is null)
            {
                return NotFound("Markdown not found");
            }

            //MarkdownViewModel markdownViewModel = new MarkdownViewModel
            //{
            //    MarkdownText = existingMarkdown.MarkdownText
            //};

            return View("~/Views/Markdown/Edit.cshtml", existingMarkdown);
        }

        [HttpPost]

        //fix duplicate id
        public IActionResult Edit(Prompt prompt)
        {
            //var notesJson = HttpContext.Session.GetString("Notes") ?? "";
            //var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);
            var prompts = _markdownRepository.GetAllPrompts().ToList();
            var existingMarkdown = prompts?.Find(m => m.MarkdownId.Equals(prompt.MarkdownId));
            if (existingMarkdown is not null)
            {
                existingMarkdown.MarkdownText = prompt.MarkdownText;
                _markdownRepository.SaveChanges();
                //HttpContext.Session.SetString("Notes", JsonSerializer.Serialize(notes));
                return RedirectToAction("Index");
            }
            return NotFound("Markdown not found");
        }

        [HttpPost]
        public async Task<IActionResult> PushToGitHub(GitHubInfo gitHubInfo)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            //var notesJson = HttpContext.Session.GetString("Notes") ?? "";
            //var notes = string.IsNullOrEmpty(notesJson) ? new List<Prompt>() : JsonSerializer.Deserialize<List<Prompt>>(notesJson);
            var markdown = _markdownRepository.GetAllPrompts().ToList().Find(m => m.MarkdownId.Equals(gitHubInfo.MarkdownId));


            if (markdown is null)
            {
                TempData["PushError"] = "No markdown available to push to GitHub.";
                return RedirectToAction("Index");
            }

            //var markdown = markdowns.Find(m => m.MarkdownId.Equals(gitHubInfo.MarkdownId)) ?? null;
            //if (markdown is null) return NotFound("Note not found");

                var username = gitHubInfo.Username;
                var repo = gitHubInfo.Repo;
                var branch = gitHubInfo.Branch;
                var commitMessage = gitHubInfo.CommitMessage;
                var githubToken = gitHubInfo.GithubToken;

             

                if (!await GitHubAPI.AddFileToRepository(username, repo, branch, commitMessage, githubToken, markdown.MarkdownText))
                {
                    TempData["PushError"] = "Something went wrong. GitHub credentials could not be authenticated.";
                    return RedirectToAction("Index");
                }

                GitHubAPI.AddFileToRepository(username, repo, branch, commitMessage, githubToken, markdown.MarkdownText);

                TempData["PushSuccess"] = "Note pushed to GitHub successfully.";
                //GitHubAPI.ConfigureRepo("SashanaFarrier", "test", "master", "Added readme");
                //GitHubAPI.AddFileToRepository(note.MarkdownText).Wait();  
            

            return RedirectToAction("Index");
        }

    }
}
