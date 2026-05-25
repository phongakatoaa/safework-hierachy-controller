# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

See the root `CLAUDE.md` for full architecture and command reference.

## OllamaSharp

When building Ollama API code, refer to [Context/ollama.md](Context/ollama.md) for current OllamaSharp code patterns.

The `Context/` folder contains reference material for Claude only — it is not served by the HTTP server:
- `Context/ollama.md` — OllamaSharp code snippets
- `Context/Hierarchy_of_Controls_02.01.23_form_508_2.pdf` — OSHA Hierarchy of Controls source document

## Key files

- `controls.json` — edit this to change control definitions; IDs must remain integers 1–7 and match what `prompt.md` references.
- `prompt.md` — edit this to tune LLM behaviour. Placeholders: `{{controls}}`, `{{current_status}}`, `{{hazard_description}}`, `{{risk_level}}`, `{{is_near_miss}}`.
- `Samples/` — update `sample.request.json` and re-run the Node test script when adding new test cases.