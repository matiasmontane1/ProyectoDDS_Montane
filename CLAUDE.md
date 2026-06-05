# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Critical Rules for Ambiguity Resolution

If there is ever any ambiguity regarding the implementation, you MUST resolve it by consulting the following designated sources before writing any code or making assumptions:

1. **Clean Code & Formatting Doubts:** You MUST use the NotebookLM skill to query my notebook named "clean code" for clarification.
2. **Game Rules & Mechanics Doubts:** You MUST read and review the official project documents located at `Documentos/md/Octopath_Traveler__Enunciado_General.md` and `Documentos/md/Octopath_Traveler__E3.md`. Do not invent, guess, or hallucinate game mechanics.

## Project Overview

C# .NET 8.0 turn-based combat simulation based on Octopath Traveler. Academic project (DDS — Diseño Detallado de Software). Three-project solution:

- **Octopath-Traveler-Controller** — Game logic and entry point (console app)
- **Octopath-Traveler-View** — View abstraction layer (class library)
- **Octopath-Traveler.Tests** — xUnit test suite

**Current delivery: E3.** E1 and E2 are complete. Phases 1, 2, and 3 are done. Phase 4 (passives) is next.

## E3 Implementation Roadmap (Strict Phased Execution)

**CRITICAL:** Execute this plan strictly one phase at a time. Do not move to the next phase until the tests for the current phase pass. **Furthermore, you MUST NOT proceed to the next phase without my explicit authorization. After finishing a phase, passing its tests, you must STOP and ask me: "Phase [X] is complete and all tests are passing. Shall I proceed to Phase [Y]?" Wait for my explicit confirmation before writing any code for the next phase.**

### ✅ Phase 1 — BP Mechanics (Basic Attack + Skill Boosting) — COMPLETE
Goal: `E3-BasicAttackBoosting`, `E3-BasicSkillBoosting`

All passing. Key implementations:
- `CombatMenuView.PromptBpUsage` validates input and re-prompts on error.
- `BasicAttackHandler.Execute` loops `1 + bpUsed` times.
- `SkillHandler.Execute` scales modifier: `modifier * (1 + bpUsed * bonusPerBp)`.
- Buff/debuff skills: BP extends duration, not modifier.
- `SkillHandlerFactory` routes divine skills to `DivineSkillHandler`. Gate: `currentBp >= 3`.

### ✅ Phase 2 — Multi-Hit Skills & Encore Interruption — COMPLETE
Goal: `E3-MultiHitOffensive`, `E3-AdvanceSkillBoosting`

All passing. Key implementations:
- `ActiveSkill.Hits` parsed via regex `(\d+) (?:veces|ataques?)`, defaults to 1.
- `OffensiveSkillHandler.Execute` loops `hits` times per target. BP only scales modifier, never hit count.
- Breaking Point interruption: detected via `ShieldDamageProcessor`; announcement printed before next hit.
- Encore revive fires mid-combo; remaining hits continue on revived unit.
- Drain skills: `HP Thief` → `floor(total / 2)` HP; `Steal SP` → `floor(total * 0.05)` SP; after all hits.
- Full infrastructure in place: `StatusEffect`, `Unit.ActiveEffects`, `ApplyStatusEffect` (`+=` stacking), `OffensiveDebuffSkillHandler`, `BuffSkillHandler`, `BeastDebuffSkillHandler`.

### ✅ Phase 3 — Buff/Debuff Status Effects & Beast Skills — COMPLETE
Goal: `E3-StatusEffects`, `E3-BeastsSkills`

All 41 tests passing. Root causes discovered and fixed:

**Beast multi-hit (Double Bite, Shadow Magic, Triple Slash):** `BeastTurnController` was not looping over `BeastSkill.Hits`. Fix: added hit loop in `ExecuteSingleTargetSkill` and `ExecuteAoeSkill`.

**Beast buff/debuff routing:** `IsNonDamaging` skills (Modifier == 0) were being silently skipped. Fix: `ExecuteNonDamagingSkill` dispatches to:
- `ApplyAllBeastEffects` when `AllBeastEffects.Count > 0` (Augmented Magic pattern — Target="Party")
- `ApplyTargetDebuffs` otherwise (Consume Armor, Acid Spray)

