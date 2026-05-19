# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Critical Rule — Clean Code Ambiguity

**If there is ever any ambiguity regarding Clean Code implementation rules or specific formatting standards, you MUST use the NotebookLM skill to query the Clean Code notebook for clarification before writing code.**

## Project Overview

C# .NET 8.0 turn-based combat simulation based on Octopath Traveler. Academic project (DDS — Diseño de Sistemas). Three-project solution:

- **Octopath-Traveler-Controller** — Game logic and entry point (console app)
- **Octopath-Traveler-View** — View abstraction layer (class library)
- **Octopath-Traveler.Tests** — xUnit test suite

**Current delivery: E3.** E1 (BasicCombat, InvalidTeams, RandomBasicCombat) and E2 (active skills, Defender, Breaking Point, passives, beast skills) are complete. E3 adds: BP mechanics for basic attacks and skills, multi-hit skills, buff/debuff status effects, hybrid skills, divine skills, and new passive skills.

## Commands

```bash
dotnet build
dotnet run --project Octopath-Traveler-Controller
dotnet test
dotnet test --filter "DisplayName~E3-BasicPassives"
```

## Architecture & Design Standards

### Mandatory Design Patterns

**Polymorphism + Factory for Skills (no switch/if-chains):**
- Every skill type must be modeled as a subclass of an abstract `SkillHandler` (or equivalent)
- Adding a new skill must never require modifying existing classes — Open/Closed Principle
- A factory class resolves the correct handler from skill data

**Observer pattern for Passive Skills:**
- Passive skills subscribe to game events (round end, on damage received, on heal, etc.)
- Game engine publishes events; passive subscribers react without the engine knowing which passives exist
- Never hard-code passive checks inline — each passive is a subscriber

**Strategy pattern for Target Selection:**
- Each targeting rule (lowest PhysDef, highest Speed, all enemies, etc.) is a separate class
- Skills declare which strategy to use; `CombatManager` delegates target resolution

### MVC Architecture (strictly enforced)

- Model: domain logic only — no I/O
- View: all string formatting and output — no game state mutation
- Controller: orchestrates model + view — no direct `Console.Write` calls
- `CombatManager` must not call view methods that show game state mid-flow (show state only at defined display points)
- `Game.cs` must not call view with hardcoded strings; route through view methods

### Clean Code Chapters (all evaluated in E3)

| Chapter | Max deduction | Focus |
|---|---|---|
| 2 — Names | -1.0 | No abbreviations (s, t, d, u, e), descriptive names |
| 3 — Functions | -2.0 | Single responsibility, ≤3 args (use parameter objects), encapsulate complex conditions |
| 6 — Objects & Data Structures | -2.0 | OCP-compliant skill design, no hybrids, correct access modifiers |
| 7 — Error Handling | -1.0 | |
| 8 — Boundaries | -1.0 | |
| 10 — Classes | -2.0 | Single responsibility, skills not in CombatManager |
| MVC | -1.0 | |
| Cap 4 bonus | +0.25 | |
| Cap 5 bonus | +0.5 | |

**Known E2 issues to avoid repeating:**
- Never use single-letter variables (s, t, d, u, e) — triple penalty if >10 occurrences; single-letter names are only acceptable as loop indices in the narrowest possible scope (e.g., `for (int i = ...)`)
- Encapsulate magic numbers (0.03, 1.0, 1.5, etc.) as named constants
- Encapsulate complex LINQ predicates into named methods
- Never chain long LINQ expressions inline (train wrecks)
- `CombatManager` must not own skill-execution logic or target-selection logic

**Chapter 3 — Functions (additional rules):**
- Command Query Separation: a method either changes state OR returns a value, never both
- If a function contains a `try` block, it must be the very first statement in the function body; no code may appear after the `catch`/`finally` blocks — extract the try/catch into its own function

**Chapter 7 — Error Handling:**
- Never return `null` from a method — use the Special Case pattern or throw an exception
- Never pass `null` as an argument
- Error handling is "one thing": a function that handles errors does nothing else

**Chapter 10 — Classes (additional rule):**
- SRP "AND" test: if describing what a class does requires the word "and", it must be split into two classes

## Core Flow

