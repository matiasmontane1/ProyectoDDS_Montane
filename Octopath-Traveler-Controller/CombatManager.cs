using Octopath_Traveler_View;
using Octopath_Traveler.Models;

namespace Octopath_Traveler;

public class CombatManager
{
    private readonly View _view;
    private readonly List<Traveler> _playerTeam;
    private readonly List<Beast> _enemyTeam;
    private readonly List<ActiveSkill> _activeSkills;
    private readonly List<BeastSkill> _beastSkills;
    private int _currentRound;

    private const string Separator = "----------------------------------------";
    private const double BasicAttackModifier = 1.3;
    private const double WeaknessBonus = 0.5;
    private const double BreakingPointBonus = 0.5;
    private const int LegHoldDuration = 2;

    private enum TravelerAction { BasicAttack = 1, UseSkill = 2, Defend = 3, Flee = 4 }

    public CombatManager(
        View view,
        List<Traveler> playerTeam,
        List<Beast> enemyTeam,
        List<ActiveSkill> activeSkills,
        List<BeastSkill> beastSkills)
    {
        _view = view;
        _playerTeam = playerTeam;
        _enemyTeam = enemyTeam;
        _activeSkills = activeSkills;
        _beastSkills = beastSkills;
        _currentRound = 1;

        ApplyPassiveSkills();
        foreach (var traveler in _playerTeam) traveler.InitializeState();
        foreach (var beast in _enemyTeam) beast.InitializeState();
    }

    public void StartCombat()
    {
        while (true)
        {
            string winnerMessage = GetWinnerMessage();
            if (winnerMessage != null)
            {
                PrintMessageWithSeparator(winnerMessage);
                return;
            }

            _view.WriteLine(Separator);
            _view.WriteLine($"INICIA RONDA {_currentRound}");

            var turnQueue = GenerateTurnQueue().Where(u => !u.IsDead).ToList();

            if (HandleTurns(turnQueue)) return;

            EndOfRoundProcessing();
            _currentRound++;
        }
    }

    // ─── Passive Skills ────────────────────────────────────────────────────────

    private void ApplyPassiveSkills()
    {
        foreach (var traveler in _playerTeam)
        {
            foreach (var passive in traveler.PassiveSkills ?? new List<string>())
                ApplyPassive(traveler.Stats, passive);
        }
    }

    private static void ApplyPassive(Stats stats, string passive)
    {
        switch (passive)
        {
            case "Elemental Augmentation": stats.ElemAtk += 50; break;
            case "Summon Strength":        stats.PhysAtk += 50; break;
            case "Hale and Hearty":        stats.HP      += 500; break;
            case "Fleefoot":               stats.Speed   += 50; break;
            case "Inner Strength":         stats.SP      += 50; break;
        }
    }

    // ─── Turn Loop ─────────────────────────────────────────────────────────────

    private bool HandleTurns(List<Unit> turnQueue)
    {
        while (turnQueue.Count > 0)
        {
            string winnerMessage = GetWinnerMessage();
            if (winnerMessage != null)
            {
                PrintMessageWithSeparator(winnerMessage);
                return true;
            }

            if (HandleSingleTurn(turnQueue)) return true;
        }
        return false;
    }

    private bool HandleSingleTurn(List<Unit> turnQueue)
    {
        var currentUnit = turnQueue[0];

        if (currentUnit.IsDead)
        {
            turnQueue.RemoveAt(0);
            return false;
        }

        PrintGameState();
        PrintTurnOrder(turnQueue, "Turnos de la ronda");
        PrintTurnOrder(GenerateNextRoundPreview(turnQueue), "Turnos de la siguiente ronda");

        bool fled = false;
        if (currentUnit is Traveler traveler)
            fled = HandleTravelerTurn(traveler, turnQueue);
        else if (currentUnit is Beast beast)
            HandleBeastTurn(beast);

        turnQueue.RemoveAt(0);
        turnQueue.RemoveAll(u => u.IsDead);
        RemoveBreakingBeastsFromQueue(turnQueue);

        return fled;
    }

    private static void RemoveBreakingBeastsFromQueue(List<Unit> queue)
    {
        queue.RemoveAll(u => u is Beast b && b.IsInBreakingPoint && !b.JustRecoveredFromBreakingPoint);
    }

    private void EndOfRoundProcessing()
    {
        foreach (var traveler in _playerTeam.Where(t => !t.IsDead))
        {
            traveler.RecoverBP();
            traveler.IsDefending = false;
        }

        foreach (var beast in _enemyTeam.Where(b => !b.IsDead))
        {
            beast.DecrementBreakingPoint();
            beast.DecrementLegHold();
        }
    }

    // ─── Turn Queue ────────────────────────────────────────────────────────────

