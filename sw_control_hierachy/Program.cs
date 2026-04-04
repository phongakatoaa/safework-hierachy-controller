using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var ollamaAddress = config["Ollama:Address"] ?? "http://localhost:11434";
var modelFromConfig = config["Ollama:Model"] ?? "";
var serverPort = config["Server:Port"] ?? "5000";

// Load controls and prompt at startup
var basePath = AppContext.BaseDirectory;

var controlsJson = await File.ReadAllTextAsync(Path.Combine(basePath, "controls.json"));
var controls = JsonSerializer.Deserialize<List<ControlDefinition>>(controlsJson)!;

var promptTemplate = await File.ReadAllTextAsync(Path.Combine(basePath, "prompt.md"));
var controlsText = string.Join("\n\n", controls.Select(c => $"{c.Id}. {c.Name}: {c.Description}"));
var promptWithControls = promptTemplate.Replace("{{controls}}", controlsText);

// Setup Ollama
var ollama = new OllamaApiClient(new Uri(ollamaAddress));

if (!string.IsNullOrWhiteSpace(modelFromConfig))
{
    ollama.SelectedModel = modelFromConfig;
}
else
{
    var models = (await ollama.ListLocalModelsAsync()).ToList();
    if (models.Count == 0)
    {
        Console.Error.WriteLine("No local Ollama models found. Set Ollama:Model in appsettings.json or pull a model first.");
        return;
    }
    ollama.SelectedModel = models[0].Name;
}

Console.WriteLine($"Ollama : {ollamaAddress}");
Console.WriteLine($"Model  : {ollama.SelectedModel}");
Console.WriteLine($"Port   : {serverPort}");

builder.WebHost.UseUrls($"http://0.0.0.0:{serverPort}");
var app = builder.Build();

app.MapPost("/assess", async (HazardRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.HazardTitle))
        return Results.BadRequest(new { error = "hazard_title is required." });
    if (string.IsNullOrWhiteSpace(request.Description))
        return Results.BadRequest(new { error = "description is required." });

    var validLevels = new[] { "LOW", "MEDIUM", "HIGH" };
    if (!validLevels.Contains(request.RiskLevel?.ToUpperInvariant()))
        return Results.BadRequest(new { error = "risk_level must be LOW, MEDIUM, or HIGH." });

    var fullPrompt = promptWithControls
        .Replace("{{hazard_title}}", request.HazardTitle)
        .Replace("{{hazard_description}}", request.Description)
        .Replace("{{risk_level}}", request.RiskLevel!.ToUpperInvariant())
        .Replace("{{is_near_miss}}", request.IsNearMiss ? "Yes" : "No");

    try
    {
        var chat = new Chat(ollama);
        var sb = new StringBuilder();
        await foreach (var token in chat.SendAsync(fullPrompt))
            sb.Append(token);

        var raw = sb.ToString().Trim();
        var jsonText = ExtractJson(raw);
        var parsed = JsonSerializer.Deserialize<JsonElement>(jsonText);
        return Results.Ok(parsed);
    }
    catch (JsonException ex)
    {
        return Results.Problem(
            detail: $"Model returned a non-JSON response: {ex.Message}",
            statusCode: 502);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: $"Error communicating with Ollama: {ex.Message}",
            statusCode: 503);
    }
});

app.Run();

static string ExtractJson(string text)
{
    var start = text.IndexOf('{');
    var end = text.LastIndexOf('}');
    return start >= 0 && end > start ? text[start..(end + 1)] : text;
}

record ControlDefinition(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description
);

record HazardRequest(
    [property: JsonPropertyName("hazard_title")] string HazardTitle,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("risk_level")] string? RiskLevel,
    [property: JsonPropertyName("is_near_miss")] bool IsNearMiss
);