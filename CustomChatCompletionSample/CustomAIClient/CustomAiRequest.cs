using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomChatCompletionSample.CustomAIClient
{
    public class CustomAiRequest
    {
        public CustomAiRequest()
        {
            this.CustomHeaders = new Dictionary<string, string>();
        }

        public Dictionary<string, string> CustomHeaders { get; set; }
    }
}
