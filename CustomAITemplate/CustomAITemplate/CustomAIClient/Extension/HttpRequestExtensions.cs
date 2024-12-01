using System.Net.Http.Headers;

namespace CustomAITemplate.CustomAIClient.Extension
{
    internal static class HttpRequestExtensions
    {
        public static void ApplyCustomHeaders(this HttpRequestMessage requestMessage, Dictionary<string, string> headers, CustomAiRequest? customAiRequest)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                AddOrUpdateHeaderValue(requestMessage.Headers, header.Key, header.Value);
            }

            if (customAiRequest == null)
            {
                return;
            }

            foreach (KeyValuePair<string, string> customHeader in customAiRequest.CustomHeaders)
            {
                AddOrUpdateHeaderValue(requestMessage.Headers, customHeader.Key, customHeader.Value);
            }
        }

        private static void AddOrUpdateHeaderValue(HttpRequestHeaders requestMessageHeaders, string headerKey, string headerValue)
        {
            if (requestMessageHeaders.Contains(headerKey))
            {
                requestMessageHeaders.Remove(headerKey);
            }

            requestMessageHeaders.Add(headerKey, headerValue);
        }
    }
}
