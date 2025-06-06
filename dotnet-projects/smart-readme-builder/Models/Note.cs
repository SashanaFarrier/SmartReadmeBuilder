namespace SmartReadmeBuilder.Models
{
    public class Note
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string Text { get; set; } = string.Empty;
        public string MarkdownText { get; set; } = string.Empty;

    }

    public class NoteViewModel
    {
        public List<Note> Notes { get; set; } = new List<Note>();
        //public string NewNoteText { get; set; } = string.Empty;
    }



}