**Beast hybrid skills** (Gather Strength, Flap): damage hit loop first, then self-buff shown and applied, then final HP.

**Volcano (AoE + per-target debuff):** for each target: damage hit, then debuff shown and applied, then final HPs for all.

**`se defiende` message:** shown ONCE per target BEFORE the hit loop — never once per hit. The damage halving still applies to all hits.

**Speed buff mid-round turn order:** `TurnQueueManager.SortRemainingQueue(queue)` called in `CombatManager.HandleSingleTurn` after removing the current unit. Preserves priority groups (recovery beasts → defenders → spearheads → normal → desprioritized), re-sorts within each group by `EffectiveSpeed`.

**Effect parsing from description:** `BeastSkill` uses Regex to parse four effect categories from the Spanish description text:
- `SelfEffects`: `"El usuario obtiene X durante N rondas"`
- `TargetEffects`: `"Aplica X al viajero ... durante N rondas"`
- `PostAttackTargetEffects`: `"Luego aplica X a ... durante N rondas"`
- `AllBeastEffects`: `"Otorga X a todas las bestias durante N rondas"`

**`BeastTurnController` requires enemy team:** constructor now takes `List<Beast> enemyTeam` — needed by `ApplyAllBeastEffects` to iterate all alive beasts.

### Phase 4 — Observer Pattern for Passive Skills (Strict MVC)
Goal: `E3-BasicPassives`, `E3-IntermediatePassives`

**Existing infrastructure (do NOT re-implement):**
- `PassiveSkillApplicator` in `Models/PassiveSkillApplicator.cs` — called in `CombatManager.InitializeCombatants()`. Currently applies base-stat passives (BoostStart, StatSwap). Extend or replace with the Observer pattern.
- `passive_skills.json` data file — already loaded.

**What needs building:**

1. **Event types** — define as records/classes implementing `IPassiveEvent`:
   - `BattleStartEvent(Traveler traveler)`
   - `RoundEndEvent(Traveler traveler)`
   - `OnDamageReceivedEvent(Unit attacker, Unit defender, int damageAmount)`
   - `OnHealEvent(Unit healer, Unit target, int healAmount)`
   - `OnSkillUseEvent(Traveler caster, ActiveSkill skill)`
   - `OnBasicAttackEvent(Traveler attacker, int totalDamage)`
   - `OnBuffReceivedEvent(Traveler target, StatusEffect effect)`
   - `OnBuffGrantedEvent(Traveler caster, Traveler target, StatusEffect effect)`
   - `OnDeathEvent(Unit deceased)` — for Encore

2. **`EventPublisher`** — holds subscriber list; `Publish(IPassiveEvent)` returns `List<string>` (messages from all activated observers). Observers MUST NOT call View/Console — they return strings only.

3. **`CombatManager`** — receives `List<string>` from `EventPublisher.Publish()` and passes to `CombatView` to print. Add `EventPublisher` field; fire events at these exact points:
   - `BattleStartEvent`: in `InitializeCombatants()` per traveler → BoostStart, StatSwap
   - `OnDamageReceivedEvent`: immediately after `TakeDamage()` in every damage-dealing path → HangTough, Encore, DivineAura
   - `OnHealEvent`: after every `Heal()` call → HeightenedHealing
   - `OnSkillUseEvent`: before SP is deducted in `SkillHandler.Execute` → SpSaver
   - `OnBasicAttackEvent`: after all basic-attack hits complete → Inspiration
   - `OnBuffReceivedEvent`: after every `ApplyStatusEffect()` on a Traveler → Persistence
   - `OnBuffGrantedEvent`: when a caster grants a buff to a different target → TheShowGoesOn
   - `RoundEndEvent`: in `EndOfRoundProcessing()` per alive traveler, after BP/defense reset → VimAndVigor, SecondWind, Patience

4. **Concrete observers:** `VimAndVigorObserver`, `SecondWindObserver`, `HangToughObserver`, `EncoreObserver`, `InspirationObserver`, `HeightenedHealingObserver`, `BoostStartObserver`, `PersistenceObserver`, `SpSaverObserver`, `DivineAuraObserver`, `StatSwapObserver`, `TheShowGoesOnObserver`, `PatienceObserver`.