`Program.cs` → `Game` → `CombatManager` + `View`

### Domain Models

- `Unit` (abstract) — base for all combatants
- `Traveler` — player character with weapons, skills (active+passive), BP (Boost Points 0-5), SP
- `Beast` — enemy with shields, weaknesses, and a skill
- `Stats` — HP, SP, PhysAtk, PhysDef, ElemAtk, ElemDef, Speed

**Damage formulas:**
- Physical (basic): `floor((attacker.PhysAtk * 1.3) - defender.PhysDef)`
- Physical (skill): `floor(attacker.PhysAtk * modifier) - defender.PhysDef`
- Elemental (skill): `floor(attacker.ElemAtk * modifier) - defender.ElemDef`
- All results clamped to ≥ 0. Use `Math.Floor(...)` then `Convert.ToInt32(...)`.
- Breaking Point: ×1.5 damage multiplier on all hits while beast has 0 shields
- Weakness: ×1.5 damage + decrement shields by 1

### View Layer

| Implementation | Use |
|---|---|
| `ConsoleView` | Interactive play |
| `TestingView` | Automated tests |
| `ManualTestingView` | Debug failing tests (blue=correct, red=incorrect) |

## Testing Strategy

Data-driven tests: `.txt` files with expected output and `INPUT:` prefixed inputs. Test data in `Octopath-Traveler.Tests/bin/Debug/net8.0/data/`.

**All test groups (E3 evaluation):**

| Group | Deduction |
|---|---|
| E1-BasicCombat | -3.0 |
| E1-InvalidTeams | -0.7 |
| E1-RandomBasicCombat | -2.3 |
| E2-BeastsSkills | -1.0 |
| E2-DefendAndBreakingPoint | -1.0 |
| E2-OffensiveSkills | -1.0 |
| E2-HealingAndQueueSkills | -1.0 |
| E2-BaseStatsPassives | -1.0 |
| E2-Mix | -0.4 |
| E2-Random | -0.4 |
| E3-BasicAttackBoosting | -0.1 |
| E3-BasicAttackBoostingRandom | -0.05 |
| E3-BasicPassives | -0.1 |
| E3-BasicPassivesRandom | -0.05 |
| E3-MultiHitOffensive | -0.15 |
| E3-BasicSkillBoosting | -0.15 |
| E3-AdvanceSkillBoosting | -0.5 |
| E3-StatusEffects | -0.5 |
| E3-BeastsSkills | -0.5 |
| E3-DivineSkills | -0.5 |
| E3-IntermediatePassives | -0.5 |
| E3-Mix | -0.1 |
| E3-Random | -0.1 |
| E3-Bonus | +0.6 (optional) |
| E3-BonusRandom | +0.4 (optional) |
| GUI missing | -2.5 |

## E3: Boost Points (BP) Mechanics

### Basic Attack Boosting

- BP prompt always shown after weapon/target selection
- Using N BP on basic attack → repeat the attack N+1 times total (1 base + N extra)
- Each hit shown individually; final HP shown once at end
- If unit dies mid-combo, continue showing remaining hits at full damage (unit stays at HP 0)
- BP validation: if input > current BP → show error and re-prompt (loop)
- Traveler gains 1 BP per round (max 5), starts at 0

**Error message:** `{name} no tiene {N} BP para utilizar`

**Output (basic attack with 2 BP — 3 hits total):**
```
----------------------------------------
H'aanit ataca
Meep recibe 373 de daño de tipo Axe
Meep recibe 373 de daño de tipo Axe
Meep recibe 373 de daño de tipo Axe
Meep termina con HP:189
```

### Skill Boosting

- BP increases modifier by a skill-specific percentage per BP used
- Buff/debuff skills: BP increases duration by a fixed number of rounds per BP
- Divine skills: require exactly 3 BP, no BP scaling, no BP prompt shown, cost automatically deducted

## E3: Multi-Hit Skills

For skills with Hits > 1, each hit on each target is printed sequentially. For AOE, all hits on the first target before moving to next:

