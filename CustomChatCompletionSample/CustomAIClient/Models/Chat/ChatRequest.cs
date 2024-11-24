using Microsoft.SemanticKernel;

namespace CustomChatCompletionSample.CustomAIClient.Models.Chat
{
    public class ChatRequest : CustomAiRequest
    {
        public ChatRequest(string modelId, List<ChatMessageContent> messages, CustomChatOptions customChatOptions, bool stream)
        {
           Model = modelId;
           Messages = messages;
           ChatOptions = customChatOptions;
        }

        public string Model { get; set; }

        public List<ChatMessageContent> Messages { get; set; }

        public CustomChatOptions ChatOptions { get; set; }

        public bool Stream { get; set; }

    }
}
