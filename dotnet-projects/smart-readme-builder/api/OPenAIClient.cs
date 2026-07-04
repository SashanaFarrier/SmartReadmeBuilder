using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using SmartReadmeBuilder.Services;
using System.Text.Json;



namespace SmartReadmeBuilder.api
{
    public class AIClient
    {
        private readonly GetPromptInstructionsService _promptInstructions;
        private string PromptInstructions => _promptInstructions.GetInstructionsText();
        public AIClient(GetPromptInstructionsService instructions)
        {
            _promptInstructions = instructions;
        }
        public async Task<string> GetResponseAsync(string userInput)
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            var client = new OpenAIClient(apiKey);
            var messages = new List<ChatMessage>
            {
               new SystemChatMessage("You are a helpful assistant that generates professional README.md markdown files."),
               new UserChatMessage(string.Concat(PromptInstructions, userInput))
            };

            var result = await client.GetChatClient(model: "gpt-4.1-nano").CompleteChatAsync(messages);
            var response = result.GetRawResponse().Content;
            using JsonDocument doc = JsonDocument.Parse(response);
            string content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

            return content ?? "No response received from OpenAI.";
        }

    }
}
