using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using System.Text.Json;



namespace SmartReadmeBuilder.api
{
    public class AIClient
    {
        //private readonly ChatClient _client;
      
        public string GeneratePrompt(string userInput)
        {
            string prompt = $"""  
                Generate a detailed, clean and professional README in markdown format with sections like:
                - Project Title
                - Description
                - Build With (include only if build tools or languages are mentioned)
                - Features
                - Installation (include only if installation steps or prerequisites are provided)
                - Usage (include only if usage provided)
                - License 

                Include emojis and any other relevant sections based on the based on the information provided. The README should also be formatted with proper line breaks and spacing exactly like this example:

                # Project Title

                ## Description
                Your description here...

                ## Features
                - Feature 1
                - Feature 2

                ## Installation

                ### Prerequisites (include only if prerequisites are provided)
                - ...

                ### Steps (include only if steps are provided)
                1. Step one


                ---  
                {userInput}  
                ---  
                """;
            return prompt;
        }

        public async Task<string> GetResponseAsync(string userInput)
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            var client = new OpenAIClient(apiKey);
            var messages = new List<ChatMessage>
            {
               new SystemChatMessage("You are a helpful assistant that generates professional README.md markdown files."),
               new UserChatMessage(GeneratePrompt(userInput))
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
