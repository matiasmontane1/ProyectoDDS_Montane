using Octopath_Traveler.Controllers.SkillHandlers;
using Octopath_Traveler.Models;
using Octopath_Traveler.Views;
using Octopath_Traveler_View;

namespace Octopath_Traveler.Controllers;

public class CombatManager
{
    private readonly CombatView _combatView;
    private readonly CombatMenuView _combatMenuView;
    private readonly List<Traveler> _playerTeam;
    private readonly List<Beast> _enemyTeam;
    private readonly List<ActiveSkill> _activeSkills;
    private readonly BeastTurnController _beastTurnController;
    private readonly BasicAttackHandler _basicAttackHandler;
    private readonly TurnQueueManager _queueManager;
    private int _currentRound;

    private const int DesprioritizationDuration = 2;
    private const int MinBpForDivineSkill = 3;

    public CombatManager(
        View view,
        List<Traveler> playerTeam,
        List<Beast> enemyTeam,
        List<ActiveSkill> activeSkills,
        List<BeastSkill> beastSkills)
    {
        _combatView = new CombatView(view);
        _combatMenuView = new CombatMenuView(view);
        _playerTeam = playerTeam;
        _enemyTeam = enemyTeam;
        _activeSkills = activeSkills;
        _beastTurnController = new BeastTurnController(_combatView, playerTeam, beastSkills);
        _basicAttackHandler = new BasicAttackHandler(_combatView, _combatMenuView, enemyTeam);
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

            var roundQueue = _queueManager.GenerateCurrentRoundQueue();
            var turnQueue = roundQueue.Where(unit => !unit.IsDead).ToList();

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

        _combatView.ShowPreTurnContext(_playerTeam, _enemyTeam, turnQueue, _queueManager.GenerateNextRoundPreview(turnQueue));

        bool fled = false;
        if (currentUnit is Traveler traveler)
            fled = HandleTravelerTurn(traveler, turnQueue);
        else if (currentUnit is Beast beast)
            _beastTurnController.HandleTurn(beast);

        turnQueue.RemoveAt(0);
        turnQueue.RemoveAll(unit => unit.IsDead);
        turnQueue.RemoveAll(IsBreakingBeastToRemove);

        return fled;
    }

    private static bool IsBreakingBeastToRemove(Unit unit) =>
        unit is Beast beast && beast.IsInBreakingPoint && !beast.JustRecoveredFromBreakingPoint;

    private void EndOfRoundProcessing()
    {
        foreach (var traveler in _playerTeam.Where(ally => !ally.IsDead))
        {
            if (!traveler.SpentBpThisRound)
                traveler.RecoverBp();
            traveler.ResetBpSpentFlag();
            traveler.ResetDefenseForNewRound();
        }

        foreach (var beast in _enemyTeam.Where(enemy => !enemy.IsDead))
        {
            beast.DecrementBreakingPoint();
            beast.DecrementDesprioritization();
        }

        foreach (var traveler in _playerTeam)
            traveler.TickStatusEffects();
        foreach (var beast in _enemyTeam)
            beast.TickStatusEffects();
    }

    private bool HandleTravelerTurn(Traveler traveler, List<Unit> turnQueue)
    {
        if (traveler.IsDead) return false;

        traveler.ConsumeTurnStartStates();

        while (true)
        {
            _combatMenuView.ShowTravelerActionMenu(traveler.Name);
            string input = _combatMenuView.ReadLine();

            var action = ParseValidAction(input);
            if (action == null) continue;

            switch (action.Value)
            {
                case TravelerAction.BasicAttack:
                    if (_basicAttackHandler.Execute(traveler)) return false;
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

    private static TravelerAction? ParseValidAction(string input)
    {
        if (!Enum.TryParse<TravelerAction>(input, out var action) || !Enum.IsDefined(action))
            return null;
        return action;
    }

    private bool HandleSkillMenu(Traveler traveler, List<Unit> turnQueue)
    {
        var availableSkills = GetAvailableSkillsFor(traveler);

        string? skillName = _combatMenuView.PromptSkillSelection(traveler.Name, availableSkills);
        if (skillName == null) return false;

        var skill = _activeSkills.FirstOrDefault(activeSkill => activeSkill.Name == skillName);
        if (skill == null) return false;

        return ExecuteSkill(traveler, skill, turnQueue);
    }

    private List<string> GetAvailableSkillsFor(Traveler traveler)
    {
        return traveler.Skills
            .Where(skillName => CanAffordSkill(traveler, skillName))
            .Where(skillName => IsDivineSkillAvailable(traveler, skillName))
            .ToList();
    }

    private bool CanAffordSkill(Traveler traveler, string skillName)
    {
        var skill = _activeSkills.FirstOrDefault(activeSkill => activeSkill.Name == skillName);
        return skill != null && traveler.CurrentSp >= skill.Sp;
    }

    private bool IsDivineSkillAvailable(Traveler traveler, string skillName)
    {
        var skill = _activeSkills.FirstOrDefault(activeSkill => activeSkill.Name == skillName);
        return skill == null || !skill.IsDivine || traveler.CurrentBp >= MinBpForDivineSkill;
    }

    private bool ExecuteSkill(Traveler traveler, ActiveSkill skill, List<Unit> turnQueue)
    {
        var handler = SkillHandlerFactory.Create(skill, _combatView, _combatMenuView, _playerTeam, _enemyTeam, DesprioritizationDuration);
        return handler.Execute(traveler, skill, turnQueue);
    }

    private bool CheckAndAnnounceWinner()
    {
        if (_enemyTeam.All(beast => beast.IsDead))
        {
            _combatView.ShowPlayerTeamWins();
            return true;
        }
        if (_playerTeam.All(ally => ally.IsDead))
        {
            _combatView.ShowEnemyTeamWins();
            return true;
        }
        return false;
    }
}
