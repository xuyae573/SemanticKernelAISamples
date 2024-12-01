using CustomAITemplate.CustomAIClient.Extension;
using CustomAITemplate.CustomAIClient.Models.Chat;
using CustomAITemplate.CustomAIClient.Models;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text;

namespace CustomAITemplate.CustomAIClient
{
    public class CustomAiApiClient : ICustomAiApiClient
    {
        // Define endpoint constants
        private const string ChatEndpoint = "api/chat";
        private const string EmbedEndpoint = "api/embed";

        public class Configuration
        {
            public Uri Endpoint { get; set; }
            public string Model { get; set; } = string.Empty;
            public string ApiKey { get; set; } = string.Empty;
            public string ApiSecret { get; set; } = string.Empty;
            public string JwtToken { get; set; } = string.Empty;
        }

        private readonly HttpClient _client;
        public Dictionary<string, string> DefaultRequestHeaders { get; } = new Dictionary<string, string>();
        public JsonSerializerOptions OutgoingJsonSerializerOptions { get; } = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public JsonSerializerOptions IncomingJsonSerializerOptions { get; } = new JsonSerializerOptions();
        public Configuration Config { get; }

        public CustomAiApiClient(Configuration config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            _client = new HttpClient
            {
                BaseAddress = Config.Endpoint
            };
        }

        public CustomAiApiClient(string uriString, string defaultModel = "")
            : this(new Configuration
            {
                Endpoint = new Uri(uriString),
                Model = defaultModel
            })
        {
        }

        public CustomAiApiClient(Uri uri, string defaultModel = "")
            : this(new Configuration
            {
                Endpoint = uri,
                Model = defaultModel
            })
        {
        }

        public async Task<EmbedResponse> Embed(EmbedRequest request, CancellationToken cancellationToken = default)
        {
            return await PostAsync<EmbedRequest, EmbedResponse>(EmbedEndpoint, request, cancellationToken);
        }

        public async Task<ChatResponse?> GetChatMessageContentsAsync(ChatRequest request, CancellationToken cancellationToken = default)
        {
            return await PostAsync<ChatRequest, ChatResponse>(ChatEndpoint, request, cancellationToken);
        }

        public IAsyncEnumerable<ChatResponseStream?> GetStreamingChatMessageContentsAsync(ChatRequest request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            return StreamPostAsync<ChatRequest, ChatResponseStream>(ChatEndpoint, request, cancellationToken);
        }

        #region Helper Methods

        private async Task<TResponse> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(Config.Endpoint, endpoint));
            using HttpResponseMessage response = await SendToCustomAiAsync(requestMessage, null, HttpCompletionOption.ResponseContentRead, cancellationToken);
            return JsonSerializer.Deserialize<TResponse>(await response.Content.ReadAsStringAsync(), IncomingJsonSerializerOptions);
        }

        private async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken)
            where TRequest : CustomAiRequest
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(Config.Endpoint, endpoint))
            {
                Content = new StringContent(JsonSerializer.Serialize(request, OutgoingJsonSerializerOptions), Encoding.UTF8, "application/json")
            };
            using HttpResponseMessage response = await SendToCustomAiAsync(requestMessage, request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            return JsonSerializer.Deserialize<TResponse>(await response.Content.ReadAsStringAsync(), IncomingJsonSerializerOptions);
        }

        private async IAsyncEnumerable<TResponse?> StreamPostAsync<TRequest, TResponse>(string endpoint, TRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
            where TRequest : CustomAiRequest
            where TResponse : ChatResponseStream
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(Config.Endpoint, endpoint))
            {
                Content = new StringContent(JsonSerializer.Serialize(request, OutgoingJsonSerializerOptions), Encoding.UTF8, "application/json")
            };
            using HttpResponseMessage response = await SendToCustomAiAsync(requestMessage, request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await foreach (TResponse item in ProcessStreamedChatResponseAsync<TResponse>(response, cancellationToken))
            {
                yield return item;
            }
        }

        private async IAsyncEnumerable<TResponse?> ProcessStreamedChatResponseAsync<TResponse>(HttpResponseMessage response, [EnumeratorCancellation] CancellationToken cancellationToken)
            where TResponse : ChatResponseStream
        {
            using Stream stream = await response.Content.ReadAsStreamAsync();
            using StreamReader reader = new StreamReader(stream);
            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                string json = await reader.ReadLineAsync();
                TResponse chatResponseStream = JsonSerializer.Deserialize<TResponse>(json, IncomingJsonSerializerOptions);
                yield return chatResponseStream != null && chatResponseStream.Done
                    ? JsonSerializer.Deserialize<ChatDoneResponseStream>(json, IncomingJsonSerializerOptions) as TResponse
                    : chatResponseStream;
            }
        }

        protected virtual async Task<HttpResponseMessage> SendToCustomAiAsync(HttpRequestMessage requestMessage, CustomAiRequest? request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            requestMessage.ApplyCustomHeaders(DefaultRequestHeaders, request);
            HttpResponseMessage response = await _client.SendAsync(requestMessage, completionOption, cancellationToken);
            await EnsureSuccessStatusCode(response);
            return response;
        }

        private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                string content = await response.Content.ReadAsStringAsync();
                string errorMessage = ParseErrorMessage(content);
                throw new Exception(errorMessage);
            }

            response.EnsureSuccessStatusCode();
        }

        private static string ParseErrorMessage(string content)
        {
            try
            {
                JsonDocument doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("error", out JsonElement errorElement))
                {
                    return errorElement.GetString() ?? content;
                }
            }
            catch (JsonException)
            {
                // Ignore JSON parsing errors and return raw content
            }

            return content;
        }

        #endregion
    }
}
