namespace SmartReadmeBuilder.Services
{
    public class GetPromptInstructionsService
    {
        private readonly IConfiguration _configuration;

        public GetPromptInstructionsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetInstructionsText()
        {
           
           return _configuration["Prompt:Instructions"] ?? throw new InvalidOperationException("Prompt instructions not found in configuration.");
        }
    }
}
