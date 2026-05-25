using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using OllamaSharp;
using sw_control_hierachy.Data;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var ollamaAddress = config["Ollama:Address"] ?? "http://localhost:11434";
var modelFromConfig = config["Ollama:Model"] ?? "";
var serverPort = config["Server:Port"] ?? "5000";

// Load controls and prompt at startup
var basePath = AppContext.BaseDirectory;

var promptTemplate = await File.ReadAllTextAsync(Path.Combine(basePath, "prompt.md"));

// Setup Ollama
var httpClient = new HttpClient { BaseAddress = new Uri(ollamaAddress), Timeout = TimeSpan.FromMinutes(10) };
var ollama = new OllamaApiClient(httpClient);

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

builder.Services.AddDbContext<SafeworkDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("Safework")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.WebHost.UseUrls($"http://0.0.0.0:{serverPort}");
var app = builder.Build();

app.UseCors();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "Docs")),
    RequestPath = "/docs"
});

app.MapGet("/docs", () => Results.Redirect("/docs/api.html"));

app.MapPost("/assess/calog/{id:int}", async (int id, SafeworkDbContext db) =>
{
    var calog = await db.Calogs.FirstOrDefaultAsync(c => c.CalogId == id);
    if (calog is null)
        return Results.NotFound(new { error = $"CALog {id} not found." });

    var risk = await db.Carisks.FirstOrDefaultAsync(r => r.CariskId == calog.CariskId);
    var activity = await db.Activities.FirstOrDefaultAsync(a => a.ActivityId == calog.ActivityId);
    var currentControl = calog.CacontrolId.HasValue
        ? await db.Cacontrols.FirstOrDefaultAsync(c => c.CacontrolId == calog.CacontrolId.Value)
        : null;

    // Build controls list from database
    var dbControls = await db.Cacontrols.Where(c => c.Enabled).OrderBy(c => c.OrderId).ToListAsync();
    var dbControlsText = string.Join("\n\n", dbControls.Select(c => $"{c.CacontrolId}. {c.Control}: {c.Description}"));

    var riskLevel = risk?.Risk?.ToUpperInvariant() ?? "MEDIUM";

    var fullPrompt = promptTemplate
        .Replace("{{controls}}", dbControlsText)
        .Replace("{{activity}}", activity?.ActivityName ?? "Unknown")
        .Replace("{{hazard_description}}", calog.Description ?? "No description")
        .Replace("{{risk_level}}", riskLevel)
        .Replace("{{is_near_miss}}", calog.IsNearMiss == true ? "Yes" : "No");

    app.Logger.LogDebug("Full prompt for CALog {CalogId}:\n{FullPrompt}", id, fullPrompt);

    try
    {
        var chat = new Chat(ollama);
        var sb = new StringBuilder();
        await foreach (var token in chat.SendAsync(fullPrompt))
            sb.Append(token);

        var raw = sb.ToString().Trim();
        var jsonText = ExtractJson(raw);
        var recommendation = JsonSerializer.Deserialize<JsonElement>(jsonText);

        return Results.Ok(new
        {
            calog = new
            {
                calog.CalogId,
                calog.Description,
                calog.IsNearMiss,
                Activity = activity is null ? null : new
                {
                    activity.ActivityId,
                    activity.ActivityName
                },
                Risk = risk is null ? null : new
                {
                    risk.CariskId,
                    risk.Risk,
                    risk.ColorCode
                },
                CurrentControl = currentControl is null ? null : new
                {
                    currentControl.CacontrolId,
                    currentControl.Control,
                    currentControl.Description
                }
            },
            recommendation
        });
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

