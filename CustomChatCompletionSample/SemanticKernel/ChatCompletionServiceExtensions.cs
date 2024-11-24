using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CustomChatCompletionSample.SemanticKernel
{
    public static class ChatCompletionServiceExtensions
    {
        public static IKernelBuilder AddOllamaChatCompletion(this IKernelBuilder builder, string enpoint, string modelId, string? serviceId = null)
        {
            var instance =  new OllamaChatCompletionService(enpoint, modelId);
            builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, instance);
            return builder;
        }
 
    }
}