5. **PatienceObserver (Strict Rule):** On `RoundEndEvent`, if HP and SP are BOTH even, grant an extra turn. Append extra turns to the END of the current round queue. Multiple Patience activations must be ordered strictly by board position, ignoring Speed.

**Output messages for passives (confirmed from spec):**
- VimAndVigor: `{name} recupera {N} de vida`
- SecondWind: `{name} recupera {N} SP`
- HangTough: prevents death (no message — HP set to 1)
- Encore: `{name} revive` then `{name} termina con HP:{floor(MaxHP*0.25)}`
- Inspiration: `{name} recupera {N} SP`
- HeightenedHealing: healed amount is ×1.3 (no extra message, just more HP)
- BoostStart: starts with +1 BP (applies before combat, no message)
- StatSwap: swaps PhysAtk ↔ ElemAtk (applies before combat, no message)
- Persistence: buff duration +1 (applied silently during buff reception)
- SpSaver: SP cost halved (applied silently before deduction)
- DivineAura: if both HP values even → 0 damage (no extra message)
- TheShowGoesOn: buff duration +1 on granted buff (silent)
- Patience: `{name} obtiene un turno adicional`

### Phase 5 — Divine Skills
Goal: `E3-DivineSkills`
1. **DivineSkillHandler:** No BP prompt; auto-consume 3 BP.
2. **Steorra's Prophecy:** Modifier = `3.0 + 0.2 * totalTeamBp` (sum all travelers' BP including dead, **excluding** the 3 spent to cast).
3. **Multi-element Divines:** Loop over the element/weapon array defined on the skill; each hit uses that specific element/type.

### Phase 6 — Bonus Implementation (Optional prep)
Goal: `E3-Bonus`, `E3-BonusRandom`
1. Prepare the architecture to easily accept `Ethereal Healing` (heal over time), `Aelfric's Auspices` (skill executes twice), `Sealticge's Seduction` (Single target becomes AoE), and extra passives (`Saving Grace`, `Second Serving`).

## Commands

