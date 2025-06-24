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
                return View();
            }

            var markdown = _markdownRepository.GetMarkdownById(gitHubInfo.MarkdownId);

            if (markdown is null)
            {
                TempData["PushError"] = "No markdown available to push to GitHub.";
                return RedirectToAction("Index", "Log");
            }

                var username = gitHubInfo.Username;
                var repo = gitHubInfo.Repo;
                var branch = gitHubInfo.Branch;
                var commitMessage = gitHubInfo.CommitMessage;
                var githubToken = gitHubInfo.GithubToken;

                if (!await GitHubAPI.AddFileToRepository(username, repo, branch, commitMessage, githubToken, markdown.Text))
                {
                    TempData["PushError"] = "Something went wrong. GitHub credentials could not be authenticated.";
                    return RedirectToAction("Index", "Log");
                }

                GitHubAPI.AddFileToRepository(username, repo, branch, commitMessage, githubToken, markdown.Text);

                TempData["PushSuccess"] = "Markdown file was successfully pushed to GitHub.";

                return RedirectToAction("Index", "Log");
        }

    }
}
