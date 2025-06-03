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

            var notesJson = HttpContext.Session.GetString("Notes");
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);

            try
            {
          
                AIClient api = new AIClient(); 
                var response = await api.GetResponseAsync(note.Text); 
                note.MarkdownText = response;

                notes?.Add(note);
                HttpContext.Session.SetString("Notes", JsonSerializer.Serialize(notes));
            }
            catch (Exception ex)
            {
                note.Text = $"Error generating response from OpenAI: {ex.Message}";
            }

            return RedirectToAction("Index", "Notes");
        }

        [HttpPost]
        public async Task<IActionResult> GenerateMarkdownFromPreviousPrompt(Guid id)
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
                        note.Text = selectedNote.Text;
                        note.MarkdownText = response;

                        notes.Add(note);
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

        [HttpPut]
        public async Task<IActionResult> UpdateMarkdownPrompt(Guid id)
        {
            var notesJson = HttpContext.Session.GetString("Notes");
            var notes = string.IsNullOrEmpty(notesJson) ? new List<Note>() : JsonSerializer.Deserialize<List<Note>>(notesJson);
            var existingNote = notes?.Find(n => n.Id.Equals(id));
            if (existingNote != null)
            {
                Note note = new Note();
                note.Text = existingNote.Text;
                existingNote.MarkdownText = note.MarkdownText;
                HttpContext.Session.SetString("Notes", JsonSerializer.Serialize(notes));
            }
            return RedirectToAction("Index", "Notes");
        }
    }
}
