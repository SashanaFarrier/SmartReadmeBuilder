namespace SmartReadmeBuilder.Models
{
    public class Markdown
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PromptId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string Text { get; set; } = string.Empty;
    }
}
