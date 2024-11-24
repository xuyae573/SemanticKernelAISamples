using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
using OllamaSharp.Models.Chat;

namespace AISampleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
#pragma warning disable SKEXP0001

            using var ollamaClient = new OllamaApiClient(
                uriString: "http://localhost:11434",
                defaultModel: "llama3");

            var aiChatService = ollamaClient.AsChatCompletionService();
            
            var chatHistory = new ChatHistory();

            // Define agents
            var agents = new List<Func<ChatHistory, Task<string>>>
            {
                async history => await SummarizationAgent(aiChatService, history),
                async history => await QuestionAnswerAgent(aiChatService, history),
                async history => await CreativeAgent(aiChatService, history)
            };

            Console.WriteLine("Multi-Agent Chat System with Ollama");
            Console.WriteLine("Type 'Quit' to exit.");
            Console.WriteLine();

            while (true)
            {
                // Get user input and add to history
                Console.WriteLine("Your prompt:");
                var userPrompt = Console.ReadLine();
                if (userPrompt?.ToLower() == "quit") break;

                chatHistory.Add(new ChatMessageContent(AuthorRole.User, userPrompt));

                // Let each agent respond in turn
                foreach (var agent in agents)
                {
                    Console.WriteLine();
                    Console.WriteLine("Agent Response:");
                    var agentResponse = await agent(chatHistory);
                    Console.WriteLine(agentResponse);

                    // Add the agent's response to the shared chat history
                    chatHistory.Add(new ChatMessageContent(AuthorRole.Assistant, agentResponse));
                }

                Console.WriteLine();
            }
        }
        // Agent 1: Summarization Agent
        private static async Task<string> SummarizationAgent(IChatCompletionService aiChatService, ChatHistory chatHistory)
        {
            Console.WriteLine("[Summarization Agent is processing...]");

            // Provide a specific task context to the agent
            var prompt = "Summarize the conversation so far.";
            chatHistory.Add(new ChatMessageContent(AuthorRole.System, prompt));

            // Generate the response
            string response = "";
            await foreach (var item in aiChatService.GetStreamingChatMessageContentsAsync(chatHistory))
            {
                response += item.Content;
            }
            return response;
        }

        // Agent 2: Question Answer Agent
        private static async Task<string> QuestionAnswerAgent(IChatCompletionService aiChatService, ChatHistory chatHistory)
        {
            Console.WriteLine("[Question Answer Agent is processing...]");

            // Assume the user wants an answer to the latest question
            var lastUserMessage = chatHistory.Last(msg => msg.Role == AuthorRole.User)?.Content;
            if (lastUserMessage == null) return "No question to answer.";

            var prompt = $"Answer this question: \"{lastUserMessage}\"";
            chatHistory.Add(new ChatMessageContent(AuthorRole.System, prompt));

            // Generate the response
            string response = "";
            await foreach (var item in aiChatService.GetStreamingChatMessageContentsAsync(chatHistory))
            {
                response += item.Content;
            }
            return response;
        }

        // Agent 3: Creative Agent
        private static async Task<string> CreativeAgent(IChatCompletionService aiChatService, ChatHistory chatHistory)
        {
            Console.WriteLine("[Creative Agent is processing...]");

            var prompt = "Generate a creative response to the last message.";
            chatHistory.Add(new ChatMessageContent(AuthorRole.System, prompt));

            // Generate the response
            string response = "";
            await foreach (var item in aiChatService.GetStreamingChatMessageContentsAsync(chatHistory))
            {
                response += item.Content;
            }
            return response;
        }

        protected static void OutputLastMessage(ChatHistory chatHistory)
        {
            var message = chatHistory.Last();

            Console.WriteLine($"{message.Role}: {message.Content}");
            Console.WriteLine("------------------------");
        }

        private static void OutputInnerContent(List<ChatResponseStream> innerContent)
        {
            Console.WriteLine($"Model: {innerContent![0].Model}"); // Model doesn't change per chunk, so we can get it from the first chunk only
            Console.WriteLine(" -- Chunk changing data -- ");

            innerContent.ForEach(streamChunk =>
            {
                Console.WriteLine($"Message role: {streamChunk.Message.Role}");
                Console.WriteLine($"Message content: {streamChunk.Message.Content}");
                Console.WriteLine($"Created at: {streamChunk.CreatedAt}");
                Console.WriteLine($"Done: {streamChunk.Done}");
                /// The last message in the chunk is a <see cref="ChatDoneResponseStream"/> type with additional metadata.
                if (streamChunk is ChatDoneResponseStream doneStreamChunk)
                {
                    Console.WriteLine($"Done Reason: {doneStreamChunk.DoneReason}");
                    Console.WriteLine($"Eval count: {doneStreamChunk.EvalCount}");
                    Console.WriteLine($"Eval duration: {doneStreamChunk.EvalDuration}");
                    Console.WriteLine($"Load duration: {doneStreamChunk.LoadDuration}");
                    Console.WriteLine($"Total duration: {doneStreamChunk.TotalDuration}");
                    Console.WriteLine($"Prompt eval count: {doneStreamChunk.PromptEvalCount}");
                    Console.WriteLine($"Prompt eval duration: {doneStreamChunk.PromptEvalDuration}");
                }
                Console.WriteLine("------------------------");
            });
        }
    }
}
