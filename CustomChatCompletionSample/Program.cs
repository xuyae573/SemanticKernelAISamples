using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using CustomChatCompletionSample.SemanticKernel;

namespace CustomChatCompletionSample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //#region .NET 9 AI Extension ChatClient Method
            //IChatClient client = new SampleChatClient(new Uri("http://coolsite.ai"), "my-custom-model");
            //var response = await client.CompleteAsync("What is AI?");
            //Console.WriteLine(response.Message);
            //#endregion

            #region Semantic Kernel, ChatCompletion Service
            Kernel kernel = Kernel.CreateBuilder().AddOllamaChatCompletion("http://localhost:11434", "llama3")
                .Build();

            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            string prompt = string.Empty;

            var promptMessages = new List<ChatMessageContent>();

            var history = new ChatHistory();
            history.Add(new ChatMessageContent(AuthorRole.System, "As you are a asp.net and react developer"));
        
            while (true)
            {
                // Get user input and add to history
                Console.Write("User > ");
                var userPrompt = Console.ReadLine();
                if (userPrompt?.ToLower() == "quit") break;

                history.AddUserMessage(userPrompt);

                Console.Write("Assistant > ");

                var res = chatCompletionService.GetStreamingChatMessageContentsAsync(history);
                await OutputStreamingResult(res);

                Console.WriteLine();
            }

            #endregion
        }

        private static async Task OutputStreamingResult(IAsyncEnumerable<StreamingChatMessageContent> res)
        {
            await foreach (var result in res)
            {
                Console.Write(result);
            };
        }
    }
}
