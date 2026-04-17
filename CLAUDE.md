# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

C# .NET 8.0 turn-based combat simulation based on Octopath Traveler. Academic project (DDS — Diseño de Sistemas). Three-project solution:

- **Octopath-Traveler-Controller** — Game logic and entry point (console app)
- **Octopath-Traveler-View** — View abstraction layer (class library)
- **Octopath-Traveler.Tests** — xUnit test suite

**Current delivery: E2.** E1 is complete (BasicCombat, InvalidTeams, RandomBasicCombat). E2 adds: active skills (Use Skill action), Defender action, weakness system, Breaking Point, passive stats skills, and beast skills.

## Commands

```bash
# Build
dotnet build

# Run (must have data/ directory alongside executable)
dotnet run --project Octopath-Traveler-Controller

# Run all tests
dotnet test

# Run a specific test group
dotnet test --filter "DisplayName~E2-BeastsSkills"
```

## Architecture

### Core Flow

`Program.cs` → `Game` → `CombatManager` + `View`

- `Game` loads teams via `TeamLoader` and data via `JsonDataLoader`, then launches `CombatManager`
- `CombatManager` orchestrates the entire battle: turn queue, player/beast actions, damage calculation, win conditions
- All I/O is routed through `View`, which wraps an `AbstractView` implementation

### Domain Models

- `Unit` (abstract) — base for all combatants
- `Traveler` — player character with weapons, skills (active+passive), BP (Break Points), SP (Skill Points)
- `Beast` — enemy with shields, weaknesses, and a skill
- `Stats` — HP, SP, PhysAtk, PhysDef, ElemAtk, ElemDef, Speed

**Damage formulas:**
- Physical: `floor((attacker.PhysAtk * 1.3) - defender.PhysDef)`
- Elemental (skill): `floor(attacker.ElemAtk * modifier) - defender.ElemDef`
- Physical (skill): `floor(attacker.PhysAtk * modifier) - defender.PhysDef`
- All results clamped to ≥ 0. Use `Math.Floor(...)` then `Convert.ToInt32(...)`.

**Breaking Point modifier:** when a beast has 0 shields, damage dealt to it is multiplied by 1.5 (or per enunciado general).

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

Test data lives in `Octopath-Traveler.Tests/bin/Debug/net8.0/data/` organized by group. To debug a failing test, switch the view to `ManualTestingView` — it shows blue (correct) and red (incorrect) lines interactively.

**E2 test groups:**
- `E2-BeastsSkills` — beast skill execution and targeting
- `E2-DefendAndBreakingPoint` — Defender action + shield/Breaking Point mechanics
- `E2-OffensiveSkills` — traveler offensive active skills
- `E2-HealingAndQueueSkills` — heal skills, Leghold Trap, Spearhead
- `E2-BaseStatsPassives` — passive skills that modify base stats
- `E2-Mix` — combined scenarios
- `E2-Random` — randomized scenarios

### Team File Syntax

```
Name[(active_skills)][passive_skills]
```

- Max 8 active skills, 4 passive skills per character
- Team: 1–4 travelers, 1–5 beasts
- Validation at load time: no duplicates, skills must exist in JSON data

### E2: Skills System

**Skill targets:**
- `Single` — player selects one enemy (offensive) or ally (healing)
- `Ally` — player selects one ally (used for healing/revive)
- `Enemies` — hits all living beasts (no target selection prompt)
- `Party` — hits all living travelers (no target selection prompt)
- `User` — affects only the caster (passives)

**Skill types (for damage):** Fire, Ice, Lightning, Light, Wind, Dark, Sword, Spear, Dagger, Axe, Bow, Stave, Phys (physical), Elem (elemental), or `–` (typeless/healing).

**Multi-target output order** (for Enemies or Party):
1. Other travelers (board order), excluding the caster
2. All beasts (board order), excluding the caster
3. The caster itself
4. Final HP of travelers affected (board order, including caster)
5. Final HP of beasts affected (board order, including caster)
6. Final HP of caster if also affected

