# Safework Hierarchy Controller

An HTTP API service that uses a locally-hosted Ollama LLM to assess workplace hazards and recommend corrective controls based on the OSHA Hierarchy of Controls framework. Designed for construction site safety workflows.

---

## Overview

When a hazard is reported (title, description, risk level, and whether it is a near miss), the service:

1. Loads a system prompt that embeds the full Hierarchy of Controls definition
2. Sends the hazard details to a locally-running Ollama model
3. Returns a structured JSON response identifying the recommended control, its ID, and a practical reason tailored to the specific hazard

The seven controls — from most to least effective — are:

| ID | Control |
|----|---------|
| 1 | Elimination |
| 2 | Substitution |
| 3 | Engineering Controls |
| 4 | Warnings |
| 5 | Administrative Controls |
| 6 | Personal Protective Equipment |
| 7 | Auto-dispositioned |

---

## Dependencies

### Runtime

| Dependency | Version | Purpose |
|------------|---------|---------|
| [.NET](https://dotnet.microsoft.com/download) | 10.0 | Application runtime |
| [Ollama](https://ollama.com) | Latest | Local LLM inference server |
| [llama3.1](https://ollama.com/library/llama3.1) | Latest | Default language model |

### NuGet Packages

| Package | Version |
|---------|---------|
| [OllamaSharp](https://github.com/awaescher/OllamaSharp) | 5.4.25 |

---

## Setup

### 1. Install Ollama

Download and install Ollama from [https://ollama.com](https://ollama.com), then pull the default model:

```bash
ollama pull llama3.1
```

Verify Ollama is running:

```bash
ollama list
```

### 2. Clone the repository

```bash
git clone https://github.com/phongakatoaa/safework-hierachy-controller.git
cd safework-hierachy-controller
```

### 3. Configure `appsettings.json`

Edit `sw_control_hierachy/appsettings.json`:

```json
{
  "Ollama": {
    "Address": "http://localhost:11434",
    "Model": "llama3.1"
  },
  "Server": {
    "Port": "5000"
  }
}
```

- **Address** — URL of your Ollama server (default: `http://localhost:11434`)
- **Model** — Name of the Ollama model to use. Leave empty (`""`) to be prompted to select from available local models at startup.
- **Port** — Port the HTTP server listens on (default: `5000`)

### 4. Run the server

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

### `POST /assess`

Assesses a reported hazard and returns the recommended Hierarchy of Controls action.

#### Request Body

```json
{
  "hazard_title": "string",
  "description": "string",
  "risk_level": "LOW | MEDIUM | HIGH",
  "is_near_miss": true | false
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `hazard_title` | string | Yes | Short title of the hazard |
| `description` | string | Yes | Detailed description of the hazard and circumstances |
| `risk_level` | string | Yes | Severity: `LOW`, `MEDIUM`, or `HIGH` |
| `is_near_miss` | boolean | Yes | `true` if no injury/damage occurred but had the potential to |

#### Response Body

```json
{
  "control_id": 3,
  "control_name": "Engineering Controls",
  "reason": "Practical explanation tailored to the specific hazard..."
}
```

#### Error Responses

| Status | Meaning |
|--------|---------|
| `400` | Missing or invalid request fields |
| `502` | Ollama returned a non-JSON response |
| `503` | Could not reach the Ollama server |

---

## Example Requests

### High-risk near miss — fall from height

```bash
curl -X POST http://localhost:5000/assess \
  -H "Content-Type: application/json" \
  -d '{
    "hazard_title": "Unsecured scaffolding plank at height",
    "description": "A scaffolding plank on the 4th floor (approx. 12m) was unsecured at one end and could shift underfoot. A colleague nearly stepped on the unsecured end.",
    "risk_level": "HIGH",
    "is_near_miss": true
  }'
```

```json
{
  "control_id": 1,
  "control_name": "Elimination",
  "reason": "The scaffolding plank should be redesigned or replaced to ensure secure anchoring at all ends. Immediate action is necessary to prevent potential falls from heights."
}
```

---

### High-risk chemical hazard

```bash
curl -X POST http://localhost:5000/assess \
  -H "Content-Type: application/json" \
  -d '{
    "hazard_title": "Solvent-based adhesive used in enclosed space",
    "description": "Workers are applying solvent-based contact adhesive in a poorly ventilated basement. Strong fumes are present and workers are reporting headaches. The chemical has high VOC content and is flammable.",
    "risk_level": "HIGH",
    "is_near_miss": false
  }'
```

```json
{
  "control_id": 2,
  "control_name": "Substitution",
  "reason": "Substituting the solvent-based adhesive with a low-VOC or water-based alternative will significantly reduce worker exposure to hazardous fumes."
}
```

---

### Medium-risk noise exposure

```bash
curl -X POST http://localhost:5000/assess \
  -H "Content-Type: application/json" \
  -d '{
    "hazard_title": "Excessive noise from concrete cutting",
    "description": "Concrete cutting with a petrol-powered disc cutter is performed daily. Noise near the operator reaches 105 dB(A). Adjacent workers are also exposed.",
    "risk_level": "MEDIUM",
    "is_near_miss": false
  }'
```

```json
{
  "control_id": 3,
  "control_name": "Engineering Controls",
  "reason": "A sound enclosure or machine guard should be installed on the disc cutter to reduce noise emission to safe levels for both the operator and adjacent workers."
}
```

---

### Low-risk PPE non-compliance

```bash
curl -X POST http://localhost:5000/assess \
  -H "Content-Type: application/json" \
  -d '{
    "hazard_title": "Workers not wearing high-visibility vests in traffic zone",
    "description": "Two subcontractor workers were observed near an active vehicle access road without high-visibility vests. The area has designated PPE requirements.",
    "risk_level": "LOW",
    "is_near_miss": false
  }'
```

```json
{
  "control_id": 6,
  "control_name": "Personal Protective Equipment",
  "reason": "Workers must comply with the designated PPE requirements. High-visibility vests must be worn at all times in the traffic zone to ensure visibility to vehicle operators."
}
```

---

## Project Structure

```
sw_control_hierachy/
├── Program.cs              # ASP.NET Core minimal API server
├── appsettings.json        # Ollama connection and server config
├── controls.json           # Hierarchy of Controls definitions
├── prompt.md               # System prompt template for the LLM
├── sample.request.json     # Example test cases
└── sample.response.json    # Captured responses for the test cases
```
