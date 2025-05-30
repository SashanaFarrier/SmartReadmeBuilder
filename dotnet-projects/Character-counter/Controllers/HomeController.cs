using System.Diagnostics;
using SmartReadmeBuilder.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace SmartReadmeBuilder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        NoteViewModel noteView = new NoteViewModel();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
           
            return View(new Character());
        }

        [HttpPost]

        public IActionResult Index(Character model)
        {
            Character character = new Character();
            character.Text = model.Text ?? string.Empty;
            //character.Count = model.CountCharacters();
            character.Words = model.CountWords();
            character.Sentences = model.CountSentences();
            return View(character);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
