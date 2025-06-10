using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SmartReadmeBuilder.Controllers
{
    public class MarkdownController : Controller
    {
        

        [HttpPost]
        public async Task<IActionResult> GenerateMarkdown(Note note)
        {

            if(string.IsNullOrWhiteSpace(note.Text) || note.Text.Length < 20)
            {
                ModelState.AddModelError("Text", "Please provide a more detailed description of your project. A minimum of 20 characters is required to generate a meaningful README. ");

                return View("~/Views/Home/Index.cshtml");
            }

            var notesJson = HttpContext.Session.GetString("Notes");
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);

            try
            {
          
                AIClient api = new AIClient(); 
                var response = await api.GetResponseAsync(note.Text); 
                note.MarkdownText = response;
               // note.Id = new Guid();

                notes?.Add(note);
                notes = notes?.OrderByDescending(n => n.CreatedOn).ToList();
                HttpContext.Session.SetString("Notes", JsonSerializer.Serialize(notes));

               

            }
            catch (Exception ex)
            {
                note.Text = $"Error generating response from OpenAI: {ex.Message}";
            }

            return RedirectToAction("Index", "Notes");
        }

        [HttpPost]
        public async Task<IActionResult> RegenerateMarkdown(Guid id)
        {
           
            var notesJson = HttpContext.Session.GetString("Notes");
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);

            if(notes?.Count > 0)
            {
                
                var selectedNote = notes.Find(n => n.Id.Equals(id)); 

                if(selectedNote is not null)
                {
                     Note note = new Note();
                     notes.Remove(selectedNote);

                    try
                    {
                        AIClient api = new AIClient();
                       
                        var response = await api.GetResponseAsync(selectedNote.Text);
                        note.CreatedOn = selectedNote.CreatedOn;
                        note.Text = selectedNote.Text;
                        note.MarkdownText = response;

                        notes.Add(note);
                        notes = notes.OrderByDescending(n => n.CreatedOn).ToList();
                        HttpContext.Session.SetString("Notes", JsonSerializer.Serialize(notes));
                       
                    }
                    catch (Exception ex)
                    {
                        note.Text = $"Error generating response from OpenAI: {ex.Message}";
                    }

                }

            }
           
            return RedirectToAction("Index", "Notes");
        }
    }
}