**Output (Fire Storm — 2 hits, all enemies):**
```
Cyrus usa Fire Storm
Meep recibe 472 de daño de tipo Fire
Meep recibe 472 de daño de tipo Fire
Shaggy Meep recibe 502 de daño de tipo Fire
Shaggy Meep recibe 502 de daño de tipo Fire
War Wolf recibe 463 de daño de tipo Fire
War Wolf recibe 463 de daño de tipo Fire
Meep termina con HP:72
Shaggy Meep termina con HP:32
War Wolf termina con HP:2622
```

Breaking Point mid-combo: show `{name} entra en Breaking Point` immediately after the hit that triggered it; subsequent hits use the ×1.5 multiplier.

### HP/SP Drain Skills

HP Thief and Steal SP: show all hits, then show the restoration, then final HPs.
- HP Thief restores `floor(totalDamage / 2)` HP
- Steal SP restores `floor(totalDamage * 0.05)` SP

```
Therion usa HP Thief
Meep recibe 172 de daño de tipo Dagger
Meep recibe 172 de daño de tipo Dagger
Therion recupera 172 de vida
Meep termina con HP:554
Therion termina con HP:4093
```

## E3: Buff/Debuff System

Status effects track their remaining duration and apply stat modifiers while active.

**Output:** `{name} tendrá {Effect} durante {N} rondas`

Where Effect is one of: `Increased Physical Attack`, `Increased Physical Defense`, `Increased Elemental Attack`, `Increased Elemental Defense`, `Increased Speed`, `Decreased Physical Attack`, `Decreased Physical Defense`, `Decreased Elemental Attack`, `Decreased Elemental Defense`, `Decreased Speed`.

Multiple effects from one skill (e.g., Starsong) each get their own line.

## E3: Divine Skills

- Only appear in skill selection menu when player has ≥ 3 BP
- No BP prompt: automatically consume 3 BP
- Announced and resolved like any other offensive skill
- Steorra's Prophecy: base modifier 3.0 + 0.2 × (total team BP excluding the 3 spent to cast)

## E3: Active Skills (new in E3)

**Offensive multi-hit:**

| Skill | Type | SP | Modifier | Target | Hits | BP effect |
|---|---|---|---|---|---|---|
| Fire Storm | Fire | 22 | 1.6 | Enemies | 2 | +90% mod/BP |
| Blizzard | Ice | 22 | 1.6 | Enemies | 2 | +90% mod/BP |
| Lightning Blast | Lightning | 22 | 1.6 | Enemies | 2 | +90% mod/BP |
| HP Thief | Dagger | 6 | 1.6 | Single | 2 | +70% mod/BP |
| Steal SP | Dagger | 6 | 1.6 | Single | 2 | +70% mod/BP |
| Thousand Spears | Spear | 20 | 0.8 | Single (lowest PhysDef) | 7 | +50% mod/BP |
| Rain of Arrows | Bow | 8 | 0.8 | Single (lowest HP) | 6 | +50% mod/BP |
| Arrowstorm | Bow | 24 | 0.8 | Enemies | 6 | +40% mod/BP |
| Guardian Liondog | Sword | 35 | 2.0 | Single (highest Speed) | 5 | +80% mod/BP |
| Ignis Ardere | Fire | 36 | 1.6 | Enemies | 3 | +90% mod/BP |
| Glacies Claudere | Ice | 36 | 1.6 | Enemies | 3 | +90% mod/BP |
| Tonitrus Canere | Lightning | 36 | 1.6 | Enemies | 3 | +90% mod/BP |
| Ventus Saltare | Wind | 36 | 1.6 | Enemies | 3 | +90% mod/BP |
| Lux Congerere | Light | 36 | 1.6 | Enemies | 3 | +90% mod/BP |
| Tenebrae Operire | Dark | 36 | 1.6 | Enemies | 3 | +90% mod/BP |

**Buff/Debuff:**

