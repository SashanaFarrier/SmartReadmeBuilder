namespace SmartReadmeBuilder.Models
{
    public class Note
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string Text { get; set; } = string.Empty;
        public string MarkdownText { get; set; } = string.Empty;

        //public GitHubInfo GitHubInfo { get; set; } = new GitHubInfo();
        //public Note(int id, DateOnly createdOn, string text)
        //{
        //    this.Id = id;
        //    this.Text = text;
        //    this.CreatedOn = createdOn;
        //}

    }

    public class NoteViewModel
    {
        public List<Note> Notes { get; set; } = new List<Note>();
        public string NewNoteText { get; set; } = string.Empty;
        //public GitHubInfo GitHubInfo { get; set; } = new GitHubInfo();
    }



}
