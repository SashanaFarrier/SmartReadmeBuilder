using System.ComponentModel.DataAnnotations;

namespace SmartReadmeBuilder.Models
{
    public class Prompt
    {
        [Required(ErrorMessage = "This field is required. Please describe your app.")]
        public string Text { get; set; } = string.Empty;

        public bool IsValid => !string.IsNullOrEmpty(Text) && Text.Length > 20;
        public int Count { get; set; } = 0;
        public int Words { get; set; } = 0;

        public int Sentences { get; set; } = 0;
        public int CountCharacters()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                Count = Text.Length;
            }
            else
            {
                Count = 0;
            }

            return Count;
        }

        public int CountWords()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                Words = Text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            }
            else
            {
                Words = 0;
            }

            return Words;
        }

        public int CountSentences()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                Sentences = Text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
            }
            else
            {
                Sentences = 0;
            }
            return Sentences;
        }
    }
}
