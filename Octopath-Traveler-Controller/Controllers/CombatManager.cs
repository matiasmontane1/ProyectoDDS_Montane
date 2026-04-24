using Octopath_Traveler.Models;
using Octopath_Traveler.Views;
using Octopath_Traveler_View;

namespace Octopath_Traveler.Controllers;

public class CombatManager
{
    private readonly CombatView _combatView;
    private readonly List<Traveler> _playerTeam;
    private readonly List<Beast> _enemyTeam;
    private readonly List<ActiveSkill> _activeSkills;
    private readonly BeastTurnController _beastTurnController;
    private readonly TurnQueueManager _queueManager;
    private int _currentRound;

    private const double BasicAttackModifier = 1.3;
    private const int DesprioritizationDuration = 2;

    private enum TravelerAction { BasicAttack = 1, UseSkill = 2, Defend = 3, Flee = 4 }

    public CombatManager(
        View view,
        List<Traveler> playerTeam,
        List<Beast> enemyTeam,
        List<ActiveSkill> activeSkills,
        List<BeastSkill> beastSkills)
    {
        _combatView = new CombatView(view);
        _playerTeam = playerTeam;
        _enemyTeam = enemyTeam;
        _activeSkills = activeSkills;
        _beastTurnController = new BeastTurnController(_combatView, playerTeam, beastSkills);
        _queueManager = new TurnQueueManager(_playerTeam, _enemyTeam);
        _currentRound = 1;

        InitializeCombatants();
    }

    private void InitializeCombatants()
    {
        foreach (var traveler in _playerTeam)
        {
            PassiveSkillApplicator.ApplyAll(traveler);
            traveler.InitializeState();
        }
        foreach (var beast in _enemyTeam) beast.InitializeState();
    }

    public void StartCombat()
    {
        while (true)
        {
            if (CheckAndAnnounceWinner()) return;

            _combatView.ShowRoundStart(_currentRound);

            var turnQueue = _queueManager.GenerateCurrentRoundQueue().Where(u => !u.IsDead).ToList();

            if (HandleTurns(turnQueue)) return;

            EndOfRoundProcessing();
            _currentRound++;
        }
    }

    private bool HandleTurns(List<Unit> turnQueue)
    {
        while (turnQueue.Count > 0)
        {
            if (CheckAndAnnounceWinner()) return true;
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

        _combatView.ShowGameState(_playerTeam, _enemyTeam);
        _combatView.ShowTurnOrder(turnQueue, "Turnos de la ronda");
        _combatView.ShowTurnOrder(_queueManager.GenerateNextRoundPreview(turnQueue), "Turnos de la siguiente ronda");

        bool fled = false;
        if (currentUnit is Traveler traveler)
            fled = HandleTravelerTurn(traveler, turnQueue);
        else if (currentUnit is Beast beast)
            _beastTurnController.HandleTurn(beast);

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
            traveler.RecoverBp();
            traveler.ResetDefenseForNewRound();
        }

        foreach (var beast in _enemyTeam.Where(b => !b.IsDead))
        {
            beast.DecrementBreakingPoint();
            beast.DecrementDesprioritization();
        }
    }