    private List<Unit> GenerateTurnQueue()
    {
        var recoveryFirst = _enemyTeam
            .Where(b => !b.IsDead && b.JustRecoveredFromBreakingPoint)
            .Select(u => new { Unit = (Unit)u, IsTraveler = false, Index = _enemyTeam.IndexOf(u) })
            .OrderByDescending(x => x.Unit.Stats.Speed)
            .ThenBy(x => x.Index);

        var defenderFirst = _playerTeam.Cast<Unit>()
            .Where(u => u is Traveler t && t.IsDefenderNextRound && !u.IsDead)
            .Select(u => new { Unit = u, IsTraveler = true, Index = _playerTeam.IndexOf((Traveler)u) })
            .OrderByDescending(x => x.Unit.Stats.Speed)
            .ThenBy(x => x.Index);

        var spearheadFirst = _playerTeam.Cast<Unit>()
            .Where(u => u is Traveler t && t.SpearheadNextRound && !t.IsDefenderNextRound && !u.IsDead)
            .Select(u => new { Unit = u, IsTraveler = true, Index = _playerTeam.IndexOf((Traveler)u) })
            .OrderByDescending(x => x.Unit.Stats.Speed)
            .ThenBy(x => x.Index);

        var normal = _playerTeam.Cast<Unit>()
            .Where(u => !(u is Traveler t && t.SpearheadNextRound) && !u.IsDead)
            .Select(u => new { Unit = u, IsTraveler = true, Index = _playerTeam.IndexOf((Traveler)u) })
            .Concat(_enemyTeam.Where(b => !b.IsDead && !b.IsInBreakingPoint && !b.IsLegHolded && !b.JustRecoveredFromBreakingPoint)
                .Select(u => new { Unit = (Unit)u, IsTraveler = false, Index = _enemyTeam.IndexOf(u) }))
            .OrderByDescending(x => x.Unit.Stats.Speed)
            .ThenByDescending(x => x.IsTraveler)
            .ThenBy(x => x.Index);

        var legholded = _enemyTeam.Where(b => !b.IsDead && !b.IsInBreakingPoint && b.IsLegHolded && !b.JustRecoveredFromBreakingPoint)
            .Select(u => new { Unit = (Unit)u, IsTraveler = false, Index = _enemyTeam.IndexOf(u) })
            .OrderByDescending(x => x.Unit.Stats.Speed)
            .ThenBy(x => x.Index);

        return recoveryFirst
            .Concat(defenderFirst)
            .Concat(spearheadFirst)
            .Concat(normal)
            .Concat(legholded)
            .Select(x => x.Unit)
            .ToList();
    }

    private List<Unit> GenerateNextRoundPreview(List<Unit> currentQueue)
    {
        // Beasts with BreakingPointRoundsRemaining==1 will recover at end of current round → go first next round
        var recoveryFirst = _enemyTeam
            .Where(b => !b.IsDead && b.BreakingPointRoundsRemaining == 1)
            .Select(u => new { Unit = (Unit)u, IsTraveler = false, Index = _enemyTeam.IndexOf(u) })
            .OrderByDescending(x => x.Unit.Stats.Speed)
            .ThenBy(x => x.Index);

        // Only travelers who already acted this round have stable IsDefenderNextRound/SpearheadNextRound
        var defenderFirst = _playerTeam.Cast<Unit>()
            .Where(u => u is Traveler t && t.IsDefenderNextRound && !u.IsDead && !currentQueue.Contains(u))
            .Select(u => new { Unit = u, IsTraveler = true, Index = _playerTeam.IndexOf((Traveler)u) })
            .OrderByDescending(x => x.Unit.Stats.Speed)
            .ThenBy(x => x.Index);

        var spearheadFirst = _playerTeam.Cast<Unit>()
            .Where(u => u is Traveler t && t.SpearheadNextRound && !t.IsDefenderNextRound && !u.IsDead && !currentQueue.Contains(u))
            .Select(u => new { Unit = u, IsTraveler = true, Index = _playerTeam.IndexOf((Traveler)u) })
            .OrderByDescending(x => x.Unit.Stats.Speed)
            .ThenBy(x => x.Index);

        var normal = _playerTeam.Cast<Unit>()
            .Where(u => !(u is Traveler t && t.SpearheadNextRound && !currentQueue.Contains(u)) && !u.IsDead)
            .Select(u => new { Unit = u, IsTraveler = true, Index = _playerTeam.IndexOf((Traveler)u) })
            .Concat(_enemyTeam
                .Where(b => !b.IsDead && b.BreakingPointRoundsRemaining == 0 && !(b.IsLegHolded && b.LegHoldRoundsRemaining > 1))
                .Select(u => new { Unit = (Unit)u, IsTraveler = false, Index = _enemyTeam.IndexOf(u) }))
            .OrderByDescending(x => x.Unit.Stats.Speed)
            .ThenByDescending(x => x.IsTraveler)
            .ThenBy(x => x.Index);

        var legholded = _enemyTeam
            .Where(b => !b.IsDead && b.BreakingPointRoundsRemaining == 0 && b.IsLegHolded && b.LegHoldRoundsRemaining > 1)
            .Select(u => new { Unit = (Unit)u, IsTraveler = false, Index = _enemyTeam.IndexOf(u) })
            .OrderByDescending(x => x.Unit.Stats.Speed)
            .ThenBy(x => x.Index);

        return recoveryFirst
            .Concat(defenderFirst)
            .Concat(spearheadFirst)
            .Concat(normal)
            .Concat(legholded)
            .Select(x => x.Unit)
            .ToList();
    }

