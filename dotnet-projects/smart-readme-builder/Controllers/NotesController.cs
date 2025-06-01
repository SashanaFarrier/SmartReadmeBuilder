using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SmartReadmeBuilder.Controllers
{
    public class NotesController : Controller
    {
        private readonly GithubClient_API _api;
        public IActionResult Index()
        {
            var notesJson = HttpContext.Session.GetString("Notes") ?? "";
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);
            ViewBag.Notes = notes;
            return View();
        }

        public NotesController(GithubClient_API api)
        {
            _api = api;
        }

        [HttpPost]
        public async Task<IActionResult> AddReadmeToGitHub(Guid id)
        {
            var notesJson = HttpContext.Session.GetString("Notes") ?? "";
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);

            if(notes.Count > 0)
            {
                var note = notes.Find(n => n.Id.Equals(id));
                _api.ConfigureRepo("SashanaFarrier", "test", "master", "Added readme");
                _api.AddFileToRepository(note.MarkdownText).Wait(); // Wait for the task to complete synchronously
            }

            
            return RedirectToAction("Index");
        }

    }
}
