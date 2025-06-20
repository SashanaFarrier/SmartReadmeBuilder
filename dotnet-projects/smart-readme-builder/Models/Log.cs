namespace SmartReadmeBuilder.Models
{
    public class Log
    {
        public List<Prompt> Prompts { get; set; } = new List<Prompt>();
        public List<Markdown> Markdowns { get; set; } = new List<Markdown>();
    }
}