```bash
dotnet build
dotnet run --project Octopath-Traveler-Controller
dotnet test Octopath-Traveler.Tests                                              # full suite
dotnet test Octopath-Traveler.Tests --filter "DisplayName~E3-StatusEffects|DisplayName~E3-BeastsSkills"
dotnet test Octopath-Traveler.Tests --filter "DisplayName~E3-BasicPassives"
dotnet test Octopath-Traveler.Tests --filter "DisplayName~E3-IntermediatePassives"
dotnet test Octopath-Traveler.Tests --filter "DisplayName~E3-DivineSkills"
dotnet test Octopath-Traveler.Tests --filter "DisplayName~E3-BasicPassives|DisplayName~E3-IntermediatePassives"
dotnet test Octopath-Traveler.Tests --filter "DisplayName~E1|DisplayName~E2"    # regression check
dotnet test Octopath-Traveler.Tests --filter "DisplayName~E3-BasicAttackBoosting|DisplayName~E3-BasicSkillBoosting|DisplayName~E3-MultiHitOffensive|DisplayName~E3-AdvanceSkillBoosting|DisplayName~E3-StatusEffects|DisplayName~E3-BeastsSkills"  # phases 1–3
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
- **Observers must NEVER call Console/View methods directly** — they return `List<string>` messages; CombatManager prints them.

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

**Known E2 issues to avoid repeating:**
- Never use single-letter variables (s, t, d, u, e) — triple penalty if >10 occurrences; single-letter names are only acceptable as loop indices in the narrowest possible scope (e.g., `for (int i = ...)`).
- Encapsulate magic numbers (0.03, 1.0, 1.5, etc.) as named constants.
- Encapsulate complex LINQ predicates into named methods.
- Never chain long LINQ expressions inline (train wrecks).
- `CombatManager` must not own skill-execution logic or target-selection logic.

**Chapter 3 — Functions (additional rules):**
- Command Query Separation: a method either changes state OR returns a value, never both.
- If a function contains a `try` block, it must be the very first statement in the function body; no code may appear after the `catch`/`finally` blocks — extract the try/catch into its own function.

**Chapter 7 — Error Handling:**
- Never return `null` from a method — use the Special Case pattern or throw an exception.
- Never pass `null` as an argument.
- Error handling is "one thing": a function that handles errors does nothing else.

**Chapter 10 — Classes (additional rule):**
- SRP "AND" test: if describing what a class does requires the word "and", it must be split into two classes.

## Core Flow
`Program.cs` → `Game` → `CombatManager` + `View`

### Domain Models
- `Unit` (abstract) — base for all combatants
- `Traveler` — player character with weapons, skills (active+passive), BP (Boost Points 0-5), SP
- `Beast` — enemy with shields, weaknesses, and a skill
- `Stats` — HP, SP, PhysAtk, PhysDef, ElemAtk, ElemDef, Speed

**Damage formulas:**

All damage follows this pipeline:
1. `baseRaw = max(0, round(Attack × modifier − Defense, 6))`
   - Physical: `Attack = PhysAtk`, `Defense = PhysDef`
   - Elemental: `Attack = ElemAtk`, `Defense = ElemDef`
   - Basic attack modifier = 1.3 (physical)
2. `contextMultiplier = 1.0 + (0.5 if weakness) + (0.5 if breaking point)`
3. `result = floor(baseRaw × contextMultiplier × AttackMultiplier / DefenseMultiplier)`
   - `AttackMultiplier` = product of all active "Increased/Decreased Attack" effects on the attacker
   - `DefenseMultiplier` = product of all active "Increased/Decreased Defense" effects on the target
   - Default multipliers = 1.0 (no active effects)
   - "Increased" effects → multiplier 1.5; "Decreased" effects → multiplier 2/3

Both basic attacks and skill attacks apply AttackMultiplier/DefenseMultiplier.

- Breaking Point: ×1.5 damage multiplier on all hits while beast has 0 shields
- Weakness: ×1.5 damage + decrement shields by 1

### View Layer
| Implementation | Use |
|---|---|
| `ConsoleView` | Interactive play |
| `TestingView` | Automated tests |
| `ManualTestingView` | Debug failing tests (blue=correct, red=incorrect) |

## E3: Boost Points (BP) Mechanics
### Basic Attack Boosting
- BP prompt shown after weapon/target selection **only if `currentBp ≥ 1`** — if `currentBp == 0`, the prompt is skipped entirely (no output line, no input consumed) and `bpUsed = 0` automatically
- Using N BP on basic attack → repeat the attack N+1 times total (1 base + N extra)
- Each hit shown individually; final HP shown once at end
- If unit dies mid-combo, continue showing remaining hits at full damage (unit stays at HP 0)
- BP validation: if input > current BP → show error and re-prompt (loop)
- Traveler starts combat with **1 BP** (not 0); gains +1 per round at round end, max 5
- **Travelers who spent BP this round do NOT recover BP at round end** (`SpentBpThisRound` flag)

**Error message:** `{name} no tiene {N} BP para utilizar`

**Output (basic attack with 2 BP — 3 hits total):**
```text
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
For skills with Hits > 1, each hit on each target is printed sequentially. For AoE, all hits on the first target before moving to next:

**Output (Fire Storm — 2 hits, all enemies):**
```text
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

```text
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

## E3: Beast Skill Output Patterns

Beast skill output depends on skill category. The category is determined from the description in `beast_skills.json`.

**Multi-hit single-target (e.g., Double Bite — 2 hits):**
```text
Remnant usa Double Bite
Z'aanta recibe 37 de daño físico
Z'aanta recibe 37 de daño físico
Z'aanta termina con HP:3148
```

**Multi-hit AoE (e.g., Shadow Magic — 2 hits, all travelers):**
```text
Devourer of Men usa Shadow Magic
Olberic recibe 194 de daño elemental
Olberic recibe 194 de daño elemental
Primrose recibe 182 de daño elemental
Primrose recibe 182 de daño elemental
Olberic termina con HP:5393
Primrose termina con HP:3638
```

