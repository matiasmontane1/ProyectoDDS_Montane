# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

C# .NET 8.0 turn-based combat simulation based on Octopath Traveler. Academic project (DDS — Diseño de Sistemas). Three-project solution:

- **Octopath-Traveler-Controller** — Game logic and entry point (console app)
- **Octopath-Traveler-View** — View abstraction layer (class library)
- **Octopath-Traveler.Tests** — xUnit test suite

## Commands

```bash
# Build
dotnet build

# Run (must have data/ directory alongside executable)
dotnet run --project Octopath-Traveler-Controller

# Run all tests
dotnet test

# Run a specific test group
dotnet test --filter "DisplayName~E1-BasicCombat"
```

## Architecture

### Core Flow

`Program.cs` → `Game` → `CombatManager` + `View`

- `Game` loads teams via `TeamLoader` and data via `JsonDataLoader`, then launches `CombatManager`
- `CombatManager` orchestrates the entire battle: turn queue, player/beast actions, damage calculation, win conditions
- All I/O is routed through `View`, which wraps an `AbstractView` implementation

### Domain Models

- `Unit` (abstract) — base for all combatants
- `Traveler` — player character with weapons, skills, BP (Break Points), SP (Skill Points)
- `Beast` — enemy with shields and skills
- `Stats` — HP, SP, PhysAtk, PhysDef, ElemAtk, ElemDef, Speed

Damage formula: `damage = (attacker.PhysAtk * 1.3) - defender.PhysDef`

### View Layer (critical for testability)

`AbstractView` has three concrete implementations injected into `Game`:

| Implementation | Use |
|---|---|
| `ConsoleView` | Interactive play |
| `TestingView` | Automated tests (reads inputs from test file) |
| `ManualTestingView` | Debug failing tests (color-codes expected vs. actual output) |

The `Script` class records all output and input lines during a run, enabling line-by-line comparison in tests.

### Testing Strategy

Tests are data-driven: each test case is a `.txt` file containing both expected output lines and `INPUT:` prefixed user inputs. The framework runs the game, captures output into a `Script`, and compares it line-by-line.

Test data lives in `Octopath-Traveler.Tests/data/` organized by group (`E1-BasicCombat/`, `E1-InvalidTeams/`, `E1-RandomBasicCombat/`). To debug a failing test, switch the view to `ManualTestingView` — it shows blue (correct) and red (incorrect) lines interactively.

### Team File Syntax

```
Name[(active_skills)][passive_skills]
```

- Max 8 active skills, 4 passive skills per character
- Team: 1–4 travelers, 1–5 beasts
- Validation at load time: no duplicates, skills must exist in JSON data

### Non-Obvious Behaviors

- Turn queue is **recalculated every turn** (not once at battle start) to reflect dynamic speed changes
- BP recovery happens **at round end**, not per-turn
- `ManualTestingView` is for development debugging only — never used in production or automated tests
- Data files (`characters.json`, `enemies.json`, `skills.json`, etc.) must be in a `data/` directory relative to the executable

## Token Efficiency & Core Behavior
- Think before acting. Read existing files before writing code.
- Be concise in output but thorough in reasoning.
- Prefer editing over rewriting whole files.
- Do not re-read files you have already read unless the file may have changed.
- Skip files over 100KB unless explicitly required.
- Suggest running /cost when a session is running long to monitor cache ratio.
- Recommend starting a new session when switching to an unrelated task.
- Test your code before declaring done.
- No sycophantic openers or closing fluff (e.g., skip "Sure!", "Great question!").
- Keep solutions simple and direct.
- User instructions always override this file.