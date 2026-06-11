using System.ComponentModel.DataAnnotations;

namespace SmartReadmeBuilder.Models
{
    public class Prompt
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "This field is required. Please describe your app.")]
        [MinLength(20, ErrorMessage = "Please provide a more detailed description of your project. A minimum of 20 characters is required to generate a meaningful README.")]
        public string Text { get; set; } = string.Empty;

    }


}
