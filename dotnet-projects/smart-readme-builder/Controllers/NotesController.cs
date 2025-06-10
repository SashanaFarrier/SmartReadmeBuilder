using Esprima.Ast;
using Microsoft.AspNetCore.Mvc;
using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SmartReadmeBuilder.Controllers
{
    public class NotesController : Controller
    {
        //private readonly GithubClient_API _api;
        GithubClient_API GitHubAPI = new GithubClient_API();

        //[BindProperty]
        //public string ? Owner { get; set; }
        //[BindProperty]
        //public string ? Repo { get; set; }
        //[BindProperty]
        //public string ? Branch { get; set; }
        //[BindProperty]
        //public string ? CommitMessage { get; set; }

        //[BindProperty]
        //public string ? GithubToken { get; set; }

        public IActionResult Index()
        {
            var notesJson = HttpContext.Session.GetString("Notes") ?? "";
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);
            ViewBag.Notes = notes;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PushToGitHub(GitHubInfo gitHubInfo)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var notesJson = HttpContext.Session.GetString("Notes") ?? "";
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);


            if (notes?.Count > 0)
            {
                var note = notes.Find(n => n.Id.Equals(gitHubInfo.NoteId));
                var username = gitHubInfo.Username;
                var repo = gitHubInfo.Repo;
                var branch = gitHubInfo.Branch;
                var commitMessage = gitHubInfo.CommitMessage;
                var githubToken = gitHubInfo.GithubToken;

                if (note is null) return NotFound("Note not found");

                if(!await GitHubAPI.AddFileToRepository(username, repo, branch, commitMessage, githubToken, note.MarkdownText))
                {
                    TempData["PushError"] = "Something went wrong. GitHub credentials could not be authenticated.";
                    return RedirectToAction("Index");
                }

                GitHubAPI.AddFileToRepository(username, repo, branch, commitMessage, githubToken, note.MarkdownText);

                TempData["PushSuccess"] = "Note pushed to GitHub successfully.";
                //GitHubAPI.ConfigureRepo("SashanaFarrier", "test", "master", "Added readme");
                //GitHubAPI.AddFileToRepository(note.MarkdownText).Wait();  
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditPrompt(Note note)
        {
            var notesJson = HttpContext.Session.GetString("Notes") ?? "";
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);
            var existingNote = notes?.Find(n => n.Id.Equals(note.Id));
           
            if(existingNote is null) return NotFound("Note not found");

            return View(existingNote);
        }

        [HttpPost]
        public async Task<IActionResult> EditPrompt(Guid id)
        {
           
            var notesJson = HttpContext.Session.GetString("Notes") ?? "";
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);
            var note = notes.Find(n => n.Id.Equals(id));

            if (notes?.Count > 0)
            {
                if (note is null) return NotFound("Note not found");

                if(string.IsNullOrEmpty(Request.Form["Text"]))
                {
                    //make this into a viewbag error message
                    return RedirectToAction("Index", new { error = "Note text cannot be empty." });
                }

                string formText = Regex.Replace(Request.Form["Text"].ToString() ?? "", @"[\p{P}]+$", "");
                string noteText = Regex.Replace(note.Text ?? "", @"[\p{P}]+$", "");

                note.Text = Request.Form["Text"];

                if (formText == noteText)
                {
                    note.Text = Request.Form["Text"];
                    HttpContext.Session.SetString("Notes", JsonSerializer.Serialize(notes));
                    return RedirectToAction("Index");

                } else
                {
                    try
                    {

                        AIClient api = new AIClient();
                        var response = await api.GetResponseAsync(note.Text);
                        note.MarkdownText = response;

                        HttpContext.Session.SetString("Notes", JsonSerializer.Serialize(notes));
                    }
                    catch (Exception ex)
                    {
                        note.Text = $"Error generating response from OpenAI: {ex.Message}";
                    }
                }
                
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult DeleteNote(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid note ID.");
            }

            var notesJson = HttpContext.Session.GetString("Notes") ?? "";
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);
            
            if (notes?.Count > 0)
            {
                var note = notes.Find(n => n.Id.Equals(id));
                if (note is not null)
                {
                    notes.Remove(note);
                    HttpContext.Session.SetString("Notes", JsonSerializer.Serialize(notes));

                    TempData["DeleteSuccess"] = "Note deleted successfully.";
                    TempData["DeletedAt"] = DateTime.UtcNow;

                    return RedirectToAction("Index");
                   // return View("~/Views/Notes/Index.cshtml", new NoteViewModel {Notes = notes});
                }
            }
            return RedirectToAction("Index");
        }
    }
}