    // ─── Traveler Turn ─────────────────────────────────────────────────────────

    private bool HandleTravelerTurn(Traveler traveler, List<Unit> turnQueue)
    {
        if (traveler.IsDead) 
            return false;
        
        traveler.IsDefending = false;
        
        traveler.SpearheadNextRound = false;
        traveler.IsDefenderNextRound = false;

        while (true)
        {
            _view.WriteLine(Separator);
            _view.WriteLine($"Turno de {traveler.Name}");
            _view.WriteLine("1: Ataque básico");
            _view.WriteLine("2: Usar habilidad");
            _view.WriteLine("3: Defender");
            _view.WriteLine("4: Huir");

            string input = _view.ReadLine();

            if (!Enum.TryParse<TravelerAction>(input, out TravelerAction action) || !Enum.IsDefined(action))
                continue;

            switch (action)
            {
                case TravelerAction.BasicAttack:
                    if (HandleBasicAttack(traveler)) return false;
                    break;
                case TravelerAction.UseSkill:
                    if (HandleSkillMenu(traveler, turnQueue)) return false;
                    break;
                case TravelerAction.Defend:
                    traveler.IsDefending = true;
                    traveler.SpearheadNextRound = true;
                    traveler.IsDefenderNextRound = true;
                    return false;
                case TravelerAction.Flee:
                    PrintMessageWithSeparator("El equipo de viajeros ha huido!");
                    _view.WriteLine(Separator);
                    _view.WriteLine("Gana equipo del enemigo");
                    return true;
            }
        }
    }

    private bool HandleBasicAttack(Traveler traveler)
    {
        string weapon = PromptWeaponSelection(traveler);
        if (weapon == null) return false;

        var aliveEnemies = _enemyTeam.Where(e => !e.IsDead).ToList();
        Beast target = PromptBeastTargetSelection(traveler, aliveEnemies);
        if (target == null) return false;

        PromptBPUsage(traveler);
        ExecuteBasicAttack(traveler, target, weapon);
        return true;
    }

    private void ExecuteBasicAttack(Traveler traveler, Beast target, string weapon)
    {
        double baseRaw = CalculatePhysicalBase(traveler.Stats.PhysAtk, BasicAttackModifier, target.Stats.PhysDef);
        bool isWeakness = target.Weaknesses.Contains(weapon);
        bool wasInBreakingPoint = target.IsInBreakingPoint;

        int damage = ApplyDamageMultipliers(baseRaw, isWeakness, wasInBreakingPoint);

        target.TakeDamage(damage);

        _view.WriteLine(Separator);
        _view.WriteLine($"{traveler.Name} ataca");

        string weaknessSuffix = isWeakness ? " con debilidad" : "";
        _view.WriteLine($"{target.Name} recibe {damage} de daño de tipo {weapon}{weaknessSuffix}");

        if (isWeakness && damage > 0)
            HandleWeaknessHit(target);

        _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");
    }

    private void HandleWeaknessHit(Beast target)
    {
        if (target.IsInBreakingPoint) return;

        target.DecrementShield();

        if (target.CurrentShields == 0)
        {
            target.TriggerBreakingPoint();
            _view.WriteLine($"{target.Name} entra en Breaking Point");
        }
    }

    // ─── Skill Menu ────────────────────────────────────────────────────────────