    private bool HandleTravelerTurn(Traveler traveler, List<Unit> turnQueue)
    {
        if (traveler.IsDead) return false;

        traveler.ConsumeTurnStartStates();

        while (true)
        {
            _combatView.ShowTravelerActionMenu(traveler.Name);
            string input = _combatView.ReadLine();

            if (!Enum.TryParse<TravelerAction>(input, out var action) || !Enum.IsDefined(action))
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
                    traveler.SetDefending();
                    traveler.SetPriorityNextRound();
                    return false;
                case TravelerAction.Flee:
                    _combatView.ShowFleeResult();
                    return true;
            }
        }
    }

    private bool HandleBasicAttack(Traveler traveler)
    {
        string? weapon = _combatView.PromptWeaponSelection(traveler.Weapons);
        if (weapon == null) return false;

        var aliveEnemies = _enemyTeam.Where(e => !e.IsDead).ToList();
        Beast? target = _combatView.PromptBeastTargetSelection(traveler.Name, aliveEnemies);
        if (target == null) return false;

        _combatView.PromptBpUsageIfAvailable(traveler.CurrentBp);
        ExecuteBasicAttack(traveler, target, weapon);
        return true;
    }

    private void ExecuteBasicAttack(Traveler traveler, Beast target, string weapon)
    {
        bool isWeakness = target.Weaknesses.Contains(weapon);
        int damage = DamageCalculator.CalculatePhysicalDamage(
            traveler.Stats.PhysicalAttack,
            BasicAttackModifier,
            target.Stats.PhysicalDefense,
            isWeakness,
            target.IsInBreakingPoint);

        target.TakeDamage(damage);

        _combatView.ShowTravelerAttacks(traveler.Name);
        _combatView.ShowDamageWithType(target.Name, damage, weapon, isWeakness);

        if (isWeakness && damage > 0)
            ProcessShieldDamage(target);

        _combatView.ShowFinalHp(target.Name, target.CurrentHp);
    }

    private void ProcessShieldDamage(Beast target)
    {
        if (target.IsInBreakingPoint) return;
        int shieldsBefore = target.CurrentShields;
        target.DecrementShield();
        if (shieldsBefore > 0 && target.CurrentShields == 0)
            _combatView.ShowBreakingPoint(target.Name);
    }


    private bool HandleSkillMenu(Traveler traveler, List<Unit> turnQueue)
    {
        var availableSkills = traveler.Skills
            .Where(name => _activeSkills.FirstOrDefault(s => s.Name == name) is ActiveSkill sk && traveler.CurrentSp >= sk.Sp)
            .ToList();

        string? skillName = _combatView.PromptSkillSelection(traveler.Name, availableSkills);
        if (skillName == null) return false;

        ActiveSkill? skill = _activeSkills.FirstOrDefault(s => s.Name == skillName);
        if (skill == null) return false;

        return ExecuteSkill(traveler, skill, turnQueue);
    }

    private bool ExecuteSkill(Traveler traveler, ActiveSkill skill, List<Unit> turnQueue)
    {
        return skill.Name switch
        {
            "Leghold Trap"  => HandleLegholdTrap(traveler, skill, turnQueue),
            "Spearhead"     => HandleSpearhead(traveler, skill),
            "Revive"        => HandleReviveParty(traveler, skill),
            "Vivify"        => HandleVivify(traveler, skill),
            "Healing Touch" => HandleHealingTouch(traveler, skill),
            _ when skill.IsShootingStars    => HandleShootingStars(traveler, skill),
            _ when skill.IsNightmareChimera => HandleNightmareChimera(traveler, skill),
            _ when skill.IsOffensive        => HandleOffensiveSkill(traveler, skill),
            _ when skill.Modifier > 0 && (skill.Target == "Party" || skill.Target == "Ally")
                                            => HandleHealSkill(traveler, skill),
            _ => false
        };
    }


    private bool HandleOffensiveSkill(Traveler traveler, ActiveSkill skill)
    {
        List<Beast>? targets = skill.Target == "Single"
            ? GetSingleBeastTarget(traveler)
            : _enemyTeam.Where(e => !e.IsDead).ToList();

        if (targets == null || targets.Count == 0) return false;

        _combatView.PromptBpUsageIfAvailable(traveler.CurrentBp);
        traveler.SpendSp(skill.Sp);

        _combatView.ShowUnitUsesSkill(traveler.Name, skill.Name);

        foreach (var target in targets)
            ApplyOffensiveHit(traveler, target, skill);

        foreach (var target in targets)
            _combatView.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }

    private void ApplyOffensiveHit(Traveler traveler, Beast target, ActiveSkill skill)
    {
        bool wasInBp = target.IsInBreakingPoint;
        bool isWeakness = !string.IsNullOrEmpty(skill.Type) && target.Weaknesses.Contains(skill.Type);

        int damage = skill.IsPhysicalOffensive
            ? (skill.HasLastStandMechanic
                ? DamageCalculator.CalculateLastStandDamage(traveler.Stats.PhysicalAttack, skill.Modifier, target.Stats.PhysicalDefense, isWeakness, wasInBp, traveler)
                : DamageCalculator.CalculatePhysicalDamage(traveler.Stats.PhysicalAttack, skill.Modifier, target.Stats.PhysicalDefense, isWeakness, wasInBp))
            : DamageCalculator.CalculateElementalDamage(traveler.Stats.ElementalAttack, skill.Modifier, target.Stats.ElementalDefense, isWeakness, wasInBp);

        if (skill.HasMercyStrikeMechanic)
            damage = Math.Min(damage, Math.Max(0, target.CurrentHp - 1));

        target.TakeDamage(damage);
        _combatView.ShowDamageWithType(target.Name, damage, skill.Type, isWeakness);

        if (isWeakness && damage > 0)
            ProcessShieldDamage(target);
    }


    private bool HandleHealSkill(Traveler traveler, ActiveSkill skill)
    {
        List<Traveler> targets;
        if (skill.Target == "Ally")
        {
            var result = GetSingleAllyTarget(traveler, _playerTeam.Where(t => !t.IsDead).ToList());
            if (result == null) return false;
            targets = result;
        }
        else
        {
            var aliveOthers = _playerTeam.Where(t => !t.IsDead && t != traveler).ToList();
            targets = aliveOthers.Concat(new[] { traveler }).ToList();
        }

        if (targets.Count == 0) return false;

        _combatView.PromptBpUsageIfAvailable(traveler.CurrentBp);
        traveler.SpendSp(skill.Sp);

        int healAmount = DamageCalculator.CalculateHeal(traveler.Stats.ElementalDefense, skill.Modifier);

        _combatView.ShowUnitUsesSkill(traveler.Name, skill.Name);

        foreach (var target in targets)
        {
            _combatView.ShowHeal(target.Name, healAmount);
            target.Heal(healAmount);
        }

        foreach (var target in targets)
            _combatView.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }

    private bool HandleReviveParty(Traveler traveler, ActiveSkill skill)
    {
        _combatView.PromptBpUsageIfAvailable(traveler.CurrentBp);
        traveler.SpendSp(skill.Sp);

        var deadTravelers = _playerTeam.Where(t => t.IsDead).ToList();

        _combatView.ShowUnitUsesSkill(traveler.Name, skill.Name);

        foreach (var target in deadTravelers)
        {
            target.Revive();
            _combatView.ShowRevive(target.Name);
        }

        foreach (var target in deadTravelers)
            _combatView.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }

    private bool HandleVivify(Traveler traveler, ActiveSkill skill)
        => ExecuteTargetedHealOrRevive(traveler, skill, _playerTeam.Where(t => t.IsDead).ToList());

    private bool HandleHealingTouch(Traveler traveler, ActiveSkill skill)
        => ExecuteTargetedHealOrRevive(traveler, skill, _playerTeam.ToList());

    private bool ExecuteTargetedHealOrRevive(Traveler traveler, ActiveSkill skill, List<Traveler> validTargets)
    {
        var targets = GetSingleAllyTarget(traveler, validTargets);
        if (targets == null || targets.Count == 0) return false;

        var target = targets.First();

        _combatView.PromptBpUsageIfAvailable(traveler.CurrentBp);
        traveler.SpendSp(skill.Sp);

        int healAmount = DamageCalculator.CalculateHeal(traveler.Stats.ElementalDefense, skill.Modifier);

        _combatView.ShowUnitUsesSkill(traveler.Name, skill.Name);

        if (target.IsDead)
        {
            target.Revive();
            _combatView.ShowRevive(target.Name);
        }

        _combatView.ShowHeal(target.Name, healAmount);
        target.Heal(healAmount);
        _combatView.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }


    private bool HandleLegholdTrap(Traveler traveler, ActiveSkill skill, List<Unit> turnQueue)
    {
        var targets = GetSingleBeastTarget(traveler);
        if (targets == null || targets.Count == 0) return false;
        var target = targets.First();

        _combatView.PromptBpUsageIfAvailable(traveler.CurrentBp);
        traveler.SpendSp(skill.Sp);

        target.ApplyDesprioritization(DesprioritizationDuration);

        if (turnQueue.Contains(target))
        {
            turnQueue.Remove(target);
            turnQueue.Add(target);
        }

        _combatView.ShowUnitUsesSkill(traveler.Name, skill.Name);
        _combatView.ShowLeghold(target.Name, DesprioritizationDuration);

        return true;
    }

    private bool HandleSpearhead(Traveler traveler, ActiveSkill skill)
    {
        var targets = GetSingleBeastTarget(traveler);
        if (targets == null || targets.Count == 0) return false;

        _combatView.PromptBpUsageIfAvailable(traveler.CurrentBp);
        traveler.SpendSp(skill.Sp);

        _combatView.ShowUnitUsesSkill(traveler.Name, skill.Name);
        ApplyOffensiveHit(traveler, targets.First(), skill);
        _combatView.ShowFinalHp(targets.First().Name, targets.First().CurrentHp);
        traveler.SetPriorityNextRound();

        return true;
    }

    private bool HandleShootingStars(Traveler traveler, ActiveSkill skill)
    {
        _combatView.PromptBpUsageIfAvailable(traveler.CurrentBp);
        traveler.SpendSp(skill.Sp);

        var targets = _enemyTeam.Where(e => !e.IsDead).ToList();
        var hitTypes = new[] { "Wind", "Light", "Dark" };

        _combatView.ShowUnitUsesSkill(traveler.Name, skill.Name);

        foreach (var target in targets)
        {
            foreach (var hitType in hitTypes)
            {
                var tempSkill = new ActiveSkill { Name = skill.Name, Type = hitType, Modifier = skill.Modifier };
                ApplyOffensiveHit(traveler, target, tempSkill);
            }
        }

        foreach (var target in targets)
            _combatView.ShowFinalHp(target.Name, target.CurrentHp);

        return true;
    }

    private bool HandleNightmareChimera(Traveler traveler, ActiveSkill skill)
    {
        var weapons = new[] { "Sword", "Spear", "Dagger", "Axe", "Bow", "Stave" };
        string? chosenWeapon = _combatView.PromptWeaponSelection(weapons);
        if (chosenWeapon == null) return false;

        var targets = GetSingleBeastTarget(traveler);
        if (targets == null || targets.Count == 0) return false;

        _combatView.PromptBpUsageIfAvailable(traveler.CurrentBp);
        traveler.SpendSp(skill.Sp);

        var tempSkill = new ActiveSkill { Name = skill.Name, Type = chosenWeapon, Modifier = skill.Modifier };

        _combatView.ShowUnitUsesSkill(traveler.Name, skill.Name);
        ApplyOffensiveHit(traveler, targets.First(), tempSkill);
        _combatView.ShowFinalHp(targets.First().Name, targets.First().CurrentHp);

        return true;
    }


    private List<Beast>? GetSingleBeastTarget(Traveler traveler)
    {
        var aliveEnemies = _enemyTeam.Where(e => !e.IsDead).ToList();
        var target = _combatView.PromptBeastTargetSelection(traveler.Name, aliveEnemies);
        return target != null ? new List<Beast> { target } : null;
    }

    private List<Traveler>? GetSingleAllyTarget(Traveler traveler, List<Traveler> validTargets)
    {
        var target = _combatView.PromptAllyTargetSelection(traveler.Name, validTargets);
        return target != null ? new List<Traveler> { target } : null;
    }


    private bool CheckAndAnnounceWinner()
    {
        if (_enemyTeam.All(b => b.IsDead))
        {
            _combatView.ShowPlayerTeamWins();
            return true;
        }
        if (_playerTeam.All(t => t.IsDead))
        {
            _combatView.ShowEnemyTeamWins();
            return true;
        }
        return false;
    }
}
