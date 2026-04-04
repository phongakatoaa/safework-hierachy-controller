Safework Hierachy Controller

## Guidelines

When building ollama API code, refer to [ollama.md](docs/ollama.md).

## Project Structure

- `Program.cs` — ASP.NET Core minimal API. POST /assess endpoint accepts a hazard and returns a recommended control as JSON.
- `appsettings.json` — Ollama address, model name, and server port configuration.
- `controls.json` — Hierarchy of Controls definitions (IDs 1–7). Loaded at startup and embedded into the prompt.
- `prompt.md` — System prompt template with `{{controls}}`, `{{hazard_title}}`, `{{hazard_description}}`, `{{risk_level}}`, `{{is_near_miss}}` placeholders.
- `samples/` — Sample request and response JSON files for testing.
- `docs/ollama.md` — OllamaSharp code snippets reference.