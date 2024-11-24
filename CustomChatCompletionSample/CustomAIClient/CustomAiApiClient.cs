using CustomChatCompletionSample.CustomAIClient.Extension;
using CustomChatCompletionSample.CustomAIClient.Models;
using CustomChatCompletionSample.CustomAIClient.Models.Chat;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CustomChatCompletionSample.CustomAIClient
{
    public class CustomAiApiClient : ICustomAiApiClient
    {
        //
        // Summary:
        //     The configuration for the CustomAI API client.
        public class Configuration
        {
            //
            // Summary:
            //     Gets or sets the URI of the CustomAI API endpoint.
            public Uri Uri { get; set; }

            //
            // Summary:
            //     Gets or sets the model that should be used.
            public string Model { get; set; }
        }

        //
        // Summary:
        //     Gets the HTTP client that is used to communicate with the CustomAI API.
        private readonly HttpClient _client;

        //
        // Summary:
        //     Gets the default request headers that are sent to the CustomAI API.
        public Dictionary<string, string> DefaultRequestHeaders { get; } = new Dictionary<string, string>();


        //
        // Summary:
        //     Gets the serializer options for outgoing web requests like Post or Delete.
        public JsonSerializerOptions OutgoingJsonSerializerOptions { get; } = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };


        //
        // Summary:
        //     Gets the serializer options used for deserializing HTTP responses.
        public JsonSerializerOptions IncomingJsonSerializerOptions { get; } = new JsonSerializerOptions();


        //
        // Summary:
        //     Gets the current configuration of the API client.
        public Configuration Config { get; }

        public string SelectedModel { get; set; }

        //
        // Summary:
        //     Creates a new instance of the CustomAI API client.
        //
        // Parameters:
        //   uriString:
        //     The URI of the CustomAI API endpoint.
        //
        //   defaultModel:
        //     The default model that should be used with CustomAI.
        public CustomAiApiClient(string uriString, string defaultModel = "")
            : this(new Uri(uriString), defaultModel)
        {
        }

        //
        // Summary:
        //     Creates a new instance of the CustomAI API client.
        //
        // Parameters:
        //   uri:
        //     The URI of the CustomAI API endpoint.
        //
        //   defaultModel:
        //     The default model that should be used with CustomAI.
        public CustomAiApiClient(Uri uri, string defaultModel = "")
            : this(new Configuration
            {
                Uri = uri,
                Model = defaultModel
            })
        {
        }

        //
        // Summary:
        //     Creates a new instance of the CustomAI API client.
        //
        // Parameters:
        //   config:
        //     The configuration for the CustomAI API client.
        public CustomAiApiClient(Configuration config)
            : this(new HttpClient
            {
                BaseAddress = config.Uri
            }, config.Model)
        {
        }

        //
        // Summary:
        //     Creates a new instance of the CustomAI API client.
        //
        // Parameters:
        //   client:
        //     The HTTP client to access the CustomAI API with.
        //
        //   defaultModel:
        //     The default model that should be used with CustomAI.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        public CustomAiApiClient(HttpClient client, string defaultModel = "")
        {
            _client = client ?? throw new ArgumentNullException("client");
            Config = new Configuration
            {
                Uri = client.BaseAddress ?? throw new InvalidOperationException("HttpClient base address is not set!"),
                Model = defaultModel
            };
            SelectedModel = defaultModel;
        }

        // Summary:
        //     Sends an embed request to the API and retrieves the corresponding response.
        // Parameters:
        //   request:
        //     The EmbedRequest object containing request details.
        //   cancellationToken:
        //     A token to cancel the operation if needed.
        public Task<EmbedResponse> Embed(EmbedRequest request, CancellationToken cancellationToken = default)
        {
            return PostAsync<EmbedRequest, EmbedResponse>("api/embed", request, cancellationToken);
        }

        // Summary:
        //     Sends a chat message request and retrieves the response from the API.
        public async Task<ChatResponse?> GetChatMessageContentsAsync(ChatRequest request, CancellationToken cancellationToken = default)
        {
            return await PostAsync<ChatRequest, ChatResponse>("api/chat", request, cancellationToken);
        }

        // Summary:
        //     Streams chat messages in real time from the API.
        public IAsyncEnumerable<ChatResponseStream?> GetStreamingChatMessageContentsAsync(ChatRequest request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            return StreamPostAsync<ChatRequest, ChatResponseStream>("api/chat", request, cancellationToken);
        }

        // Summary:
        //     Retrieves the version of the CustomAI API.
        // Returns:
        //     A Version object representing the API version.
        public async Task<Version> GetVersion(CancellationToken cancellationToken = default)
        {
            return Version.Parse((await GetAsync<JsonNode>("api/version", cancellationToken))["version"]?.ToString());
        }


        #region Helper Methods

        // Summary:
        //     Makes a GET request to the specified endpoint and deserializes the response into the specified type.
        private async Task<TResponse> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, endpoint);
            using HttpResponseMessage response = await SendToCustomAiAsync(requestMessage, null, HttpCompletionOption.ResponseContentRead, cancellationToken);
            return JsonSerializer.Deserialize<TResponse>(await response.Content.ReadAsStringAsync(), IncomingJsonSerializerOptions);
        }

        // Summary:
        //     Makes a POST request to the specified endpoint with the given request object, and deserializes the response.
        // Parameters:
        //   endpoint:
        //     The API endpoint to send the request to.
        //   request:
        //     The request object to send.
        // Returns:
        //     The deserialized response object of type TResponse.
        private async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken) where TRequest : CustomAiRequest
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(request, OutgoingJsonSerializerOptions), Encoding.UTF8, "application/json")
            };
            using HttpResponseMessage response = await SendToCustomAiAsync(requestMessage, request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            return JsonSerializer.Deserialize<TResponse>(await response.Content.ReadAsStringAsync(), IncomingJsonSerializerOptions);
        }

        // Summary:
        //     Streams a POST request to the API and returns deserialized objects from the response stream.
        private async IAsyncEnumerable<TResponse?> StreamPostAsync<TRequest, TResponse>(string endpoint, TRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
            where TRequest : CustomAiRequest
            where TResponse: ChatResponseStream
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(request, OutgoingJsonSerializerOptions), Encoding.UTF8, "application/json")
            };
            using HttpResponseMessage response = await SendToCustomAiAsync(requestMessage, request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await foreach (TResponse item in ProcessStreamedChatResponseAsync<TResponse>(response, cancellationToken))
            {
                yield return item;
            }
        }
 
        // Summary:
        //     Processes a streamed chat response and returns ChatResponseStream objects.
        private async IAsyncEnumerable<ChatResponseStream?> ProcessStreamedChatResponseAsync<TResponse>(HttpResponseMessage response, [EnumeratorCancellation] CancellationToken cancellationToken)
            where TResponse : ChatResponseStream
        {
            using Stream stream = await response.Content.ReadAsStreamAsync();
            using StreamReader reader = new StreamReader(stream);
            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                string json = await reader.ReadLineAsync();
                TResponse chatResponseStream = JsonSerializer.Deserialize<TResponse>(json, IncomingJsonSerializerOptions);
                yield return chatResponseStream != null && chatResponseStream.Done ? JsonSerializer.Deserialize<ChatDoneResponseStream>(json, IncomingJsonSerializerOptions) : chatResponseStream;
            }
        }

        // Summary:
        //     Sends an HTTP request message to the CustomAI API, ensuring a successful response.
        protected virtual async Task<HttpResponseMessage> SendToCustomAiAsync(HttpRequestMessage requestMessage, CustomAiRequest? request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            requestMessage.ApplyCustomHeaders(DefaultRequestHeaders, request);
            HttpResponseMessage response = await _client.SendAsync(requestMessage, completionOption, cancellationToken);
            await EnsureSuccessStatusCode(response);
            return response;
        }

        // Summary:
        //     Ensures the response from the API indicates success, otherwise throws an appropriate exception.
        private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                string text = await response.Content.ReadAsStringAsync() ?? string.Empty;
                JsonElement value = default;
                bool flag = false;
                try
                {
                    flag = JsonDocument.Parse(text)?.RootElement.TryGetProperty("error", out value) ?? false;
                }
                catch (JsonException)
                {
                }

                string text2 = (flag ? value.GetString() : text) ?? string.Empty;
                if (text2.Contains("does not support tools"))
                {
                    throw new Exception(text2);
                }

                throw new Exception(text2);
            }

            response.EnsureSuccessStatusCode();
        }

        #endregion
    }
}