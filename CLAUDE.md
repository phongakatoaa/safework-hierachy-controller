# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Docker — start everything (Ollama + model pull + app)
docker compose up

# Docker — use a different model
OLLAMA_MODEL=llama3.2 docker compose up

# Docker — rebuild the app image after code changes
docker compose up --build
```

Local development (from `sw_control_hierachy/`):

```bash
dotnet build
dotnet run
dotnet restore
```

There are no automated tests. Manual API testing uses the samples in `sw_control_hierachy/samples/`.

## Architecture

This is a single-file ASP.NET Core minimal API (`Program.cs`) that acts as a bridge between a caller reporting a workplace hazard and a locally-running Ollama LLM. The LLM's job is to recommend the correct level of the OSHA Hierarchy of Controls.

### Request flow

```
POST /assess
  → validate request fields
  → fill prompt.md placeholders ({{hazard_title}}, {{hazard_description}}, {{risk_level}}, {{is_near_miss}}, {{controls}})
  → send full prompt to Ollama via OllamaSharp Chat.SendAsync (streamed, collected into string)
  → extract JSON object from raw response (strips markdown fences if present)
  → return parsed JSON to caller
```

### Startup sequence

On startup, `Program.cs` reads `controls.json` and `prompt.md` from `AppContext.BaseDirectory` (the build output directory — files are copied there via `.csproj`). The controls list is embedded into the prompt template once at startup, not per request. The Ollama model is taken from `appsettings.json`; if blank, the first model returned by `ListLocalModelsAsync()` is used.

### Prompt design

`prompt.md` is the full system prompt. It contains all assessment guidelines and the `{{controls}}` placeholder, which is replaced at startup with the numbered list from `controls.json`. Per-request placeholders (`{{hazard_title}}` etc.) are replaced at request time. The LLM is instructed to return only a bare JSON object — no markdown fences. `ExtractJson()` in `Program.cs` handles cases where the model wraps the JSON in code fences anyway, by finding the first `{` and last `}`.

### Configuration (`appsettings.json`)

| Key | Default | Purpose |
|-----|---------|---------|
| `Ollama:Address` | `http://localhost:11434` | Ollama server URL |
| `Ollama:Model` | `llama3.1` | Model name; blank = auto-select |
| `Server:Port` | `5000` | HTTP listen port |

### OllamaSharp usage

For OllamaSharp API patterns, refer to `sw_control_hierachy/context/ollama.md`. A new `Chat` instance is created per request (stateless — no conversation history is retained between API calls).

### Docker setup

`docker-compose.yml` at the repo root defines three services:
- `ollama` — runs the Ollama inference server; health-checked before dependents start
- `ollama-pull` — one-shot service that runs `ollama pull $OLLAMA_MODEL` against the ollama service, then exits; uses the shared `ollama_data` volume so the model is only downloaded once
- `app` — the .NET app; starts only after `ollama-pull` completes successfully

Config is injected via environment variables (`Ollama__Address`, `Ollama__Model`, `Server__Port`), which ASP.NET Core resolves over `appsettings.json`. The model is controlled by `OLLAMA_MODEL` in `.env` (gitignored, default `llama3.1`).

### Context folder

`sw_control_hierachy/context/` contains reference material for Claude only — not served by the HTTP server, not copied to build output, and excluded from the Docker image via `.dockerignore`:
- `ollama.md` — OllamaSharp code snippets
- `Hierarchy_of_Controls_02.01.23_form_508_2.pdf` — OSHA source document for control definitions
