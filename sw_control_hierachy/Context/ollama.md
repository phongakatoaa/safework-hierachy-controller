### List All Local Ollama Models

Source: https://github.com/awaescher/ollamasharp/blob/main/README.md

This example shows how to asynchronously retrieve a collection of all Ollama models that are currently available on the local system. It's a straightforward way to inspect your installed models.

```C#
var models = await ollama.ListLocalModelsAsync();
```

--------------------------------

### Interact with Ollama using OllamaSharp in C#

Source: https://github.com/awaescher/ollamasharp/blob/main/index.md

This C# example demonstrates how to initialize an OllamaApiClient, create a chat session, and continuously interact with the Ollama model. It shows how to send user messages and stream assistant responses in a console application.

```csharp
using OllamaSharp;

var uri = new Uri("http://localhost:11434");
var ollama = new OllamaApiClient(uri, "llama3.2");

// messages including their roles and tool calls will automatically
// be tracked within the chat object and are accessible via the Messages property
var chat = new Chat(ollama);
   
Console.WriteLine("You're now talking with Ollama. Hit Ctrl+C to exit.");

while (true)
{
    Console.Write("You: ");
    var message = Console.ReadLine();

    Console.Write("Assistant: ");
    await foreach (var stream in chat.SendAsync(message))
        Console.Write(stream);

    Console.WriteLine("");
}
```

--------------------------------

### List Local Ollama Models Asynchronously

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/getting-started.md

This example illustrates how to asynchronously retrieve a collection of all models that are currently available on your local Ollama server. The `ListLocalModelsAsync()` method provides a convenient way to inspect your installed models.

```C#
var models = await ollama.ListLocalModelsAsync();
```

--------------------------------

### Build Interactive Ollama Chat Sessions

Source: https://github.com/awaescher/ollamasharp/blob/main/README.md

This example shows how to create an interactive chat session with an Ollama model. The `Chat` object automatically tracks messages, roles, and tool calls, making it easy to build conversational AI applications. User input is read from the console, and model responses are streamed back.

```C#
// messages including their roles and tool calls will automatically be tracked within the chat object
// and are accessible via the Messages property

var chat = new Chat(ollama);

while (true)
{
    var message = Console.ReadLine();
    await foreach (var answerToken in chat.SendAsync(message))
        Console.Write(answerToken);
}
```

--------------------------------

### Build Interactive Ollama Chat Session

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/getting-started.md

This example illustrates how to create an interactive chat experience with Ollama using the `Chat` class. It continuously reads user input from the console, sends it to Ollama via `chat.SendAsync()`, and streams the AI's responses back to the console. The `Chat` object automatically manages message history and roles.

```C#
var chat = new Chat(ollama);

while (true)
{
    var message = Console.ReadLine();
    await foreach (var answerToken in chat.SendAsync(message))
        Console.Write(answerToken);
}
```

--------------------------------

### Ollama Tool Definition JSON Example

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/tool-support.md

This JSON snippet illustrates how a function tool, such as 'get_current_weather', is defined for an Ollama AI model. It specifies the tool's type, name, description, and the schema for its input parameters, including their types and whether they are required.

```json
[
    {
      "type": "function",
      "function": {
        "name": "get_current_weather",
        "description": "Get the current weather for a city",
        "parameters": {
          "type": "object",
          "properties": {
            "city": {
              "type": "string",
              "description": "The name of the city"
            }
          },
          "required": ["city"]
        }
      }
    }
  ]
```

--------------------------------

### Initialize OllamaApiClient and Select Model

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/getting-started.md

This snippet demonstrates how to set up and initialize an instance of the `OllamaApiClient` by providing the Ollama server's URI. It also shows how to select a default model that will be used for subsequent operations, streamlining interactions with the Ollama API.

```C#
var uri = new Uri("http://localhost:11434");
var ollama = new OllamaApiClient(uri);

// select a model which should be used for further operations
olama.SelectedModel = "llama3.1:8b";
```

--------------------------------

### Initialize OllamaApiClient

Source: https://github.com/awaescher/ollamasharp/blob/main/README.md

This snippet demonstrates how to set up the Ollama API client. It involves specifying the Ollama server's URI and then selecting a default model that will be used for subsequent operations, streamlining interactions.

```C#
// set up the client
var uri = new Uri("http://localhost:11434");
var ollama = new OllamaApiClient(uri);

// select a model which should be used for further operations
ollama.SelectedModel = "llama3.1:8b";
```

--------------------------------

### Create IChatClient for Ollama or OpenAI with Microsoft.Extensions.AI

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/getting-started.md

This snippet demonstrates how to abstract AI provider selection using Microsoft.Extensions.AI's `IChatClient` interface. It shows a factory method that returns either an `OllamaApiClient` or an `OpenAIChatClient` based on a provided argument, enabling flexible integration with different AI services.

