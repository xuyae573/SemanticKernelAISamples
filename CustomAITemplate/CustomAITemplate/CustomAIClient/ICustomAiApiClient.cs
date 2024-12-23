﻿using CustomAITemplate.CustomAIClient.Models;
using CustomAITemplate.CustomAIClient.Models.Chat;
using System.Runtime.CompilerServices;

namespace CustomAITemplate.CustomAIClient
{
    public interface ICustomAiApiClient
    {
        //
        // Summary:
        //     Sends a request to the /api/chat endpoint and streams the response of the chat.
        //     To implement a fully interactive chat, you should make use of the Chat class
        //     with "new Chat(...)"
        //
        // Parameters:
        //   request:
        //     The request to send to your model
        //
        //   cancellationToken:
        //     The token to cancel the operation with
        //
        // Returns:
        //     An asynchronous enumerable that yields ChatResponseStream. Each item represents
        //     a message in the chat response stream. Returns null when the stream is completed.
        //
        //
        // Remarks:
        //     This is the method to call the Ollama endpoint /api/chat. You might not want
        //     to do this manually. To implement a fully interactive chat, you should make use
        //     of the Chat class with "new Chat(...)"
        Task<ChatResponse?> GetChatMessageContentsAsync(ChatRequest request, CancellationToken cancellationToken = default(CancellationToken));

        //
        // Summary:
        //     Sends a request to the /api/chat endpoint and streams the response of the chat.
        //     To implement a fully interactive chat, you should make use of the Chat class
        //     with "new Chat(...)"
        //
        // Parameters:
        //   request:
        //     The request to send to your model
        //
        //   cancellationToken:
        //     The token to cancel the operation with
        //
        // Returns:
        //     An asynchronous enumerable that yields ChatResponseStream. Each item represents
        //     a message in the chat response stream. Returns null when the stream is completed.
        //
        //
        // Remarks:
        //     This is the method to call the Ollama endpoint /api/chat. You might not want
        //     to do this manually. To implement a fully interactive chat, you should make use
        //     of the Chat class with "new Chat(...)"
        IAsyncEnumerable<ChatResponseStream?> GetStreamingChatMessageContentsAsync(ChatRequest request,[EnumeratorCancellation]CancellationToken cancellationToken = default(CancellationToken));

        //
        // Summary:
        //     Sends a request to the /api/embed endpoint to generate embeddings
        //
        // Parameters:
        //   request:
        //     The parameters to generate embeddings for
        //
        //   cancellationToken:
        //     The token to cancel the operation with
        Task<EmbedResponse> Embed(EmbedRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}
