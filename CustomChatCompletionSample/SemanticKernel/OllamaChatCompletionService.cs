using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Services;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using System.Text;

namespace CustomChatCompletionSample.SemanticKernel
{
    public class OllamaChatCompletionService : IChatCompletionService
    {
        public IReadOnlyDictionary<string, object?> Attributes => metadata.AsReadOnly();

        private readonly IDictionary<string, object?> metadata;

        private OllamaApiClient ollamaApiClient;

        public OllamaChatCompletionService(string endpoint = "http://localhost:11434", string modelId = "ollama3")
        {
            ollamaApiClient = new OllamaApiClient(
                  uriString: endpoint,
                  defaultModel: modelId);

            metadata = new Dictionary<string, object?>
            {
                { "model", modelId },
                { "client", nameof(OllamaApiClient) },
                { "endpoint", endpoint }
            };
        }

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
              ChatHistory chatHistory,
              PromptExecutionSettings? executionSettings = null,
              Kernel? kernel = null,
              CancellationToken cancellationToken = default)
        {
            // Ensure there is a chat history to process
            if (chatHistory == null || !chatHistory.Any())
            {
                throw new ArgumentException("Chat history is empty or null.", nameof(chatHistory));
            }
            var currentSize = chatHistory.Count;
            // Initialize the Ollama API client

            // Combine all chat history messages into a single prompt
            var messages = chatHistory.ToList().Select(m => new Message(m.Role.ToString(), m.Content.ToString()));

            // Prepare to collect responses
            var chatMessageContents = new List<ChatMessageContent>();

            var chatRequest = new ChatRequest
            {
                Messages = messages,
                Model = metadata["model"].ToString()
            };
            var fullResponseBuilder = new StringBuilder();
            // Call the Ollama API to get a chat response
            await foreach (var chatResponseStream in ollamaApiClient.Chat(chatRequest, cancellationToken))
            {
                // Skip null or empty responses
                if (chatResponseStream?.Message?.Content == null || string.IsNullOrWhiteSpace(chatResponseStream.Message.Content))
                {
                    continue;
                }

                var streamingMessage = new StreamingChatMessageContent(
                  AuthorRole.Assistant,
                  chatResponseStream.Message.Role.ToString(),
                  chatResponseStream.Message.Content);

                // Append the chunk to the full response builder
                fullResponseBuilder.Append(chatResponseStream.Message.Content);

            }

            chatHistory.AddAssistantMessage(fullResponseBuilder.ToString());

            chatMessageContents.Add(new ChatMessageContent(AuthorRole.Assistant, fullResponseBuilder.ToString()));
            return chatMessageContents;
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            // Ensure there is a chat history to process
            if (chatHistory == null || !chatHistory.Any())
            {
                throw new ArgumentException("Chat history is empty or null.", nameof(chatHistory));
            }

            // Combine all chat history messages into a list of Message objects
            var messages = chatHistory.Select(m => new Message(m.Role.ToString(), m.Content)).ToList();

            // Create the chat request
            var chatRequest = new ChatRequest
            {
                Messages = messages,
                Model = metadata["model"].ToString()
            };

            // Variable to collect the full response for the current message
            var fullResponseBuilder = new StringBuilder();

            // Call the Ollama API and process the streaming responses
            await foreach (var chatResponseStream in ollamaApiClient.Chat(chatRequest, cancellationToken))
            {
                if (chatResponseStream == null || string.IsNullOrWhiteSpace(chatResponseStream.Message.Content))
                {
                    continue; // Skip empty or null responses
                }

                var streamingMessage = new StreamingChatMessageContent(
                   AuthorRole.Assistant, chatResponseStream.Message.Content);

                // Append the chunk to the full response builder
                fullResponseBuilder.Append(chatResponseStream.Message.Content);

                yield return streamingMessage;
            }


            if (fullResponseBuilder.Length > 0)
            {
                chatHistory.AddAssistantMessage(fullResponseBuilder.ToString());
            }
        }
    }
}
