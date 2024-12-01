using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using System.Data;

namespace XAISample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IChatClient client =
             new OpenAIClient(new ApiKeyCredential(""), new OpenAIClientOptions()
             {
                 Endpoint = new Uri("https://api.x.ai/v1"),
             })
                 .AsChatClient("grok-beta");

            var res = await client.CompleteAsync(new List<ChatMessage>()
            {
               new ChatMessage(ChatRole.System,"You are Grok, a chatbot inspired by the Hitchhiker's Guide to the Galaxy."),
               new ChatMessage(ChatRole.User,"What is the meaning of life, the universe, and everything?")

            });
            Console.WriteLine(res.Choices[0].Text);
        }
    }
}
