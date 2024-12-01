using CustomAITemplate.CustomAIClient;

namespace CustomAITemplate
{
    internal class Program
    {
        static async Task Main(string[] args)
        { 
            var client = new CustomAiApiClient(new CustomAiApiClient.Configuration()
            {
                Model = "",
                Endpoint = new Uri("https://ai.api"),
                ApiKey = "",
            });
            var res = await client.GetChatMessageContentsAsync(new CustomAIClient.Models.Chat.ChatRequest("What is Ai"));
            Console.WriteLine(res.Answer);
        }
    }
}