**Attack + self-buff (e.g., Gather Strength, Flap):**
```text
Accursed Armor usa Gather Strength
Olberic recibe 182 de daño físico
Accursed Armor tendrá Increased Physical Attack durante 2 rondas
Olberic termina con HP:5599
```

**AoE attack + per-target debuff (Volcano):**
```text
Blood Revenant usa Volcano
Olberic recibe 593 de daño elemental
Olberic tendrá Decreased Elemental Defense durante 2 rondas
Primrose recibe 532 de daño elemental
Primrose tendrá Decreased Elemental Defense durante 2 rondas
Tressa recibe 579 de daño elemental
Tressa tendrá Decreased Elemental Defense durante 2 rondas
Olberic termina con HP:5188
Primrose termina con HP:3470
Tressa termina con HP:4312
```

**Pure debuff on traveler (Consume Armor — single effect):**
```text
Armor Eater usa Consume Armor
Olberic tendrá Decreased Physical Defense durante 2 rondas
```

**Pure multi-debuff on traveler (Acid Spray — two effects):**
```text
Army Ant usa Acid Spray
Primrose tendrá Decreased Physical Defense durante 2 rondas
Primrose tendrá Decreased Elemental Defense durante 2 rondas
```

**Buff all beasts (Augmented Magic — iterates all alive beasts in board order):**
```text
Curator usa Augmented Magic
Frost Fox tendrá Increased Elemental Attack durante 2 rondas
Frost Fox tendrá Increased Elemental Defense durante 2 rondas
Curator tendrá Increased Elemental Attack durante 2 rondas
Curator tendrá Increased Elemental Defense durante 2 rondas
```

**Defending against multi-hit beast attack:**
`{name} se defiende` is shown **once** before the hit loop, NOT once per hit. All hits still deal halved damage.
```text
Remnant usa Double Bite
Z'aanta se defiende
Z'aanta recibe 18 de daño físico
Z'aanta recibe 18 de daño físico
Z'aanta termina con HP:2620
```

## E3: Divine Skills
- Only appear in skill selection menu when player has ≥ 3 BP
- No BP prompt: automatically consume 3 BP
- Announced and resolved like any other offensive skill
- Steorra's Prophecy: base modifier 3.0 + 0.2 × (total team BP excluding the 3 spent to cast)

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
```text
{name} obtiene un turno adicional
```
Then the normal turn menu follows. Multiple Patience activations: one message each in board order, then show full turn order (including extra turns).

**Encore output (single-hit lethal):**
```text
{name} recibe {N} de daño de tipo {Type}
{name} revive
{name} termina con HP:{floor(MaxHP * 0.25)}
```

**Encore output (multi-hit, lethal hit mid-combo):**
```text
{name} recibe {N} de daño    ← lethal hit
{name} revive
{name} recibe {N} de daño    ← remaining hits apply normally
{name} termina con HP:{X}
```

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
- `{name} recibe {N} de daño elemental` — beast elemental attack
- `{name} recibe {N} de daño físico` — beast physical attack
- `{name} recupera {N} de vida`
- `{name} recupera {N} SP`
- `{name} revive`
- `{name} termina con HP:{n}`
- `{name} entra en Breaking Point`
- `{name} tendrá {Effect} durante {N} rondas`
- `{name} obtiene un turno adicional`
- `{name} se defiende` — shown **once per target** before the hit block (not once per hit)
- `{name} tendrá menor prioridad de turno durante 2 rondas` — Leghold Trap

**Action menu flow (Traveler turn):**
1. Action menu → INPUT (1: Ataque básico, 2: Usar habilidad, 3: Defender, 4: Huir)
2. If UseSkill: skill list (team file order) + Cancelar → INPUT
   - Divine skills only appear if player has ≥3 BP
3. If Single/Ally target: target list + Cancelar → INPUT
4. If NOT divine AND `currentBp ≥ 1`: BP prompt → INPUT (validate; re-prompt if invalid). If `currentBp == 0`: skip entirely (no output, no input).
5. If divine: no BP prompt; consume 3 BP automatically
6. Result block + separator

