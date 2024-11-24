
# Custom AI Client for Multiple AI Models

This repository contains a custom AI client template designed to work with various AI models. Built on a highly extensible architecture, this client supports integration with Semantic Kernel (SK) to create a `CustomChatCompletionService`, allowing interaction with AI models while maintaining a history of conversations.

## Features

- **Generic AI Client Template**: Supports embedding, chat completion, and streaming responses.
- **Extensible**: Easily integrate with any AI model by configuring the endpoint and model settings.
- **Semantic Kernel Integration**: Implements `CustomChatCompletionService` to leverage SK capabilities for orchestrating AI workflows and maintaining conversation history.
- **Streamed Responses**: Handle streamed responses efficiently for real-time applications.
- **Template for New Models**: Serve as a base for creating clients for new AI models with minimal effort.

## Technologies Used

- **C#**: Main programming language.
- **Semantic Kernel (SK)**: For orchestration and managing conversation histories.
- **.NET HTTP Client**: For making RESTful API calls to AI endpoints.
- **System.Text.Json**: For JSON serialization and deserialization.

---

## Table of Contents

1. [Installation](#installation)
2. [Usage](#usage)
3. [Architecture Overview](#architecture-overview)
4. [Methods in the AI Client](#methods-in-the-ai-client)
5. [Extending the Template](#extending-the-template)
6. [Examples](#examples)
7. [Contributing](#contributing)
8. [License](#license)

---

## Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/xuyae573/SemanticKernelAISamples
   cd SemanticKernelAISamples
   ```

2. Restore dependencies:

   ```bash
   dotnet restore
   ```

3. Build the project:

   ```bash
   dotnet build
   ```

---

## Usage

### Setting up the AI Client

The AI client requires configuration for the endpoint URI and the model you want to use.

```csharp
var client = new CustomAiApiClient("https://your-ai-endpoint.com", "default-model");
```

### Example: Making a Chat Request

```csharp
var chatRequest = new ChatRequest
{
    Prompt = "Explain the concept of Semantic Kernel.",
    MaxTokens = 100,
    Stream = false // Set to true for streaming response
};

var response = await client.GetChatMessageContentsAsync(chatRequest);
Console.WriteLine(response.Text);
```

### Example: Streaming Chat Responses

```csharp
await foreach (var responseStream in client.GetStreamingChatMessageContentsAsync(chatRequest))
{
    if (responseStream != null)
    {
        Console.WriteLine(responseStream.Text);
    }
}
```

---

## Architecture Overview

### Key Components

1. **`CustomAiApiClient`**: A generic HTTP client designed to communicate with different AI models. It handles API requests and responses, including streamed responses.
2. **Configuration**: Centralized configuration for managing API URIs and model selection.
3. **Serializer Options**: Separate serializer options for outgoing and incoming JSON payloads.
4. **Semantic Kernel Integration**: Designed to work with Semantic Kernel, enabling workflow orchestration and conversation history tracking.

---

## Methods in the AI Client

| Method                                         | Description                                                                                       |
|-----------------------------------------------|---------------------------------------------------------------------------------------------------|
| `Embed(EmbedRequest)`                         | Sends an embedding request to the AI endpoint.                                                   |
| `GetVersion()`                                | Fetches the version of the connected AI model or service.                                         |
| `GetChatMessageContentsAsync(ChatRequest)`    | Sends a chat request and retrieves the full response.                                             |
| `GetStreamingChatMessageContentsAsync(ChatRequest)` | Streams the response from the AI model for real-time applications.                               |
| `PostAsync<TRequest, TResponse>()`            | Generic method for sending POST requests.                                                        |
| `StreamPostAsync<TRequest, TResponse>()`      | Sends POST requests and processes the response as a stream.                                       |

---

## Extending the Template

### Adding a New AI Model

1. **Configuration**: Update the `Configuration` class to include the new model's URI and any additional settings.
2. **Custom Request/Response**: Define custom request and response classes in the `Models` folder.
3. **Client Methods**: Add new methods in `CustomAiApiClient` for specific capabilities of the new model.

### Customizing the Serializer

Update the `OutgoingJsonSerializerOptions` or `IncomingJsonSerializerOptions` as required for specific AI model APIs.

---

## Examples

### Using Semantic Kernel Integration

The `CustomChatCompletionService` can be implemented to wrap the AI client for conversation orchestration.

```csharp
public class CustomChatCompletionService
{
    private readonly CustomAiApiClient _aiClient;

    public CustomChatCompletionService(CustomAiApiClient aiClient)
    {
        _aiClient = aiClient;
    }

    public async Task<string> GenerateResponseAsync(string prompt)
    {
        var request = new ChatRequest
        {
            Prompt = prompt,
            MaxTokens = 150
        };

        var response = await _aiClient.GetChatMessageContentsAsync(request);
        return response.Text;
    }
}
```

---

## Contributing

1. Fork the repository.
2. Create a new feature branch.
3. Submit a pull request with a detailed explanation of the changes.

---

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
