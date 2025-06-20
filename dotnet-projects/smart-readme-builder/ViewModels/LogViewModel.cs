using SmartReadmeBuilder.Models;

namespace SmartReadmeBuilder.ViewModels
{
    public class LogViewModel
    {
        public List<Prompt> Prompts { get; set; } = new List<Prompt>();
        public List<Markdown> Markdowns { get; set; } = new List<Markdown>();
    }

    //public List<Prompt> GetPrompts()
    //    {
    //        Prompt prompt = new Prompt
    //        {

    //        }
    //    }

    
}