```C#
private static IChatClient CreateChatClient(Arguments arguments)
{
  if (arguments.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
    return new OllamaApiClient(arguments.Uri, arguments.Model);
  else
    return new OpenAIChatClient(new OpenAI.OpenAIClient(arguments.ApiKey), arguments.Model); // ChatGPT or compatible
}
```

--------------------------------

### OllamaSharp Model Context Protocol (MCP) Tool Integration

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/tool-support.md

Describes how OllamaSharp supports the Model Context Protocol for defining tools more generically. It specifies the NuGet package required and the `Tools.GetFromMcpServers()` method for adding MCP tools from configuration or code.

```APIDOC
Model Context Protocol (MCP) Integration:
  - NuGet Package: OllamaSharp.ModelContextProtocol
  - Tool Addition Method: Tools.GetFromMcpServers()
    - Supports: Reading MCP servers from configuration files or via code.
  - Purpose: Define tools generically in a separate project for use by multiple models/languages.
```

--------------------------------

### Pull Ollama Model with Progress Reporting

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/getting-started.md

This snippet shows how to asynchronously pull a new model from the Ollama model hub, such as 'llama3.1:405b'. It utilizes `await foreach` to stream progress updates, allowing you to monitor the download status in real-time, including percentage completion and current status messages.

```C#
await foreach (var status in ollama.PullModelAsync("llama3.1:405b"))
    Console.WriteLine($"{status.Percent}% {status.Status}");
```

--------------------------------

### Add OllamaSharp NuGet Package to Project

Source: https://github.com/awaescher/ollamasharp/blob/main/index.md

This command adds the OllamaSharp NuGet package to your .NET project, making the library's functionalities available for use in your application.

```bash
dotnet add package OllamaSharp
```

--------------------------------

### Generate Ollama Completion Directly to Console

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/getting-started.md

This code demonstrates how to send a text prompt to the Ollama server and stream the generated response directly to the console. The `GenerateAsync()` method returns an asynchronous stream of response tokens, which are then written to the console as they arrive.

```C#
await foreach (var stream in ollama.GenerateAsync("How are you today?"))
    Console.Write(stream.Response);
```

--------------------------------

### OllamaApiClient Interface Implementations

Source: https://github.com/awaescher/ollamasharp/blob/main/README.md

This documentation details the various interfaces implemented by the `OllamaApiClient` class. It clarifies that `OllamaApiClient` supports both native Ollama-specific methods via `IOllamaApiClient` and abstract AI functionalities through Microsoft.Extensions.AI's `IChatClient` and `IEmbeddingGenerator<string, Embedding<float>>` interfaces. It also notes the scope limitations of the abstraction interfaces compared to the full native API.

```APIDOC
OllamaApiClient Class Interfaces:
  IOllamaApiClient: Provides native Ollama specific methods.
  IChatClient: Microsoft.Extensions.AI abstraction for chat operations.
  IEmbeddingGenerator<string, Embedding<float>>: Microsoft.Extensions.AI abstraction for embedding generation.

Note: OllamaApiClient implements all three interfaces, allowing casting to any as needed. IChatClient and IEmbeddingGenerator do not cover the full Ollama API specification.
```

--------------------------------

### Integrate OllamaApiClient with Microsoft.Extensions.AI IChatClient

Source: https://github.com/awaescher/ollamasharp/blob/main/README.md

This code demonstrates how to integrate `OllamaApiClient` with Microsoft.Extensions.AI's `IChatClient` abstraction. This allows `OllamaApiClient` to be used interchangeably with other AI providers like OpenAI, promoting a unified approach to AI application development.

```C#
// install package Microsoft.Extensions.AI.Abstractions

private static IChatClient CreateChatClient(Arguments arguments)
{
  if (arguments.Provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
    return new OllamaApiClient(arguments.Uri, arguments.Model);
  else
    return new OpenAIChatClient(new OpenAI.OpenAIClient(arguments.ApiKey), arguments.Model); // ChatGPT or compatible
}
```

--------------------------------

### OllamaSharp Tool Invocation and Chat.ToolInvoker Customization

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/tool-support.md

Explains the automatic invocation of tools by the AI model and the back-propagation of results to the chat. It highlights that the `Chat.ToolInvoker` instance can be modified to customize the entire tool invocation behavior.

```APIDOC
Tool Invocation:
  - Automatic: Tools are invoked automatically when requested by the AI model.
  - Execution: Tool implementation is executed directly from the generated tool.
  - Result Propagation: Tool's result value is automatically sent back to the chat for AI model continuation.
Chat.ToolInvoker:
  - Purpose: Instance to modify or customize tool invocation behavior.
```

--------------------------------

### Define Basic OllamaSharp Tool with [OllamaTool] Attribute

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/tool-support.md

