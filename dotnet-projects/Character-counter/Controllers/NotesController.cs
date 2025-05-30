using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SmartReadmeBuilder.Controllers
{
    public class NotesController : Controller
    {
        public IActionResult Index()
        {
            var notesJson = HttpContext.Session.GetString("Notes") ?? "";
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);
            ViewBag.Notes = notes;
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> SaveNotes(Note note)
        //{
            

        //    var notesJson = HttpContext.Session.GetString("Notes");
        //    var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);


        //    try
        //    {
        //        AIClient api = new AIClient(); // Create an instance of the AIClient
        //        var response = await api.GetResponseAsync(note.Text); // Await the result properly
        //        note.Text = response;
        //    }
        //    catch (Exception ex)
        //    {
        //        note.Text = $"Error generating response from OpenAI: {ex.Message}";
        //    }

        //    notes.Add(note);
        //    HttpContext.Session.SetString("Notes", JsonSerializer.Serialize(notes));

        //    return RedirectToAction("Index");
        //}

    }
}
