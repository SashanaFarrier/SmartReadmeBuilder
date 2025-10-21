using Esprima.Ast;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
using SmartReadmeBuilder.Repositories;
using SmartReadmeBuilder.ViewModels;
using System.Security.Cryptography;
using System.Text.Json;

namespace SmartReadmeBuilder.Controllers
{
    public class MarkdownController : Controller
    {
        private readonly IMarkdownRepository _markdownRepository;
        private readonly IHttpContextAccessor _context;
        private readonly GithubClient_API GitHubAPI;

        public MarkdownController(IMarkdownRepository markdownRepository, IHttpContextAccessor context, GithubClient_API githubClient_API)
        {
            _markdownRepository = markdownRepository; 
            _context = context;
            GitHubAPI = githubClient_API;
        }

        
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var existingMarkdown = _markdownRepository.GetMarkdownById(id);

            if (existingMarkdown is null)
            {
                return NotFound("Markdown not found");
            }

            return View("~/Views/Markdown/Edit.cshtml", existingMarkdown);
        }

        [HttpPost]
        public IActionResult Edit(Markdown markdown)
        {

            var existingMarkdown = _markdownRepository.GetMarkdownById(markdown.Id);
            if(existingMarkdown is null) return NotFound("Markdown not found");

            existingMarkdown.Text = markdown.Text;
            _markdownRepository.SaveChanges();

            return RedirectToAction("Index", "Log");

        }

        [HttpPost]
        public async Task<IActionResult> PushToGitHub(GitHubInfo gitHubInfo)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", "Log");
            }

            var markdown = _markdownRepository.GetMarkdownById(gitHubInfo.MarkdownId);

                if (markdown is null)
                {
                    TempData["PushError"] = "No markdown available to push to GitHub.";
                    return RedirectToAction("Index", "Log");
                }

                var repo = gitHubInfo.Repo;
                var branch = gitHubInfo.Branch;
                var commitMessage = gitHubInfo.CommitMessage;

                if (!await GitHubAPI.AddFileToRepository(repo, branch, commitMessage, markdown.Text))
                {
                    TempData["PushError"] = "Something went wrong. GitHub credentials could not be authenticated.";
                    return RedirectToAction("Index", "Log");
                }

                GitHubAPI.AddFileToRepository(repo, branch, commitMessage, markdown.Text);

                TempData["PushSuccess"] = "Markdown file was successfully pushed to GitHub.";

            return RedirectToAction("Index", "Log");

        }
    }
}
