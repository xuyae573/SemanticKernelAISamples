namespace CustomAITemplate.CustomAIClient.Models.Chat
{   
    //adjust it based on the api request format 
    public class ChatRequest : CustomAiRequest
    {
        public ChatRequest(string userContent)
        {
            Messages = new List<Message>()
            {
                new Message("user", userContent)
            };

            ChatOptions = new CustomChatOptions();
            Stream = false;
        }

        public ChatRequest(string modelId, List<Message> messages, CustomChatOptions customChatOptions, bool stream)
        {
           Model = modelId;
           Messages = messages;
           ChatOptions = customChatOptions;
        }

        public List<Message> Messages { get; set; }

        public CustomChatOptions ChatOptions { get; set; }

        public bool Stream { get; set; }

    }
}
