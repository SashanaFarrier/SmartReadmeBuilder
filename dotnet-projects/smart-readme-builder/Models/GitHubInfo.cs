namespace SmartReadmeBuilder.Models
{
    public class GitHubInfo
    {
        public Guid NoteId { get; set; }
        public string? Owner { get; set; }
        public string? Repo { get; set; }
        public string? Branch { get; set; }
        public string? CommitMessage { get; set; }

        //public Note Note { get; set; } = new Note();

        //public GitHubInfo(string owner, string repo, string branch, string commitMessage)
        //{
        //    Owner = owner;
        //    Repo = repo;
        //    Branch = branch;
        //    CommitMessage = commitMessage;
        //}

    }
}