| Skill | SP | Target | Effect | Base Duration | BP effect |
|---|---|---|---|---|---|
| Sheltering Veil | 6 | Ally | Increased Elemental Defense | 2 rounds | +2 rounds/BP |
| Abide | 4 | Ally | Increased Physical Attack | 2 rounds | +2 rounds/BP |
| Stout Wall | 4 | User | Increased Physical Defense | 3 rounds | +2 rounds/BP |
| Lion Dance | 4 | Ally | Increased Physical Attack | 2 rounds | +2 rounds/BP |
| Peacock Strut | 4 | Ally | Increased Elemental Attack | 2 rounds | +2 rounds/BP |
| Mole Dance | 4 | Ally | Increased Physical Defense | 2 rounds | +2 rounds/BP |
| Panther Dance | 4 | Ally | Increased Speed | 2 rounds | +2 rounds/BP |
| Shackle Foe | 4 | Single (enemy) | Decreased Physical Attack | 2 rounds | +2 rounds/BP |
| Armor Corrosive | 4 | Single (enemy) | Decreased Physical Defense | 2 rounds | +2 rounds/BP |
| Starsong | 25 | Ally | Increased Phys Def + Elem Def + Speed | 2 rounds | +2 rounds/BP |

**Hybrid:**

| Skill | Type | SP | Modifier | Target | Hits | Effect | BP effect |
|---|---|---|---|---|---|---|---|
| Elemental Break | Stave | 20 | 2.0 | Single | 1 | Decreased Elemental Defense 2 rounds | +100% mod/BP |

**Divine (require 3 BP):**

| Skill | Type | SP | Modifier | Target | Hits | Special |
|---|---|---|---|---|---|---|
| Brand's Thunder | Sword | 30 | 7.0 | Single | 1 | — |
| Draefendi's Rage | Bow | 30 | 7.0 | Enemies | 1 | — |
| Steorra's Prophecy | Dark | 50 | 3.0 | Enemies | 1 | +0.2 mod per team BP (excl. 3 spent) |
| Balogar's Blade | 6 elements | 50 | 1.5 | Single | 6 | Fire→Ice→Lightning→Wind→Light→Dark |
| Winnehild's Battle Cry | 6 weapons | 50 | 1.5 | Enemies | 6 | Sword→Spear→Dagger→Axe→Bow→Stave |

## E3: Passive Skills (Observer Pattern)

All passives subscribe to game events. Never check passives inline.

**User-targeted passives (subscriber on self):**

| Passive | Trigger | Effect |
|---|---|---|
| Vim and Vigor | End of round | Recover 10% MaxHP |
| Second Wind | End of round | Recover 5% MaxSP |
| Patience | End of round (after all turns) | If HP and SP are both even → grant 1 extra turn. One-time per round. Multiple Patience units act in board order, ignoring speed sort. |
| Persistence | On receive buff | Duration +1 round |
| Hang Tough | On receive lethal damage | If above 10% MaxHP, leave at 1 HP instead of 0 |
| SP Saver | On skill use | Halve SP cost |
| Encore | On death (first time) | Revive with 25% MaxHP |
| Inspiration | After basic attack | Restore SP = 1% of total damage dealt |
| Heightened Healing | On receive heal | Heal amount ×1.3 |
| Boost Start | Battle start | Begin with 1 extra BP |
| Divine Aura | On receive attack | If receiver HP and attacker HP are both even → negate damage |
| Stat Swap | Battle start | Swap PhysAtk ↔ ElemAtk |

**Ally-targeted passive:**

| Passive | Trigger | Effect |
|---|---|---|
| The Show Goes On | On grant buff | Duration of granted buff +1 round |

**Patience output:**
```
{name} obtiene un turno adicional
```
Then the normal turn menu follows. Multiple Patience activations: one message each in board order, then show full turn order (including extra turns).

**Encore output (single-hit lethal):**
```
{name} recibe {N} de daño de tipo {Type}
{name} revive
{name} termina con HP:{floor(MaxHP * 0.25)}
```

**Encore output (multi-hit, lethal hit mid-combo):**
```
{name} recibe {N} de daño    ← lethal hit
{name} revive
{name} recibe {N} de daño    ← remaining hits apply normally
{name} termina con HP:{X}
```

## E3: Beast Skills (new)

