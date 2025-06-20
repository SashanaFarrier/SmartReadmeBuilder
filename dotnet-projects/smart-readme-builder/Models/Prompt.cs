using System.ComponentModel.DataAnnotations;

namespace SmartReadmeBuilder.Models
{
    public class Prompt
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "This field is required. Please describe your app.")]
        public string Text { get; set; } = string.Empty;

    }


}
