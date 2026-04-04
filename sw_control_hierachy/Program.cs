using Microsoft.Extensions.Configuration;
using OllamaSharp;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var address = config["Ollama:Address"] ?? "http://localhost:11434";
var modelFromConfig = config["Ollama:Model"] ?? "";

var uri = new Uri(address);
var ollama = new OllamaApiClient(uri);

string selectedModel;

if (!string.IsNullOrWhiteSpace(modelFromConfig))
{
    selectedModel = modelFromConfig;
}
else
{
    var models = (await ollama.ListLocalModelsAsync()).ToList();
    if (models.Count == 0)
    {
        Console.WriteLine("No local Ollama models found. Please pull a model first (e.g. `ollama pull llama3.2`).");
        return;
    }

    Console.WriteLine("Available models:");
    for (int i = 0; i < models.Count; i++)
        Console.WriteLine($"  [{i + 1}] {models[i].Name}");

    Console.Write($"\nSelect model (Enter for default '{models[0].Name}'): ");
    var input = Console.ReadLine()?.Trim();

    selectedModel = models[0].Name;
    if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int choice) && choice >= 1 && choice <= models.Count)
        selectedModel = models[choice - 1].Name;
}

ollama.SelectedModel = selectedModel;
Console.WriteLine($"\nConnected to {address} — model: {selectedModel}");
Console.WriteLine("Type your message and press Enter. Press Ctrl+C to exit.\n");

var chat = new Chat(ollama);

while (true)
{
    Console.Write("You: ");
    var message = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(message)) continue;

    Console.Write("Assistant: ");
    await foreach (var token in chat.SendAsync(message))
        Console.Write(token);
    Console.WriteLine();
}