## E2 Carry-Over: Weakness & Breaking Point
- Weakness hit: ×1.5 damage, decrement shields by 1, print `con debilidad`
- Shields → 0: print `{name} entra en Breaking Point` immediately after the triggering hit
- While in Breaking Point: all damage ×1.5
- Breaking Point resets at round end (after beast acted or round over)
- If shields reach 0 mid-combo during a multi-hit attack, all subsequent hits in that exact same combo immediately benefit from the ×1.5 Breaking Point multiplier.

## Non-Obvious Behaviors

### Turn Queue
- `TurnQueueManager.SortRemainingQueue(queue)` is called in `CombatManager.HandleSingleTurn` after every action. It re-sorts within priority groups (recovery beasts → defenders → spearheads → normal → desprioritized) using `EffectiveSpeed`. This is how speed buffs/debuffs immediately affect the remaining turn order.
- `EffectiveSpeed` uses `minRemainingRounds: 1` — includes freshly applied buffs. `EffectiveSpeedNextRound` uses `minRemainingRounds: 2` — only includes buffs that will persist past the current round tick.
- Defender classification: `HasPriorityNextRound == true && DefendedLastRound == true`. Spearhead: `HasPriorityNextRound == true && DefendedLastRound == false`. Defenders always have both flags set because `SetDefending()` + `SetPriorityNextRound()` are always called together.
- `ConsumeTurnStartStates()` resets both `HasPriorityNextRound` and `DefendedLastRound` when a unit's turn STARTS (not at round start). So remaining units in the queue still carry their correct priority flags.

### BP & SP
- BP recovery: +1 per round per traveler, at round end, max 5 — **skipped for travelers who spent BP that round** (`SpentBpThisRound` flag)
- BP is spent when used; dead units still count toward team BP total (Steorra's Prophecy)
- **BP prompt is completely omitted** (no output, no input read) when `currentBp == 0`

### Status Effects
- **Re-application extends duration additively** (`existing.RemainingRounds += newEffect.RemainingRounds`); never resets or duplicates the effect slot.
- `TickStatusEffects()` is called in `EndOfRoundProcessing` AFTER all turns of the round; decrements by 1 and removes effects with 0 remaining rounds.
- **Buff/debuff multipliers apply to the final damage result**, not to raw stats: `floor(baseRaw × contextMult × AttackMult / DefenseMult)`. "Decreased Defense" on a target → DefenseMult = 2/3 → dividing by 2/3 is ×1.5 effective damage increase.
- `BuffSkillHandler` shows ALL effect messages first (for all targets), then applies all effects. This separates display from state mutation.

### Beast Skills
- `BeastSkill.IsNonDamaging` is true when `Modifier == 0 && !IsVortalClaw`. These skills (Consume Armor, Acid Spray, Augmented Magic) must NOT be silently skipped — they apply effects.
- `BeastTurnController` constructor requires `List<Beast> enemyTeam` (for Augmented Magic which buffs all alive beasts).
- Beast target selection for pure-debuff skills (`Consume Armor`, `Acid Spray`) still uses `TargetCriteria` parsed from the description — same logic as offensive skills.
- Beast self-buff (Gather Strength, Flap): applied and shown AFTER the hit loop, BEFORE the final HP line.
- Beast AoE + per-target debuff (Volcano): for each target — damage hit, then debuff for THAT target, then next target; final HPs shown after all targets.

### Other
- `TravelerAction` enum must live in its own file, not inside `CombatManager`
- Data files: `characters.json`, `enemies.json`, `skills.json`, `passive_skills.json`, `beast_skills.json` — all in `data/` relative to executable
- Beast `CurrentShields` (not `Shields`) shown in game state
- If unit dies mid-multi-hit: continue showing all remaining hits at normal damage, unit stays at HP 0
- Patience extra turns follow board order, not speed order

## Token Efficiency & Core Behavior
- Think before acting. Read existing files and this CLAUDE.md before writing code.
- Be concise in output but thorough in reasoning.
- Do not re-read files you have already read unless the file may have changed.
- Skip files over 100KB unless explicitly required.
- Test your code before declaring done.
- Keep solutions simple and direct.
