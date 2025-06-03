using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SmartReadmeBuilder.Controllers
{
    public class NotesController : Controller
    {
        //private readonly GithubClient_API _api;
        GithubClient_API GitHubAPI = new GithubClient_API();

        //GitHubInfo GitHubInfo = new GitHubInfo();
        [BindProperty]
        public string ? Owner { get; set; }
        [BindProperty]
        public string ? Repo { get; set; }
        [BindProperty]
        public string ? Branch { get; set; }
        [BindProperty]
        public string ? CommitMessage { get; set; }

        [BindProperty]
        public string ? GithubToken { get; set; }

        public IActionResult Index()
        {
            var notesJson = HttpContext.Session.GetString("Notes") ?? "";
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);
            ViewBag.Notes = notes;
            return View();
        }

        //public NotesController(GithubClient_API api)
        //{
        //    _api = api;
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddReadmeToGitHub(Guid id)
        //{
        //    var notesJson = HttpContext.Session.GetString("Notes") ?? "";
        //    var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);

           
        //    if (notes?.Count > 0)
        //    {
        //        var note = notes.Find(n => n.Id.Equals(id));

        //        if (note is null) return NotFound("Note not found");

        //    }

        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public async Task<IActionResult> PushToGitHub(Guid id)
        {
            var notesJson = HttpContext.Session.GetString("Notes") ?? "";
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);


            if (notes?.Count > 0)
            {
                var note = notes.Find(n => n.Id.Equals(id));

                if (note is null) return NotFound("Note not found");

                GitHubAPI.AddFileToRepository(Owner, Repo, Branch, CommitMessage, GithubToken, note.MarkdownText);
                //GitHubAPI.ConfigureRepo("SashanaFarrier", "test", "master", "Added readme");
                //GitHubAPI.AddFileToRepository(note.MarkdownText).Wait();  
            }

            return RedirectToAction("Index");
        }

    }
}