Illustrates the simplest form of defining an AI tool in OllamaSharp using the `[OllamaTool]` attribute on a static method. This method, `GetWeather`, takes a city name and is intended to retrieve weather information, with its metadata automatically generated.

```csharp
public class SampleTools
{
	/// <summary>
	/// Get the current weather for a city
	/// </summary>
	/// <param name="city">Name of the city</param>
	[OllamaTool]
	public static string GetWeather(string city) => ...;
}
```

--------------------------------

### Utilize Generated OllamaSharp Tool in Chat Interaction

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/tool-support.md

Shows how to integrate and use the automatically generated OllamaSharp tool (e.g., `GetWeatherTool`) within a chat conversation. It demonstrates sending a message that triggers the tool and processing the AI's response, highlighting the seamless tool invocation process.

```csharp
var chat = new Chat(...);

await foreach (var answerToken in chat.SendAsync("How's the weather in Stuttgart?", [new GetWeatherTool()]))
    Console.WriteLine(answerToken);
```

--------------------------------

### OllamaSharp Legacy Manual Tool Definition in C#

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/tool-support.md

This C# code demonstrates how tools were manually defined in earlier versions of OllamaSharp. It shows the creation of a `WeatherTool` class inheriting from `Tool`, where the function's metadata (name, description, parameters) is explicitly constructed using nested objects like `Function`, `Parameters`, and `Property`. This approach is verbose and couples the tool definition to the OllamaSharp library.

```csharp
public class WeatherTool : Tool
{
    public WeatherTool()
    {
        Function = new Function
        {
            Description = "Get the current weather for a city",
            Name = "get_current_weather",
            Parameters = new Parameters
            {
                Properties = new Dictionary<string, Property>
                {
                    ["city"] = new() { Type = "string", Description = "Name of the city" }
                },
                Required = ["city"]
            }
        };
        Type = "function";
    }
}
```

--------------------------------

### Pull Ollama Model with Progress Reporting

Source: https://github.com/awaescher/ollamasharp/blob/main/README.md

This code illustrates how to asynchronously pull a specified Ollama model from a remote source. It includes real-time progress reporting, allowing you to monitor the download status, including percentage completion and current status messages.

```C#
await foreach (var status in ollama.PullModelAsync("llama3.1:405b"))
    Console.WriteLine($"{status.Percent}% {status.Status}");
```

--------------------------------

### Generate Ollama Completion to Console

Source: https://github.com/awaescher/ollamasharp/blob/main/README.md

This snippet demonstrates how to generate a text completion from an Ollama model based on a given prompt. It streams the response tokens directly to the console as they are generated, providing a real-time output experience.

```C#
await foreach (var stream in ollama.GenerateAsync("How are you today?"))
    Console.Write(stream.Response);
```

--------------------------------

### [OllamaTool] Attribute Metadata Extraction and Tool Naming Convention

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/tool-support.md

Documents how the `[OllamaTool]` attribute automatically extracts metadata for AI tools from method signatures and summaries. It details the mapping of function name, description, arguments (including types, descriptions, enums, required/optional status), and the naming convention for generated tools (method name + 'Tool' appendix).

```APIDOC
[OllamaTool] Attribute:
  - Function Name: Derived from method name.
  - Function Description: Derived from method summary XML documentation.
  - Arguments:
    - Description: From parameter XML documentation.
    - Return Type: From method return type.
    - Enum: Values parsed from enum type.
    - Required/Optional: Based on parameter definition (e.g., default value for optional).
  - Generated Tool Name: MethodName + "Tool" (e.g., GetWeather() -> GetWeatherTool).
  - Generated Tool Namespace: Same as the defining class.
  - Requirement: Project containing tools must generate a documentation file (<GenerateDocumentationFile>true</GenerateDocumentationFile>).
```

--------------------------------

### Define OllamaSharp Tool with Optional Enum Parameter

Source: https://github.com/awaescher/ollamasharp/blob/main/docs/tool-support.md

Expands on the basic tool definition by adding an optional `Unit` enum parameter to the `GetWeather` method. This demonstrates how to handle fixed-value arguments and provides a simple implementation for the tool, showcasing automatic enum parsing by OllamaSharp.

```csharp
public class SampleTools
{
	/// <summary>
	/// Get the current weather for a city
	/// </summary>
	/// <param name="city">Name of the city</param>
	/// <param name="unit">Temperature unit for the weather</param>
	[OllamaTool]
	public static string GetWeather(string city, Unit unit = Unit.Celsius) => $"It's cold at only 6° {unit} in {city}.";

    public enum Unit
    {
        Celsius,
        Fahrenheit
    }
}
```

=== COMPLETE CONTENT === This response contains all available snippets from this library. No additional content exists. Do not make further requests.