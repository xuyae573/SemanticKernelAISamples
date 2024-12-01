using System.Text.Json.Serialization;

namespace CustomAITemplate.CustomAIClient.Models.Chat
{
    //adjust it based on the api response 
    public class ChatResponseStream
    {
        private DateTimeOffset? _createdAt = null!;
        private string? _createdAtString = null!;

        /// <summary>
        /// Gets or sets the model that generated the response.
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; } = null!;

        /// <summary>
        /// Gets or sets the time the response was generated. 
        /// </summary>
        [JsonPropertyName("created_at")]
        public string? CreatedAtString
        {
            get => _createdAtString;
            set
            {
                _createdAtString = value;
                _createdAt = DateTimeOffset.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var createdAt) ? createdAt : null;
            }
        }

        /// <summary>
        /// Gets or sets the time the response was generated.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? CreatedAt
        {
            get => _createdAt;
            set
            {
                _createdAt = value;
                _createdAtString = value?.ToString("o");
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the response is complete.
        /// </summary>
        [JsonPropertyName("done")]
        public bool Done { get; set; }
    }
}
