using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomChatCompletionSample.AI
{
    public class SampleChatClient : IChatClient
    {
        public ChatClientMetadata Metadata { get; }

        public SampleChatClient(Uri endpoint, string modelId) =>
            Metadata = new("SampleChatClient", endpoint, modelId);

        public async Task<ChatCompletion> CompleteAsync(
            IList<ChatMessage> chatMessages,
            ChatOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            // Simulate some operation.
            await Task.Delay(300, cancellationToken);

            // Return a sample chat completion response randomly.
            string[] responses =
            [
                "This is the first sample response.",
            "Here is another example of a response message.",
            "This is yet another response message."
            ];

            return new([new ChatMessage()
        {
            Role = ChatRole.Assistant,
            Text = responses[Random.Shared.Next(responses.Length)],
        }]);
        }

        public async IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(
            IList<ChatMessage> chatMessages,
            ChatOptions? options = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Simulate streaming by yielding messages one by one.
            string[] words = ["This ", "is ", "the ", "response ", "for ", "the ", "request."];
            foreach (string word in words)
            {
                // Simulate some operation.
                await Task.Delay(100, cancellationToken);

                // Yield the next message in the response.
                yield return new StreamingChatCompletionUpdate
                {
                    Role = ChatRole.Assistant,
                    Text = word,
                };
            }
        }

        public TService? GetService<TService>(object? key = null) where TService : class =>
            this as TService;

        void IDisposable.Dispose() { }
    }
}
