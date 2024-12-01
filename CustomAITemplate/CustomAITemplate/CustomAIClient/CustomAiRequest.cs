using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CustomAITemplate.CustomAIClient
{
    public class CustomAiRequest
    {
        public CustomAiRequest()
        {
            this.CustomHeaders = new Dictionary<string, string>();
        }

        /// <summary>
        /// The name of the model to generate embeddings from
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; } = null!;

        public Dictionary<string, string> CustomHeaders { get; set; }
    }
}
