using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CustomAITemplate.CustomAIClient.Models
{
    public class EmbedRequest : CustomAiRequest
    {
        /// <summary>
        /// The text to generate embeddings for
        /// </summary>
        [JsonPropertyName("input")]
        public List<string> Input { get; set; } = null!;

        /// <summary>
        /// Additional model parameters listed in the documentation for the Modelfile
        /// such as temperature.
        /// </summary>
        [JsonPropertyName("options")]
        public CustomChatOptions? Options { get; set; }

        /// <summary>
        /// Gets or sets the KeepAlive property, which decides how long a given
        /// model should stay loaded.
        /// </summary>
        [JsonPropertyName("keep_alive")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? KeepAlive { get; set; }

        /// <summary>
        /// Truncates the end of each input to fit within context length.
        /// Returns error if false and context length is exceeded. Defaults to true
        /// </summary>
        [JsonPropertyName("truncate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Truncate { get; set; }
    }
}