**Key output messages:**
- Damage: `{name} recibe {N} de daño de tipo {Type}`
- Damage with weakness: `{name} recibe {N} de daño de tipo {Type} con debilidad`
- Damage typeless (Vortal Claw): `{name} recibe {N} de daño`
- Physical (no type, Stampede etc.): `{name} recibe {N} de daño de tipo físico`
- Heal: `{name} recupera {N} de vida`
- Revive: `{name} revive`
- Final HP: `{name} termina con HP:{n}`
- Breaking Point entry: `{name} entra en Breaking Point` (shown immediately after the hit that broke it, before next hit)
- Leghold Trap: `{name} tendrá menor prioridad de turno durante 2 rondas`
- Defender hit: `{name} se defiende` shown just before that unit's damage line

**Action menu flow (Traveler turn):**
1. Show action menu → INPUT
2. If UseSkill: show skill list (order from team file) + Cancelar → INPUT
3. If Single/Ally target: show target list + Cancelar → INPUT
4. If Single/Ally OR Enemies/Party: show BP prompt → INPUT (0 to skip)
5. Print result block with separator

**Nightmare Chimera:** after skill selection, show weapon picker (all 6: Sword, Spear, Dagger, Axe, Bow, Stave + Cancelar), then target selection, then BP prompt.

**Defender action:** no output on the defender's turn. When the defending traveler is hit by an offensive skill/attack, print `{name} se defiende` just before their damage line. Defender reduces physical damage (halves it — check enunciado general for exact formula). Vortal Claw ignores Defender — no "se defiende" message, always halves HP.

**Shooting Stars:** three sequential hits per target (Wind → Light → Dark), each announced separately.

### E2: Beast Skills

Beasts never choose target or BP — they always execute their assigned skill automatically. Beast skill targeting rules:

| Skill | Target selection |
|---|---|
| Befuddling claw, Stab, Boar Rush, Vorpal Fang | Living traveler with lowest PhysDef |
| Meteor Storm, Freeze, Luminescence, Enshadow, Wind slash | Living traveler with highest Speed |
| Windshot, Firesand, Thundershot, Lightshot, Iceshot, Shadowshot | Living traveler with lowest ElemDef |
| Befuddling claw | Living traveler with highest ElemAtk |
| Stampede, Ice blast, Rampage, Incinerate, Black Gale, Galestorm | All living travelers |
| Vortal Claw | All living travelers — sets HP to floor(currentHP / 2), ignores Defender |

Physical beast attacks: `floor(beast.PhysAtk * modifier) - target.PhysDef`
Elemental beast attacks: `floor(beast.ElemAtk * modifier) - target.ElemDef`

### E2: Passive Skills (BaseStatsPassives)

Applied at game start, permanently modify the unit's stats:

| Passive | Effect |
|---|---|
| Elemental Augmentation | +50 ElemAtk |
| Summon Strength | +50 PhysAtk |
| Hale and Hearty | +500 MaxHP (and CurrentHP) |
| Fleefoot | +50 Speed |
| Inner Strength | +50 MaxSP (and CurrentSP) |

No output message for passives — their effect is visible through normal stat displays.

### E2: Weakness & Breaking Point System

- Each beast has a `Weaknesses` list of damage types
- When a traveler hits a weakness: apply 1.5× damage multiplier AND decrement shields by 1. Announce `con debilidad`.
- When shields reach 0: beast enters **Breaking Point** — announce `{name} entra en Breaking Point`. In Breaking Point: beast takes 1.5× damage on all hits (including the triggering hit onward), cannot be further broken, shields display as 0.
- If shields break mid-multi-hit (e.g., Boosted basic attack): show the Breaking Point message between the hit that broke it and the next hit.
- After a Breaking Point round (beast acted or round ended), shields reset to their original value.

### Non-Obvious Behaviors

- Turn queue is **recalculated every turn** (not once at battle start) to reflect dynamic speed changes
- BP recovery happens **at round end**, not per-turn
- Skill selection menu header: `Seleccione una habilidad` (no "para {name}" needed per E1 tests, but E2 examples show `para {name}` — follow test file output exactly)
- `ManualTestingView` is for development debugging only — never used in production or automated tests
- Data files (`characters.json`, `enemies.json`, `skills.json`, `passive_skills.json`) must be in a `data/` directory relative to the executable
- Beast `CurrentShields` (not `Shields`) must be shown in game state display
- BP prompt is shown regardless of whether the player uses BP (always prompt, ignore the value for now in E2 since BP effects on skills are not evaluated)
- Leghold Trap: target acts at end of turn queue for 2 rounds; requires tracking per-unit status effects with round countdown

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
