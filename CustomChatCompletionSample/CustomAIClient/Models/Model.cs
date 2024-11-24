using System.Text.Json.Serialization;

namespace CustomChatCompletionSample.CustomAIClient.Models
{
    public class Model
    {
        /// <summary>
        /// Gets or sets the name of the model.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the time the model was created or last modified.
        /// </summary>
        [JsonPropertyName("modified_at")]
        public DateTime ModifiedAt { get; set; }

        /// <summary>
        /// Gets or sets the size of the model file in bytes.
        /// </summary>
        [JsonPropertyName("size")]
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets a cryptographic hash of the model file.
        /// </summary>
        [JsonPropertyName("digest")]
        public string Digest { get; set; } = null!;

     
    }
}