    private bool HandleSkillMenu(Traveler traveler, List<Unit> turnQueue)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"Seleccione una habilidad para {traveler.Name}");

        var allSkills = traveler.Skills ?? new List<string>();
        var skills = allSkills
            .Where(name => _activeSkills.FirstOrDefault(s => s.Name == name) is ActiveSkill sk && traveler.CurrentSP >= sk.SP)
            .ToList();
        for (int i = 0; i < skills.Count; i++)
            _view.WriteLine($"{i + 1}: {skills[i]}");
        _view.WriteLine($"{skills.Count + 1}: Cancelar");

        string input = _view.ReadLine();
        if (!int.TryParse(input, out int choice) || choice < 1 || choice > skills.Count)
            return false;

        string skillName = skills[choice - 1];
        ActiveSkill skill = _activeSkills.FirstOrDefault(s => s.Name == skillName);
        if (skill == null) return false;

        return ExecuteSkill(traveler, skill, turnQueue);
    }

    private bool ExecuteSkill(Traveler traveler, ActiveSkill skill, List<Unit> turnQueue)
    {
        switch (skill.Name)
        {
            case "Leghold Trap":
                return HandleLegHoldTrap(traveler, skill, turnQueue);
            case "Spearhead":
                return HandleSpearhead(traveler, skill);
            case "Revive":
                return HandleReviveParty(traveler, skill);
            case "Vivify":
                return HandleVivify(traveler, skill);
            case "Healing Touch":
                return HandleHealingTouch(traveler, skill);
        }

        if (skill.IsShootingStars)
            return HandleShootingStars(traveler, skill);

        if (skill.IsNightmareChimera)
            return HandleNightmareChimera(traveler, skill);

        if (skill.IsOffensive)
            return HandleOffensiveSkill(traveler, skill);

        if (skill.Modifier > 0 && (skill.Target == "Party" || skill.Target == "Ally"))
            return HandleHealSkill(traveler, skill);

        // Unsupported skill: still show menu, consume turn
        return false;
    }

    // ─── Offensive Skills ──────────────────────────────────────────────────────

    private bool HandleOffensiveSkill(Traveler traveler, ActiveSkill skill)
    {
        List<Beast> targets;

        if (skill.Target == "Single")
        {
            var aliveEnemies = _enemyTeam.Where(e => !e.IsDead).ToList();
            Beast target = PromptBeastTargetSelection(traveler, aliveEnemies);
            if (target == null) return false;
            targets = new List<Beast> { target };
        }
        else // Enemies
        {
            targets = _enemyTeam.Where(e => !e.IsDead).ToList();
        }

        PromptBPUsage(traveler);
        traveler.SpendSP(skill.SP);

        _view.WriteLine(Separator);
        _view.WriteLine($"{traveler.Name} usa {skill.Name}");

        var damageResults = new List<(Beast beast, int damage, bool isWeakness)>();

        foreach (var target in targets)
        {
            bool wasInBP = target.IsInBreakingPoint;
            int damage;
            bool isWeakness = !string.IsNullOrEmpty(skill.Type) && target.Weaknesses.Contains(skill.Type);

            if (skill.IsPhysicalOffensive)
            {
                double baseRaw = CalculatePhysicalBase(traveler.Stats.PhysAtk, skill.Modifier, target.Stats.PhysDef);
                if (skill.HasLastStandMechanic)
                    baseRaw += ApplyLastStandBonus(baseRaw, traveler);
                damage = ApplyDamageMultipliers(baseRaw, isWeakness, wasInBP);
            }
            else
            {
                double baseRaw = CalculateElementalBase(traveler.Stats.ElemAtk, skill.Modifier, target.Stats.ElemDef);
                damage = ApplyDamageMultipliers(baseRaw, isWeakness, wasInBP);
            }

            if (skill.HasMercyStrikeMechanic)
                damage = Math.Min(damage, Math.Max(0, target.CurrentHP - 1));

            target.TakeDamage(damage);
            damageResults.Add((target, damage, isWeakness));

            string weaknessSuffix = isWeakness ? " con debilidad" : "";
            _view.WriteLine($"{target.Name} recibe {damage} de daño de tipo {skill.Type}{weaknessSuffix}");

            if (isWeakness && damage > 0 && !wasInBP)
            {
                target.DecrementShield();
                if (target.CurrentShields == 0)
                {
                    target.TriggerBreakingPoint();
                    _view.WriteLine($"{target.Name} entra en Breaking Point");
                }
            }
        }

        foreach (var (target, _, _) in damageResults)
            _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");

        return true;
    }

    // ─── Healing Skills ────────────────────────────────────────────────────────

    private bool HandleHealSkill(Traveler traveler, ActiveSkill skill)
    {
        if (skill.Target == "Ally")
        {
            var allTravelers = _playerTeam.Where(t => !t.IsDead).ToList();
            Traveler target = PromptAllyTargetSelection(traveler, allTravelers);
            if (target == null) return false;

            PromptBPUsage(traveler);
            traveler.SpendSP(skill.SP);

            int healAmount = CalculateHeal(traveler.Stats.ElemDef, skill.Modifier);

            _view.WriteLine(Separator);
            _view.WriteLine($"{traveler.Name} usa {skill.Name}");
            _view.WriteLine($"{target.Name} recupera {healAmount} de vida");
            target.Heal(healAmount);
            _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");
        }
        else // Party
        {
            PromptBPUsage(traveler);
            traveler.SpendSP(skill.SP);

            int healAmount = CalculateHeal(traveler.Stats.ElemDef, skill.Modifier);
            var aliveTravelers = _playerTeam.Where(t => !t.IsDead).ToList();
            var others = aliveTravelers.Where(t => t != traveler).ToList();

            _view.WriteLine(Separator);
            _view.WriteLine($"{traveler.Name} usa {skill.Name}");

            var healTargets = others.Concat(new[] { traveler }).ToList();
            foreach (var target in healTargets)
            {
                _view.WriteLine($"{target.Name} recupera {healAmount} de vida");
                target.Heal(healAmount);
            }
            foreach (var target in healTargets)
                _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");
        }

        return true;
    }

    private bool HandleReviveParty(Traveler traveler, ActiveSkill skill)
    {
        PromptBPUsage(traveler);
        traveler.SpendSP(skill.SP);

        var deadTravelers = _playerTeam.Where(t => t.IsDead).ToList();

        _view.WriteLine(Separator);
        _view.WriteLine($"{traveler.Name} usa {skill.Name}");

        foreach (var target in deadTravelers)
        {
            target.Revive();
            _view.WriteLine($"{target.Name} revive");
        }
        foreach (var target in deadTravelers)
            _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");

        return true;
    }

    private bool HandleVivify(Traveler traveler, ActiveSkill skill)
    {
        var deadAllies = _playerTeam.Where(t => t.IsDead).ToList();
        Traveler target = PromptAllyTargetSelection(traveler, deadAllies);
        if (target == null) return false;

        PromptBPUsage(traveler);
        traveler.SpendSP(skill.SP);

        int healAmount = CalculateHeal(traveler.Stats.ElemDef, skill.Modifier);

        _view.WriteLine(Separator);
        _view.WriteLine($"{traveler.Name} usa {skill.Name}");

        if (target.IsDead)
        {
            target.Revive();
            _view.WriteLine($"{target.Name} revive");
        }

        _view.WriteLine($"{target.Name} recupera {healAmount} de vida");
        target.Heal(healAmount);
        _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");

        return true;
    }

    private bool HandleHealingTouch(Traveler traveler, ActiveSkill skill)
    {
        var allTravelers = _playerTeam.ToList();
        Traveler target = PromptAllyTargetSelection(traveler, allTravelers);
        if (target == null) return false;

        PromptBPUsage(traveler);
        traveler.SpendSP(skill.SP);

        int healAmount = CalculateHeal(traveler.Stats.ElemDef, skill.Modifier);

        _view.WriteLine(Separator);
        _view.WriteLine($"{traveler.Name} usa {skill.Name}");

        if (target.IsDead)
        {
            target.Revive();
            _view.WriteLine($"{target.Name} revive");
        }

        _view.WriteLine($"{target.Name} recupera {healAmount} de vida");
        target.Heal(healAmount);
        _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");

        return true;
    }

    // ─── Special Skills ────────────────────────────────────────────────────────

    private bool HandleLegHoldTrap(Traveler traveler, ActiveSkill skill, List<Unit> turnQueue)
    {
        var aliveEnemies = _enemyTeam.Where(e => !e.IsDead).ToList();
        Beast target = PromptBeastTargetSelection(traveler, aliveEnemies);
        if (target == null) return false;

        PromptBPUsage(traveler);
        traveler.SpendSP(skill.SP);

        target.ApplyLegHold(LegHoldDuration);

        // Move leholded beast to end of current round queue if still there
        if (turnQueue.Contains(target))
        {
            turnQueue.Remove(target);
            turnQueue.Add(target);
        }

        _view.WriteLine(Separator);
        _view.WriteLine($"{traveler.Name} usa {skill.Name}");
        _view.WriteLine($"{target.Name} tendrá menor prioridad de turno durante 2 rondas");

        return true;
    }

    private bool HandleSpearhead(Traveler traveler, ActiveSkill skill)
    {
        var aliveEnemies = _enemyTeam.Where(e => !e.IsDead).ToList();
        Beast target = PromptBeastTargetSelection(traveler, aliveEnemies);
        if (target == null) return false;

        PromptBPUsage(traveler);
        traveler.SpendSP(skill.SP);

        bool wasInBP = target.IsInBreakingPoint;
        bool isWeakness = target.Weaknesses.Contains(skill.Type);
        double baseRaw = CalculatePhysicalBase(traveler.Stats.PhysAtk, skill.Modifier, target.Stats.PhysDef);
        int damage = ApplyDamageMultipliers(baseRaw, isWeakness, wasInBP);

        target.TakeDamage(damage);
        traveler.SpearheadNextRound = true;

        _view.WriteLine(Separator);
        _view.WriteLine($"{traveler.Name} usa {skill.Name}");

        string weaknessSuffix = isWeakness ? " con debilidad" : "";
        _view.WriteLine($"{target.Name} recibe {damage} de daño de tipo {skill.Type}{weaknessSuffix}");

        if (isWeakness && damage > 0 && !wasInBP)
        {
            target.DecrementShield();
            if (target.CurrentShields == 0)
            {
                target.TriggerBreakingPoint();
                _view.WriteLine($"{target.Name} entra en Breaking Point");
            }
        }

        _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");

        return true;
    }

    private bool HandleShootingStars(Traveler traveler, ActiveSkill skill)
    {
        PromptBPUsage(traveler);
        traveler.SpendSP(skill.SP);

        var targets = _enemyTeam.Where(e => !e.IsDead).ToList();
        var types = new[] { "Wind", "Light", "Dark" };

        _view.WriteLine(Separator);
        _view.WriteLine($"{traveler.Name} usa {skill.Name}");

        var damagePerTarget = new List<(Beast beast, int totalDamage)>();

        foreach (var target in targets)
        {
            int totalDamage = 0;
            foreach (var hitType in types)
            {
                bool wasInBP = target.IsInBreakingPoint;
                bool isWeakness = target.Weaknesses.Contains(hitType);
                double baseRaw = CalculateElementalBase(traveler.Stats.ElemAtk, skill.Modifier, target.Stats.ElemDef);
                int damage = ApplyDamageMultipliers(baseRaw, isWeakness, wasInBP);
                target.TakeDamage(damage);
                totalDamage += damage;

                string weaknessSuffix = isWeakness ? " con debilidad" : "";
                _view.WriteLine($"{target.Name} recibe {damage} de daño de tipo {hitType}{weaknessSuffix}");

                if (isWeakness && damage > 0 && !wasInBP)
                {
                    target.DecrementShield();
                    if (target.CurrentShields == 0)
                    {
                        target.TriggerBreakingPoint();
                        _view.WriteLine($"{target.Name} entra en Breaking Point");
                    }
                }
            }
            damagePerTarget.Add((target, totalDamage));
        }

        foreach (var (target, _) in damagePerTarget)
            _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");

        return true;
    }

    private bool HandleNightmareChimera(Traveler traveler, ActiveSkill skill)
    {
        var weapons = new[] { "Sword", "Spear", "Dagger", "Axe", "Bow", "Stave" };

        _view.WriteLine(Separator);
        _view.WriteLine("Seleccione un arma");
        for (int i = 0; i < weapons.Length; i++)
            _view.WriteLine($"{i + 1}: {weapons[i]}");
        _view.WriteLine($"{weapons.Length + 1}: Cancelar");

        if (!int.TryParse(_view.ReadLine(), out int weaponChoice) || weaponChoice < 1 || weaponChoice > weapons.Length)
            return false;

        string chosenWeapon = weapons[weaponChoice - 1];

        var aliveEnemies = _enemyTeam.Where(e => !e.IsDead).ToList();
        Beast target = PromptBeastTargetSelection(traveler, aliveEnemies);
        if (target == null) return false;

        PromptBPUsage(traveler);
        traveler.SpendSP(skill.SP);

        bool wasInBP = target.IsInBreakingPoint;
        bool isWeakness = target.Weaknesses.Contains(chosenWeapon);
        double baseRaw = CalculatePhysicalBase(traveler.Stats.PhysAtk, skill.Modifier, target.Stats.PhysDef);
        int damage = ApplyDamageMultipliers(baseRaw, isWeakness, wasInBP);
        target.TakeDamage(damage);

        _view.WriteLine(Separator);
        _view.WriteLine($"{traveler.Name} usa {skill.Name}");

        string weaknessSuffix = isWeakness ? " con debilidad" : "";
        _view.WriteLine($"{target.Name} recibe {damage} de daño de tipo {chosenWeapon}{weaknessSuffix}");

        if (isWeakness && damage > 0 && !wasInBP)
        {
            target.DecrementShield();
            if (target.CurrentShields == 0)
            {
                target.TriggerBreakingPoint();
                _view.WriteLine($"{target.Name} entra en Breaking Point");
            }
        }

        _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");
        return true;
    }

    // ─── Beast Turn ────────────────────────────────────────────────────────────

    private void HandleBeastTurn(Beast beast)
    {
        beast.ClearRecovery();
        var skill = _beastSkills.FirstOrDefault(s => s.Name == beast.Skill);
        if (skill == null) return;

        _view.WriteLine(Separator);
        _view.WriteLine($"{beast.Name} usa {beast.Skill}");

        if (skill.IsVortalClaw)
        {
            ExecuteVortalClaw(beast);
            return;
        }

        if (skill.IsNonDamaging) return;

        if (skill.IsAoe)
            ExecuteBeastAoe(beast, skill);
        else
            ExecuteBeastSingle(beast, skill);
    }

    private void ExecuteVortalClaw(Beast beast)
    {
        var targets = _playerTeam.Where(t => !t.IsDead).ToList();
        var results = new List<(Traveler target, int damage)>();

        foreach (var target in targets)
        {
            int newHp = (int)Math.Floor(target.CurrentHP / 2.0);
            int damage = target.CurrentHP - newHp;
            target.TakeDamage(damage);
            results.Add((target, damage));
            _view.WriteLine($"{target.Name} recibe {damage} de daño");
        }
        foreach (var (target, _) in results)
            _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");
    }

    private void ExecuteBeastAoe(Beast beast, BeastSkill skill)
    {
        var targets = _playerTeam.Where(t => !t.IsDead).ToList();
        var results = new List<(Traveler traveler, int damage)>();

        foreach (var target in targets)
        {
            int damage = CalculateBeastDamage(beast, skill, target);

            if (target.IsDefending)
            {
                _view.WriteLine($"{target.Name} se defiende");
                damage = (int)Math.Floor(damage / 2.0);
            }

            target.TakeDamage(damage);
            results.Add((target, damage));

            string dmgType = skill.IsPhysical ? "físico" : "elemental";
            _view.WriteLine($"{target.Name} recibe {damage} de daño {dmgType}");
        }

        foreach (var (target, _) in results)
            _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");
    }

    private void ExecuteBeastSingle(Beast beast, BeastSkill skill)
    {
        var target = SelectBeastTarget(skill);
        if (target == null) return;

        int damage = CalculateBeastDamage(beast, skill, target);

        if (target.IsDefending)
        {
            _view.WriteLine($"{target.Name} se defiende");
            damage = (int)Math.Floor(damage / 2.0);
        }

        target.TakeDamage(damage);

        string dmgType = skill.IsPhysical ? "físico" : "elemental";
        _view.WriteLine($"{target.Name} recibe {damage} de daño {dmgType}");
        _view.WriteLine($"{target.Name} termina con HP:{target.CurrentHP}");
    }

    private int CalculateBeastDamage(Beast beast, BeastSkill skill, Traveler target)
    {
        double raw = skill.IsPhysical
            ? CalculatePhysicalBase(beast.Stats.PhysAtk, skill.Modifier, target.Stats.PhysDef)
            : CalculateElementalBase(beast.Stats.ElemAtk, skill.Modifier, target.Stats.ElemDef);
        return (int)Math.Floor(raw);
    }

    private Traveler SelectBeastTarget(BeastSkill skill)
    {
        var alive = _playerTeam.Where(t => !t.IsDead).ToList();
        if (alive.Count == 0) return null;

        return skill.TargetCriteria switch
        {
            "MaxHP"      => alive.OrderByDescending(t => t.CurrentHP).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MaxElemAtk" => alive.OrderByDescending(t => t.Stats.ElemAtk).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MinPhysDef" => alive.OrderBy(t => t.Stats.PhysDef).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MaxSpeed"   => alive.OrderByDescending(t => t.Stats.Speed).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MinElemDef" => alive.OrderBy(t => t.Stats.ElemDef).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MaxPhysDef" => alive.OrderByDescending(t => t.Stats.PhysDef).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MaxPhysAtk" => alive.OrderByDescending(t => t.Stats.PhysAtk).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            "MinSpeed"   => alive.OrderBy(t => t.Stats.Speed).ThenBy(t => _playerTeam.IndexOf(t)).First(),
            _ => alive.OrderByDescending(t => t.CurrentHP).ThenBy(t => _playerTeam.IndexOf(t)).First(),
        };
    }

    // ─── Prompts ───────────────────────────────────────────────────────────────

    private string PromptWeaponSelection(Traveler traveler)
    {
        _view.WriteLine(Separator);
        _view.WriteLine("Seleccione un arma");

        for (int i = 0; i < traveler.Weapons.Count; i++)
            _view.WriteLine($"{i + 1}: {traveler.Weapons[i]}");
        _view.WriteLine($"{traveler.Weapons.Count + 1}: Cancelar");

        if (int.TryParse(_view.ReadLine(), out int choice) && choice > 0 && choice <= traveler.Weapons.Count)
            return traveler.Weapons[choice - 1];

        return null;
    }

    private Beast PromptBeastTargetSelection(Traveler traveler, List<Beast> targets)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"Seleccione un objetivo para {traveler.Name}");

        for (int i = 0; i < targets.Count; i++)
        {
            var e = targets[i];
            _view.WriteLine($"{i + 1}: {e.Name} - HP:{e.CurrentHP}/{e.Stats.HP} Shields:{e.CurrentShields}");
        }
        _view.WriteLine($"{targets.Count + 1}: Cancelar");

        if (int.TryParse(_view.ReadLine(), out int choice) && choice > 0 && choice <= targets.Count)
            return targets[choice - 1];

        return null;
    }

    private Traveler PromptAllyTargetSelection(Traveler traveler, List<Traveler> targets)
    {
        _view.WriteLine(Separator);
        _view.WriteLine($"Seleccione un objetivo para {traveler.Name}");

        for (int i = 0; i < targets.Count; i++)
        {
            var t = targets[i];
            _view.WriteLine($"{i + 1}: {t.Name} - HP:{t.CurrentHP}/{t.Stats.HP} SP:{t.CurrentSP}/{t.Stats.SP} BP:{t.CurrentBP}");
        }
        _view.WriteLine($"{targets.Count + 1}: Cancelar");

        if (int.TryParse(_view.ReadLine(), out int choice) && choice > 0 && choice <= targets.Count)
            return targets[choice - 1];

        return null;
    }

    private void PromptBPUsage(Traveler traveler)
    {
        if (traveler.CurrentBP >= 1)
        {
            _view.WriteLine(Separator);
            _view.WriteLine("Seleccione cuantos BP utilizar");
            _view.ReadLine();
        }
    }

    // ─── Damage Formulas ───────────────────────────────────────────────────────

    private static double CalculatePhysicalBase(int physAtk, double modifier, int physDef)
    {
        return Math.Max(0.0, physAtk * modifier - physDef);
    }

    private static double CalculateElementalBase(int elemAtk, double modifier, int elemDef)
    {
        return Math.Max(0.0, elemAtk * modifier - elemDef);
    }

    private static int CalculateHeal(int elemDef, double modifier)
    {
        return (int)Math.Floor(elemDef * modifier);
    }

    private static double ApplyLastStandBonus(double baseRaw, Traveler traveler)
    {
        // La división entre enteros trunca automáticamente los decimales hacia cero
        // cumpliendo la regla de "(porcentaje truncado)" de manera perfecta.
        int missingPct = (traveler.Stats.HP - traveler.CurrentHP) * 100 / traveler.Stats.HP;
        
        // Retornamos un double para no perder precisión en el multiplicador de daño
        // antes de que pase al ApplyDamageMultipliers final.
        return baseRaw * missingPct * 0.03;
    }

    private static int ApplyDamageMultipliers(double baseRaw, bool isWeakness, bool isBreakingPoint)
    {
        double multiplier = 1.0
            + (isWeakness ? WeaknessBonus : 0)
            + (isBreakingPoint ? BreakingPointBonus : 0);
        return (int)Math.Floor(baseRaw * multiplier);
    }

    // ─── Display ───────────────────────────────────────────────────────────────

    private void PrintGameState()
    {
        _view.WriteLine(Separator);
        _view.WriteLine("Equipo del jugador");
        for (int i = 0; i < _playerTeam.Count; i++)
        {
            var t = _playerTeam[i];
            char letter = (char)('A' + i);
            _view.WriteLine($"{letter}-{t.Name} - HP:{t.CurrentHP}/{t.Stats.HP} SP:{t.CurrentSP}/{t.Stats.SP} BP:{t.CurrentBP}");
        }

        _view.WriteLine("Equipo del enemigo");
        for (int i = 0; i < _enemyTeam.Count; i++)
        {
            var b = _enemyTeam[i];
            char letter = (char)('A' + i);
            _view.WriteLine($"{letter}-{b.Name} - HP:{b.CurrentHP}/{b.Stats.HP} Shields:{b.CurrentShields}");
        }
    }

    private void PrintTurnOrder(List<Unit> queue, string title)
    {
        _view.WriteLine(Separator);
        _view.WriteLine(title);
        for (int i = 0; i < queue.Count; i++)
            _view.WriteLine($"{i + 1}.{queue[i].Name}");
    }

    private void PrintMessageWithSeparator(string message)
    {
        _view.WriteLine(Separator);
        _view.WriteLine(message);
    }

    private string GetWinnerMessage()
    {
        if (_enemyTeam.All(b => b.IsDead)) return "Gana equipo del jugador";
        if (_playerTeam.All(t => t.IsDead)) return "Gana equipo del enemigo";
        return null;
    }
}
