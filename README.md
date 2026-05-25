# Safework Hierarchy Controller

An HTTP API service that uses a locally-hosted Ollama LLM to assess workplace hazards and recommend corrective controls based on the OSHA Hierarchy of Controls framework. Hazard data is fetched from the Safework SQL Server database (CALog table) and assessed by the LLM.

---

## Overview

Given a CALog entry ID, the service:

1. Fetches the hazard record (description, activity, near miss flag) and its associated risk level from the Safework database
2. Loads all available controls from the CAControl table
3. Builds a system prompt embedding the controls and hazard details
4. Sends the prompt to a locally-running Ollama model
5. Returns the CALog data alongside a structured JSON recommendation identifying the best control and a practical reason

---

## Dependencies

### Docker (recommended)

| Dependency | Version |
|------------|---------|
| [Docker](https://docs.docker.com/get-docker/) + Compose | Latest |

### Local development

| Dependency | Version | Purpose |
|------------|---------|---------|
| [.NET](https://dotnet.microsoft.com/download) | 10.0 | Application runtime |
| [Ollama](https://ollama.com) | Latest | Local LLM inference server |
| [llama3.1](https://ollama.com/library/llama3.1) | Latest | Default language model |

### NuGet Packages

| Package | Version |
|---------|---------|
| [OllamaSharp](https://github.com/awaescher/OllamaSharp) | 5.4.25 |
| [Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer) | 10.0.8 |

---

## Setup

### Option A — Docker (recommended)

```bash
git clone https://github.com/phongakatoaa/safework-hierachy-controller.git
cd safework-hierachy-controller
docker compose up
```

On first run, Docker will pull the Ollama image and download the model (may take a few minutes depending on connection speed). Subsequent starts are instant as the model is cached in the `ollama_data` volume.

To use a different model, set `OLLAMA_MODEL` before starting:

```bash
OLLAMA_MODEL=llama3.2 docker compose up
```

Or edit `.env` in the repo root:

```
OLLAMA_MODEL=llama3.2
```

Once running, the API is available at `http://localhost:5000` and the API docs at `http://localhost:5000/docs`.

---

### Option B — Local development

#### 1. Install Ollama and pull the model

```bash
ollama pull llama3.1
```

#### 2. Clone the repository

```bash
git clone https://github.com/phongakatoaa/safework-hierachy-controller.git
cd safework-hierachy-controller
```

#### 3. Configure `appsettings.json`

Edit `sw_control_hierachy/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Safework": "<your SQL Server connection string>"
  },
  "Ollama": {
    "Address": "http://localhost:11434",
    "Model": "llama3.1"
  },
  "Server": {
    "Port": "5000"
  }
}
```

- **ConnectionStrings:Safework** — Connection string to the Safework SQL Server database containing CALog, Activity, CARisk, and CAControl tables.
- **Model** — Leave empty (`""`) to auto-select the first available local model at startup.

#### 4. Run the server

```bash
cd sw_control_hierachy
dotnet run
```

Expected output:

```
Ollama : http://localhost:11434
Model  : llama3.1
Port   : 5000
info: Now listening on: http://0.0.0.0:5000
```

---

## API Reference

### `POST /assess/calog/{id}`

Fetches a CALog entry from the database, sends it to the Ollama LLM for assessment, and returns both the hazard data and the recommended control.

#### Parameters

| Parameter | In | Type | Required | Description |
|-----------|----|------|----------|-------------|
| `id` | path | integer | Yes | The CALog ID to assess |

#### Response Body

```json
{
  "calog": {
    "calogId": 1234,
    "description": "Loose guardrail on mezzanine level",
    "isNearMiss": true,
    "activity": {
      "activityId": 5,
      "activityName": "Scaffolding"
    },
    "risk": {
      "cariskId": 3,
      "risk": "High",
      "colorCode": "#FF0000"
    },
    "currentControl": {
      "cacontrolId": 6,
      "control": "Personal Protective Equipment",
      "description": "Use of PPE to reduce exposure to hazards."
    }
  },
  "recommendation": {
    "control_id": 3,
    "control_name": "Engineering Controls",
    "reason": "The loose guardrail should be re-secured or replaced with a fixed barrier to eliminate the fall hazard at the mezzanine level."
  }
}
```

#### Error Responses

| Status | Meaning |
|--------|---------|
| `404` | CALog entry not found |
| `502` | Ollama returned a non-JSON response |
| `503` | Could not reach the Ollama server |

---

## Example Request

```bash
curl -X POST http://localhost:5000/assess/calog/1234
```

---

## Project Structure

```
safework-hierachy-controller/
├── docker-compose.yml              # Boots Ollama + model pull + app
├── .env                            # Set OLLAMA_MODEL here (gitignored)
└── sw_control_hierachy/
    ├── Dockerfile                  # Multi-stage build (SDK → ASP.NET runtime)
    ├── Program.cs                  # ASP.NET Core minimal API
    ├── appsettings.json            # Connection string, Ollama address, model, port
    ├── controls.json               # Hierarchy of Controls definitions (IDs 1–7)
    ├── prompt.md                   # LLM system prompt template
    ├── Data/
    │   └── SafeworkDbContext.cs     # EF Core DbContext for Safework database
    ├── Models/
    │   ├── Activity.cs             # Activity entity (hazard activity type)
    │   ├── CALog.cs                # CALog entity (corrective action log)
    │   ├── CARisk.cs               # CARisk entity (risk levels)
    │   └── CAControl.cs            # CAControl entity (hierarchy of controls)
    ├── Docs/
    │   ├── api.html                # Swagger UI (served at /docs)
    │   └── openapi.json            # OpenAPI 3.0 spec
    ├── Samples/
    │   ├── sample.request.json     # Example test cases
    │   └── sample.response.json    # Captured responses
    └── Context/                    # Reference material for Claude only (not served)
        ├── ollama.md
        └── Hierarchy_of_Controls_02.01.23_form_508_2.pdf
```
