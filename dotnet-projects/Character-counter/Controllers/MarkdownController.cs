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

            //notes.Add(note);
            //HttpContext.Session.SetString("Notes", JsonSerializer.Serialize(notes));

            //OPenAIClient api = new OPenAIClient("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");

            try
            {
          
                AIClient api = new AIClient(); // Create an instance of the AIClient
                var response = await api.GetResponseAsync(note.Text); // Await the result properly
                note.MarkdownText = response;
                notes.Add(note);
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

            if(notes.Count > 0)
            {
                var selectedNote = notes.Find(n => n.Id.Equals(id)); // Find the previous prompt by Id

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

                        //selectedNote = note; // Update the old note with the new response
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
            var existingNote = notes.Find(n => n.Id.Equals(id));
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