| Skill | Type | Modifier | Target | Hits | Special |
|---|---|---|---|---|---|
| Double Bite | Phys | 1.3 | Single (lowest PhysDef) | 2 | — |
| Shadow Magic | Elem | 1.5 | Party (all travelers) | 2 | — |
| Triple Slash | Phys | 1.3 | Single (highest HP) | 3 | — |
| Consume Armor | — | — | Single (highest PhysDef) | 1 | Decreased Physical Defense 2 rounds |
| Flap | Phys | 1.4 | Single (highest HP) | 1 | Increased Speed to self 2 rounds |
| Acid Spray | — | — | Single (highest HP) | 1 | Decreased Phys Def + Decreased Elem Def 2 rounds |
| Gather Strength | Phys | 1.4 | Single (lowest PhysDef) | 1 | Increased Physical Attack to self 2 rounds |
| Augmented Magic | — | — | Party (all beasts) | 1 | Increased Elem Atk + Increased Elem Def 2 rounds |
| Volcano | Elem | 1.5 | Party (all travelers) | 1 | Then Decreased Elemental Defense to all travelers 2 rounds |

## E2 Carry-Over: Skills System

**Skill targets:** Single, Ally, Enemies, Party, User

**Multi-target output order** (Enemies/Party):
1. All hits on each target in board order (non-caster travelers, then beasts, then caster)
2. Final HP of all travelers (board order)
3. Final HP of all beasts (board order)

**Key output messages:**
- `{name} recibe {N} de daño de tipo {Type}` — typed damage
- `{name} recibe {N} de daño de tipo {Type} con debilidad` — weakness hit
- `{name} recibe {N} de daño` — typeless (Vortal Claw)
- `{name} recibe {N} de daño de tipo físico` — Phys type (no weakness possible)
- `{name} recupera {N} de vida`
- `{name} recupera {N} SP`
- `{name} revive`
- `{name} termina con HP:{n}`
- `{name} entra en Breaking Point`
- `{name} tendrá {Effect} durante {N} rondas`
- `{name} obtiene un turno adicional`
- `{name} se defiende` — shown before damage line when defender is hit
- `{name} tendrá menor prioridad de turno durante 2 rondas` — Leghold Trap

**Action menu flow (Traveler turn):**
1. Action menu → INPUT (1: Ataque básico, 2: Usar habilidad, 3: Defender, 4: Huir)
2. If UseSkill: skill list (team file order) + Cancelar → INPUT
   - Divine skills only appear if player has ≥3 BP
3. If Single/Ally target: target list + Cancelar → INPUT
4. If NOT divine: BP prompt → INPUT (validate; re-prompt if invalid)
5. If divine: no BP prompt; consume 3 BP automatically
6. Result block + separator

## E2 Carry-Over: Weakness & Breaking Point

- Weakness hit: ×1.5 damage, decrement shields by 1, print `con debilidad`
- Shields → 0: print `{name} entra en Breaking Point` immediately after the triggering hit
- While in Breaking Point: all damage ×1.5
- Breaking Point resets at round end (after beast acted or round over)

## E2 Carry-Over: BaseStats Passives (applied at game start, no output)

| Passive | Effect |
|---|---|
| Elemental Augmentation | +50 ElemAtk |
| Summon Strength | +50 PhysAtk |
| Hale and Hearty | +500 MaxHP + CurrentHP |
| Fleefoot | +50 Speed |
| Inner Strength | +50 MaxSP + CurrentSP |

## Non-Obvious Behaviors

- Turn queue recalculated every turn (reflects dynamic speed/buff changes)
- BP recovery: +1 per round per traveler, at round end, max 5
- BP is spent when used; dead units still count toward team BP total (Steorra's Prophecy)
- `TravelerAction` enum must live in its own file, not inside `CombatManager`
- Data files (`characters.json`, `enemies.json`, `skills.json`, `passive_skills.json`) in `data/` relative to executable
- Beast `CurrentShields` (not `Shields`) shown in game state
- If unit dies mid-multi-hit: continue showing all remaining hits at normal damage, unit stays at HP 0
- Patience extra turns follow board order, not speed order

## Token Efficiency & Core Behavior
- Think before acting. Read existing files before writing code.
- Be concise in output but thorough in reasoning.
- Prefer editing over rewriting whole files.
- Do not re-read files you have already read unless the file may have changed.
- Skip files over 100KB unless explicitly required.
- Test your code before declaring done.
- No sycophantic openers or closing fluff.
- Keep solutions simple and direct.
- User instructions always override this file.
