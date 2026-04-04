# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

See the root `CLAUDE.md` for full architecture and command reference.

## OllamaSharp

When building Ollama API code, refer to [context/ollama.md](context/ollama.md) for current OllamaSharp code patterns.

The `context/` folder contains reference material for Claude only — it is not served by the HTTP server:
- `context/ollama.md` — OllamaSharp code snippets
- `context/Hierarchy_of_Controls_02.01.23_form_508_2.pdf` — OSHA Hierarchy of Controls source document

## Key files

- `controls.json` — edit this to change control definitions; IDs must remain integers 1–7 and match what `prompt.md` references.
- `prompt.md` — edit this to tune LLM behaviour. Placeholders: `{{controls}}`, `{{hazard_title}}`, `{{hazard_description}}`, `{{risk_level}}`, `{{is_near_miss}}`.
- `samples/` — update `sample.request.json` and re-run the Node test script when adding new test cases.