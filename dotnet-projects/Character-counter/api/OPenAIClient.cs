using OpenAI;
using OpenAI.Chat;
using System.Text.Json;



namespace SmartReadmeBuilder.api
{
    public class AIClient
    {
        private readonly ChatClient _client;
       //ChatClient client = new("gpt-4.1-nano", "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");

        //public AIClient()
        //{
            
        //    _client = new ChatClient("gpt-4.1-nano", "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");
        //}

        public string GeneratePrompt(string userInput)
        {
            string prompt = $"""  
                Generate a clean, professional README in markdown with sections like:
                - Project Title
                - Description
                - Build With (include only if build tools or languages are mentioned)
                - Features
                - Installation (include only if installation steps or prerequisites are provided)
                - Usage (include only if usage provided)
                - License 

                The README should also be formatted with proper line breaks and spacing exactly like this example:

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
            var client = new OpenAIClient("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");
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
